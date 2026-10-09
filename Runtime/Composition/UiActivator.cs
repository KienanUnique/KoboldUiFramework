using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace KoboldUi.Composition
{
    public sealed class UiActivator
    {
        private const BindingFlags Members = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private static readonly Dictionary<Type, InjectionPlan> Plans = new Dictionary<Type, InjectionPlan>();
        private readonly IUiResolver _resolver;
        private readonly Action<object> _afterControllerCreated;

        public UiActivator(IUiResolver resolver, Action<object> afterControllerCreated = null)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _afterControllerCreated = afterControllerCreated;
        }

        public void InjectGameObject(GameObject root)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));
            foreach (var component in root.GetComponentsInChildren<Component>(true))
            {
                if (component == null) continue;
                if (component is IUiActivatorAware aware) aware.Configure(this);
                Inject(component, root.name);
            }
        }

        public T InstantiatePrefab<T>(T prefab) where T : Component
        {
            if (prefab == null) throw new ArgumentNullException(nameof(prefab));
            var staging = new GameObject("KoboldUi injection staging");
            staging.SetActive(false);
            try
            {
                var instance = UnityEngine.Object.Instantiate(prefab, staging.transform);
                InjectGameObject(instance.gameObject);
                instance.transform.SetParent(null, false);
                return instance;
            }
            finally
            {
                if (Application.isPlaying) UnityEngine.Object.Destroy(staging);
                else UnityEngine.Object.DestroyImmediate(staging);
            }
        }

        public void Inject(object target, string context = null)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            var plan = GetPlan(target.GetType());
            var path = context == null ? target.GetType().Name : context + " -> " + target.GetType().Name;
            foreach (var field in plan.Fields)
                field.SetValue(target, Resolve(field.FieldType, field.GetCustomAttribute<UiInjectAttribute>().Optional, path));
            foreach (var property in plan.Properties)
                property.SetValue(target, Resolve(property.PropertyType, property.GetCustomAttribute<UiInjectAttribute>().Optional, path));
            foreach (var method in plan.Methods)
            {
                var parameters = method.GetParameters();
                var args = new object[parameters.Length];
                for (var i = 0; i < args.Length; i++)
                    args[i] = ResolveParameter(parameters[i], method.GetCustomAttribute<UiInjectAttribute>().Optional, path);
                try { method.Invoke(target, args); }
                catch (TargetInvocationException e) { throw new UiActivationException($"Injection failed: {path}.{method.Name}", e.InnerException); }
            }
        }

        public TController CreateController<TController, TView>(TView view) where TController : class
        {
            if (ReferenceEquals(view, null)) throw new ArgumentNullException(nameof(view));
            var type = typeof(TController);
            var constructors = type.GetConstructors(Members);
            if (constructors.Length != 1)
                throw new UiActivationException($"{type.FullName} must have exactly one constructor.");
            var parameters = constructors[0].GetParameters();
            var args = new object[parameters.Length];
            var viewConsumed = false;
            for (var i = 0; i < args.Length; i++)
            {
                if (!viewConsumed && parameters[i].ParameterType.IsInstanceOfType(view))
                {
                    args[i] = view;
                    viewConsumed = true;
                }
                else args[i] = ResolveParameter(parameters[i], false, type.Name);
            }
            TController controller;
            try { controller = (TController)constructors[0].Invoke(args); }
            catch (TargetInvocationException e) { throw new UiActivationException($"Controller creation failed: {type.FullName}", e.InnerException); }
            if (controller is Element.Controller.IUiViewReceiver<TView> receiver) receiver.SetView(view);
            Inject(controller, type.Name);
            _afterControllerCreated?.Invoke(controller);
            return controller;
        }

        private object Resolve(Type type, bool optional, string path)
        {
            if (!_resolver.TryResolve(type, out var value) || value == null)
            {
                if (optional) return type.IsValueType ? Activator.CreateInstance(type) : null;
                throw new UiActivationException($"Missing dependency {type.FullName} while activating {path}.");
            }
            if (!type.IsInstanceOfType(value))
                throw new UiActivationException($"Resolver returned {value.GetType().FullName} for {type.FullName} while activating {path}.");
            return value;
        }

        private object ResolveParameter(ParameterInfo parameter, bool optional, string path)
        {
            if (parameter.IsOptional)
            {
                if (!_resolver.TryResolve(parameter.ParameterType, out var value)) return parameter.DefaultValue;
                if (value == null || !parameter.ParameterType.IsInstanceOfType(value))
                    throw new UiActivationException($"Invalid dependency {parameter.ParameterType.FullName} while activating {path}.");
                return value;
            }
            return Resolve(parameter.ParameterType, optional, path);
        }

        private static InjectionPlan GetPlan(Type type)
        {
            lock (Plans)
            {
                if (Plans.TryGetValue(type, out var plan)) return plan;
                plan = new InjectionPlan();
                for (var current = type; current != null && current != typeof(object); current = current.BaseType)
                {
                    foreach (var field in current.GetFields(Members | BindingFlags.DeclaredOnly))
                        if (field.IsDefined(typeof(UiInjectAttribute), false)) plan.Fields.Add(field);
                    foreach (var property in current.GetProperties(Members | BindingFlags.DeclaredOnly))
                        if (property.IsDefined(typeof(UiInjectAttribute), false)) plan.Properties.Add(property);
                    foreach (var method in current.GetMethods(Members | BindingFlags.DeclaredOnly))
                        if (method.IsDefined(typeof(UiInjectAttribute), false)) plan.Methods.Add(method);
                }
                Plans.Add(type, plan);
                return plan;
            }
        }

        private sealed class InjectionPlan
        {
            public readonly List<FieldInfo> Fields = new List<FieldInfo>();
            public readonly List<PropertyInfo> Properties = new List<PropertyInfo>();
            public readonly List<MethodInfo> Methods = new List<MethodInfo>();
        }
    }
}

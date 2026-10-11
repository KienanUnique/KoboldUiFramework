using System;
using KoboldUi.Element.Animations;
using KoboldUi.Element.Controller;
using KoboldUi.Services.WindowsService;
using UnityEngine;
using UnityEngine.Pool;

namespace KoboldUi.Composition
{
    /// <summary>Creates UI objects and applies optional composition before initialization.</summary>
    public sealed class UiActivator
    {
        private readonly IUiCompositionAdapter _adapter;
        private readonly UiAnimationDefaults _animationDefaults;
        public ILocalWindowsService LocalWindows { get; }
        public IProjectWindowsService ProjectWindows { get; }

        public UiActivator(IUiCompositionAdapter adapter = null, UiAnimationDefaults animationDefaults = null,
            ILocalWindowsService localWindows = null, IProjectWindowsService projectWindows = null)
        {
            _adapter = adapter;
            _animationDefaults = animationDefaults;
            LocalWindows = localWindows;
            ProjectWindows = projectWindows;
        }

        public void PrepareGameObject(GameObject root)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));
            if (_adapter == null && _animationDefaults == null) return;

            var components = ListPool<Component>.Get();
            try
            {
                root.GetComponentsInChildren(true, components);
                foreach (var component in components)
                {
                    if (component == null) continue;
                    if (component is IUiActivatorAware aware) aware.Configure(this);
                    if (_animationDefaults != null && component is IUiAnimationDefaultsReceiver receiver)
                        receiver.ApplyDefaults(_animationDefaults);
                    _adapter?.Inject(component);
                }
            }
            finally
            {
                ListPool<Component>.Release(components);
            }
        }

        public T InstantiatePrefab<T>(T prefab) where T : Component
        {
            if (prefab == null) throw new ArgumentNullException(nameof(prefab));
            if (_adapter == null && _animationDefaults == null)
                return UnityEngine.Object.Instantiate(prefab);

            var staging = new GameObject("KoboldUi composition staging");
            staging.SetActive(false);
            try
            {
                var instance = UnityEngine.Object.Instantiate(prefab, staging.transform);
                PrepareGameObject(instance.gameObject);
                instance.transform.SetParent(null, false);
                return instance;
            }
            finally
            {
                if (Application.isPlaying) UnityEngine.Object.Destroy(staging);
                else UnityEngine.Object.DestroyImmediate(staging);
            }
        }

        public TController CreateController<TController, TView>(TView view, Func<TView, TController> factory)
            where TController : class
        {
            if (ReferenceEquals(view, null)) throw new ArgumentNullException(nameof(view));
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return BindView(factory(view), view);
        }

        public TController CreateController<TController, TView>(TView view) where TController : class
        {
            if (ReferenceEquals(view, null)) throw new ArgumentNullException(nameof(view));
            if (_adapter != null)
            {
                var count = 1 + (LocalWindows == null ? 0 : 1) + (ProjectWindows == null ? 0 : 1);
                var arguments = new object[count];
                arguments[0] = view;
                var index = 1;
                if (LocalWindows != null) arguments[index++] = LocalWindows;
                if (ProjectWindows != null) arguments[index] = ProjectWindows;
                return BindView(_adapter.Create<TController>(arguments), view);
            }

            try
            {
                return BindView(System.Activator.CreateInstance<TController>(), view);
            }
            catch (MissingMethodException exception)
            {
                throw new UiActivationException(
                    $"{typeof(TController).FullName} needs a public parameterless constructor or a controller factory.",
                    exception);
            }
        }

        public TController CreateController<TController, TView>(TView view, params object[] additionalArguments)
            where TController : class
        {
            if (ReferenceEquals(view, null)) throw new ArgumentNullException(nameof(view));
            if (_adapter == null)
                throw new UiActivationException($"Creating {typeof(TController).FullName} requires a controller factory or a composition adapter.");
            additionalArguments = additionalArguments ?? Array.Empty<object>();
            var arguments = new object[additionalArguments.Length + 1];
            arguments[0] = view;
            Array.Copy(additionalArguments, 0, arguments, 1, additionalArguments.Length);
            return BindView(_adapter.Create<TController>(arguments), view);
        }

        private static TController BindView<TController, TView>(TController controller, TView view)
            where TController : class
        {
            if (controller == null) throw new UiActivationException($"The factory returned null for {typeof(TController).FullName}.");
            if (controller is IUiViewReceiver<TView> receiver) receiver.SetView(view);
            return controller;
        }
    }
}

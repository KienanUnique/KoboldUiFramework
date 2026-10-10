using System;
using System.Collections.Generic;
using System.Reflection;
using KoboldUi.Composition;
using Zenject;

namespace KoboldUi.Composition.Zenject
{
    /// <summary>Composes UI objects with an existing Zenject container.</summary>
    public sealed class ZenjectUiCompositionAdapter : IUiCompositionAdapter
    {
        private readonly DiContainer _container;

        public ZenjectUiCompositionAdapter(DiContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public void Inject(object instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            _container.Inject(instance);
        }

        public T Create<T>(params object[] arguments) where T : class
        {
            arguments = arguments ?? Array.Empty<object>();
            var constructors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            ConstructorInfo selected = null;
            foreach (var constructor in constructors)
                if (constructor.IsDefined(typeof(InjectAttribute), true))
                {
                    selected = constructor;
                    break;
                }
            if (selected == null && constructors.Length == 1) selected = constructors[0];
            if (selected == null) return (T)_container.Instantiate(typeof(T), arguments);

            var accepted = new List<object>();
            var parameters = selected.GetParameters();
            foreach (var argument in arguments)
            {
                if (argument == null) throw new ArgumentException("Explicit constructor arguments cannot be null.", nameof(arguments));
                foreach (var parameter in parameters)
                    if (parameter.ParameterType.IsInstanceOfType(argument))
                    {
                        accepted.Add(argument);
                        break;
                    }
            }
            return (T)_container.Instantiate(typeof(T), accepted);
        }
    }
}

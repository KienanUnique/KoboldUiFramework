using System;
using KoboldUi.Composition;
using VContainer;
using VContainer.Internal;
using System.Collections.Generic;

namespace KoboldUi.Adapters.VContainer
{
    /// <summary>
    /// Composes KoboldUi objects with an existing VContainer scope.
    /// The caller retains ownership of the scope.
    /// </summary>
    public sealed class VContainerUiCompositionAdapter : IUiCompositionAdapter
    {
        private IObjectResolver _container;

        public VContainerUiCompositionAdapter(IObjectResolver container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        internal VContainerUiCompositionAdapter() { }

        internal void Attach(IObjectResolver container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public void Inject(object instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            EnsureAttached();
            _container.Inject(instance);
        }

        public T Create<T>(params object[] arguments) where T : class
        {
            EnsureAttached();
            var parameters = new List<IInjectParameter>();
            if (arguments != null)
                foreach (var argument in arguments)
                {
                    if (argument == null) throw new ArgumentException("Explicit constructor arguments cannot be null.", nameof(arguments));
                    parameters.Add(new ExplicitArgument(argument));
                }
            return (T)InjectorCache.GetOrBuild(typeof(T)).CreateInstance(_container, parameters);
        }

        private void EnsureAttached()
        {
            if (_container == null)
                throw new InvalidOperationException("VContainer is not built yet. Create UI objects after building its scope.");
        }

        private sealed class ExplicitArgument : IInjectParameter
        {
            private readonly object _value;
            public ExplicitArgument(object value) { _value = value; }
            public bool Match(Type parameterType, string parameterName) => parameterType.IsInstanceOfType(_value);
            public object GetValue(IObjectResolver resolver) => _value;
        }
    }
}

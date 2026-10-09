using System;
using System.Collections.Generic;

namespace KoboldUi.Composition.Manual
{
    /// <summary>
    /// Resolves explicitly registered UI dependencies without a DI container.
    /// </summary>
    public sealed class ManualUiResolver : IUiResolver
    {
        private readonly Dictionary<Type, Func<object>> _registrations = new Dictionary<Type, Func<object>>();
        private readonly IUiResolver _parent;

        public ManualUiResolver(IUiResolver parent = null)
        {
            _parent = parent;
        }

        /// <summary>
        /// Creates a local scope that inherits registrations from this resolver.
        /// </summary>
        public ManualUiResolver CreateScope()
        {
            return new ManualUiResolver(this);
        }

        /// <summary>
        /// Registers one instance under the specified contract.
        /// </summary>
        public ManualUiResolver AddInstance<T>(T instance)
        {
            if (ReferenceEquals(instance, null))
                throw new ArgumentNullException(nameof(instance));

            AddRegistration(typeof(T), () => instance);
            return this;
        }

        /// <summary>
        /// Registers a factory that creates a value on each resolution.
        /// </summary>
        public ManualUiResolver AddFactory<T>(Func<T> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            AddRegistration(typeof(T), () => factory());
            return this;
        }

        /// <inheritdoc />
        public bool TryResolve(Type type, out object instance)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (_registrations.TryGetValue(type, out var factory))
            {
                instance = factory();
                if (instance == null)
                    throw new InvalidOperationException($"The factory for {type.FullName} returned null.");

                return true;
            }

            if (_parent != null)
                return _parent.TryResolve(type, out instance);

            instance = null;
            return false;
        }

        private void AddRegistration(Type type, Func<object> factory)
        {
            if (_registrations.ContainsKey(type))
                throw new InvalidOperationException($"A value for {type.FullName} is already registered.");

            _registrations.Add(type, factory);
        }
    }
}

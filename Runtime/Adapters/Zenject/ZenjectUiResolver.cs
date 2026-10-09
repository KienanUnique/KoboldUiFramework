using System;
using KoboldUi.Composition;
using Zenject;

namespace KoboldUi.Composition.Zenject
{
    /// <summary>Resolves UI dependencies from a Zenject container.</summary>
    public sealed class ZenjectUiResolver : IUiResolver
    {
        private readonly DiContainer _container;

        public ZenjectUiResolver(DiContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public bool TryResolve(Type type, out object instance)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!_container.HasBinding(type))
            {
                instance = null;
                return false;
            }

            instance = _container.Resolve(type);
            return true;
        }
    }
}

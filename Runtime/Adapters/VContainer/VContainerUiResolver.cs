using System;
using KoboldUi.Composition;
using VContainer;

namespace KoboldUi.Adapters.VContainer
{
    /// <summary>
    /// Resolves KoboldUi dependencies from an existing VContainer scope.
    /// The caller retains ownership of the scope.
    /// </summary>
    public sealed class VContainerUiResolver : IUiResolver
    {
        private readonly IObjectResolver _container;

        public VContainerUiResolver(IObjectResolver container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public bool TryResolve(Type type, out object value)
        {
            return _container.TryResolve(type, out value);
        }
    }
}

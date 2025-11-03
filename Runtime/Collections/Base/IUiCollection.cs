using System.Collections.Generic;

namespace KoboldUi.Collections.Base
{
    /// <summary>
    /// Represents a read-only collection of UI views managed by a collection component.
    /// </summary>
    /// <typeparam name="TView">Type of view stored in the collection.</typeparam>
    public interface IUiCollection<out TView> : IEnumerable<TView>
        where TView : IUiCollectionView
    {
        /// <summary>
        /// Gets the number of active views currently tracked by the collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns all managed views to the pool and clears the collection.
        /// </summary>
        void Clear();
    }
}

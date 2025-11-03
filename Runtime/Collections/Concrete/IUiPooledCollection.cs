using KoboldUi.Collections.Base;

namespace KoboldUi.Collections.Concrete
{
    /// <summary>
    /// Represents a collection that can both create and recycle view instances.
    /// </summary>
    /// <typeparam name="TView">Type of view managed by the pool.</typeparam>
    public interface IUiPooledCollection<TView> : IUiFactory<TView>, IUiCollection<TView>
        where TView : IUiCollectionView
    {
        /// <summary>
        /// Returns a view to the pool for reuse.
        /// </summary>
        /// <param name="view">View instance to recycle.</param>
        void ReturnToPool(TView view);
    }
}

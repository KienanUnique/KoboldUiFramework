using KoboldUi.Collections.Base;

namespace KoboldUi.Collections.Concrete
{
    /// <summary>
    /// Represents an indexed collection of UI views with removal support.
    /// </summary>
    /// <typeparam name="TView">Type of view stored in the collection.</typeparam>
    public interface IUiListCollection<TView> : IUiFactory<TView>, IUiCollection<TView> where TView : IUiCollectionView
    {
        /// <summary>
        /// Gets the view at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index of the view.</param>
        TView this[int index] { get; }

        /// <summary>
        /// Removes the specified view from the collection.
        /// </summary>
        /// <param name="view">View to remove.</param>
        void Remove(TView view);

        /// <summary>
        /// Removes the view at the specified index from the collection.
        /// </summary>
        /// <param name="index">Zero-based index of the view to remove.</param>
        void RemoveAt(int index);
    }
}

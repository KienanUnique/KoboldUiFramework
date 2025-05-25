using KoboldUi.Collections.Base;

namespace KoboldUi.Collections.Concrete
{
    public interface IUiListCollection<TView> : IUiFactory<TView>, IUiCollection<TView> where TView : IUiCollectionView
    {
        TView this[int index] { get; }

        void Remove(TView view);

        void RemoveAt(int index);
    }
}
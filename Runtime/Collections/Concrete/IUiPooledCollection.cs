using KoboldUi.Collections.Base;

namespace KoboldUi.Collections.Concrete
{
    public interface IUiPooledCollection<TView> : IUiFactory<TView>, IUiCollection<TView>
        where TView : IUiCollectionView
    {
        void ReturnToPool(TView view);
    }
}
using KoboldUi.Collections.Base;
using KoboldUi.Element.View;

namespace KoboldUi.Collections.Concrete
{
    public interface IUiListCollectionBase<TView> : IUiFactory<TView>, IUiCollectionBase<TView> where TView : IUiView
    {
        TView this[int index] { get; }

        void Remove(TView view);

        void RemoveAt(int index);
    }
}
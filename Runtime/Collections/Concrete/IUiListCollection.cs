using KoboldUi.Collections.Base;
using KoboldUi.Element.View;

namespace KoboldUi.Collections.Concrete
{
    public interface IUiListCollection<TView> : IUiFactory<TView>, IUiCollection<TView> where TView : IUiView
    {
        TView this[int index] { get; }

        void Remove(TView view);

        void RemoveAt(int index);
    }
}
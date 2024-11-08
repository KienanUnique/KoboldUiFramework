using KoboldUi.Collections.Base;
using KoboldUi.Element.View;

namespace KoboldUi.Collections.Concrete
{
    public interface IUiPooledCollection<TView> : IUiFactory<TView>, IUiCollection<TView> where TView : IUiView
    {
        void ReturnToPool(TView view);
    }
}
using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;

namespace KoboldUi.Element.View
{
    public interface IUiView
    {
        void Initialize();

        IUiAction Open(in IUiActionsPool pool);
        IUiAction ReturnFocus(in IUiActionsPool pool);
        IUiAction RemoveFocus(in IUiActionsPool pool);
        IUiAction Close(in IUiActionsPool pool);
        void CloseInstantly();
    }
}
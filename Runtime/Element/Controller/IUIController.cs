using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;

namespace KoboldUi.Element.Controller
{
    public interface IUIController
    {
        IUiAction SetState(EWindowState state, in IUiActionsPool pool);
    }
}
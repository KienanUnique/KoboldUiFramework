using KoboldUi.UiAction;
using KoboldUi.Utils;

namespace KoboldUi.Element.Controller
{
    public interface IUIController
    {
        IUiAction SetState(EWindowState state);
    }
}
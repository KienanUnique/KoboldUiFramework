using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;

namespace KoboldUi.Windows
{
    public interface IWindow
    {
        bool IsInitialized { get; }
        string Name { get; }
        bool IsPopup { get; }
        bool IsBackLogicIgnorable { get; }

        IUiAction WaitInitialization(in IUiActionsPool pool);
        IUiAction SetState(EWindowState state, in IUiActionsPool pool);
        void ApplyOrder(int order);
    }
}
using KoboldUi.UiAction;
using KoboldUi.Utils;

namespace KoboldUi.Windows
{
    public interface IWindow
    {
        bool IsInitialized { get; }
        string Name { get; }

        IUiAction WaitInitialization();
        IUiAction SetState(EWindowState state);
        void ApplyOrder(int order);
    }
}
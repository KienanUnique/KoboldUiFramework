using KoboldUi.Utils;
using UniRx;

namespace KoboldUi.Windows
{
    public interface IWindow
    {
        IReactiveProperty<bool> IsInitialized { get; }
        string Name { get; }

        void SetState(EWindowState state);
        void ApplyOrder(int order);
    }
}
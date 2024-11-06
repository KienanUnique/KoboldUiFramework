using Cysharp.Threading.Tasks;
using KoboldUi.Utils;

namespace KoboldUi.Windows
{
    public interface IWindow
    {
        bool IsInitialized { get; }
        string Name { get; }

        UniTask WaitInitialization();
        UniTask SetState(EWindowState state);
        void ApplyOrder(int order);
    }
}
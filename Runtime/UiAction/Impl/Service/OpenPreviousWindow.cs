using Cysharp.Threading.Tasks;
using KoboldUi.Utils;
using KoboldUi.WindowsStack;

namespace KoboldUi.UiAction.Impl.Service
{
    public class OpenPreviousWindow : IUiAction
    {
        private IWindowsStackHolder _windowsStackHolder;

        public void Setup(IWindowsStackHolder windowsStackHolder)
        {
            _windowsStackHolder = windowsStackHolder;
        }

        public UniTask Start()
        {
            if (_windowsStackHolder.IsEmpty)
                return UniTask.CompletedTask;

            var currentWindow = _windowsStackHolder.CurrentWindow;
            var action = currentWindow.SetState(EWindowState.Active);
            return action.Start();
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }
    }
}
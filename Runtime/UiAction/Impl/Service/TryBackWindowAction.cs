using Cysharp.Threading.Tasks;
using KoboldUi.Interfaces;
using KoboldUi.Utils;
using KoboldUi.Windows;
using KoboldUi.WindowsStack;

namespace KoboldUi.UiAction.Impl.Service
{
    public class TryBackWindowAction: IUiAction
    {
        private IWindow _windowToClose;
        private IWindowsStackHolder _windowsStackHolder;

        public void Setup(IWindowsStackHolder windowsStackHolder)
        {
            _windowsStackHolder = windowsStackHolder;
            _windowToClose = windowsStackHolder.CurrentWindow;
        }

        public UniTask Start()
        {
            if (_windowToClose == null || _windowsStackHolder.IsEmpty || _windowsStackHolder.CurrentWindow != _windowToClose)
                return UniTask.CompletedTask;

            var currentWindow = _windowsStackHolder.CurrentWindow;

            var isWindowIgnoreBackSignal = currentWindow is IBackLogicIgnorable;
            if (isWindowIgnoreBackSignal)
                return UniTask.CompletedTask;

            return BackWindow(_windowsStackHolder.Pop());
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        private async UniTask BackWindow(IWindow currentWindow)
        {
            await currentWindow.SetState(EWindowState.Closed).Start();

            WindowsOrdersManager.HandleWindowDisappear(_windowsStackHolder.Stack, currentWindow);
            var openPreviousWindow = new OpenPreviousWindow();
            openPreviousWindow.Setup(_windowsStackHolder);

            await openPreviousWindow.Start();
        }
    }
}
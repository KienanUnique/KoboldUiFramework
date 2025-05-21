using Cysharp.Threading.Tasks;
using KoboldUi.Interfaces;
using KoboldUi.Utils;
using KoboldUi.Windows;
using KoboldUi.WindowsStack;

namespace KoboldUi.UiAction.Impl.Service
{
    public class OpenWindowAction : IUiAction
    {
        private IWindow _windowToOpen;
        private IWindowsStackHolder _windowsStackHolder;

        public void Setup(IWindow windowToOpen, IWindowsStackHolder windowsStackHolder)
        {
            _windowToOpen = windowToOpen;
            _windowsStackHolder = windowsStackHolder;
        }

        public UniTask Start()
        {
            return _windowsStackHolder.IsOpened(_windowToOpen) ? UniTask.CompletedTask : OpenWindow();
        }

        public void Dispose()
        {
            _windowToOpen = null;
        }

        private async UniTask OpenWindow()
        {
            var isNextWindowPopUp = _windowToOpen is IPopUp;
            if (!_windowsStackHolder.IsEmpty)
            {
                var currentWindow = _windowsStackHolder.CurrentWindow;
                var newState = isNextWindowPopUp ? EWindowState.NonFocused : EWindowState.Closed;
                await currentWindow.SetState(newState).Start();
            }

            if (!_windowToOpen.IsInitialized)
                await _windowToOpen.WaitInitialization().Start();

            WindowsOrdersManager.HandleWindowAppear(_windowsStackHolder.Stack, _windowToOpen);
            _windowsStackHolder.Push(_windowToOpen);

            await _windowToOpen.SetState(EWindowState.Active).Start();
        }
    }
}
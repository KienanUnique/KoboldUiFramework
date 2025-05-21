using Cysharp.Threading.Tasks;
using KoboldUi.Utils;
using KoboldUi.Windows;
using KoboldUi.WindowsStack;

namespace KoboldUi.UiAction.Impl.Service
{
    public class OpenPreviousWindow : IUiAction
    {
        private IWindow _windowToOpen;
        private IWindowsStackHolder _windowsStackHolder;

        public void Setup(IWindowsStackHolder windowsStackHolder)
        {
            _windowsStackHolder = windowsStackHolder;
            _windowToOpen = windowsStackHolder.CurrentWindow;
        }

        public UniTask Start()
        {
            if (_windowToOpen == null || _windowsStackHolder.IsEmpty || _windowsStackHolder.CurrentWindow != _windowToOpen)
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
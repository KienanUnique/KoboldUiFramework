using Cysharp.Threading.Tasks;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using KoboldUi.Windows;
using KoboldUi.WindowsStack;

namespace KoboldUi.UiAction.Impl.Service
{
    public class OpenPreviousWindowAction : AUiAction
    {
        private readonly IWindowsStackHolder _windowsStackHolder;

        private IWindow _windowToOpen;

        public OpenPreviousWindowAction(
            IUiActionsPool pool,
            IWindowsStackHolder windowsStackHolder
        ) : base(pool)
        {
            _windowsStackHolder = windowsStackHolder;
        }

        public void Setup()
        {
            _windowToOpen = _windowsStackHolder.CurrentWindow;
        }

        public override void Dispose()
        {
            _windowToOpen = null;
        }

        protected override UniTask HandleStart()
        {
            if (_windowToOpen == null || _windowsStackHolder.IsEmpty ||
                _windowsStackHolder.CurrentWindow != _windowToOpen)
                return UniTask.CompletedTask;

            var currentWindow = _windowsStackHolder.CurrentWindow;
            var action = currentWindow.SetState(EWindowState.Active, Pool);
            return action.Start();
        }

        protected override void ReturnToPool()
        {
            Pool.ReturnAction(this);
        }
    }
}
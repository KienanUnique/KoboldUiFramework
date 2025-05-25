using Cysharp.Threading.Tasks;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using KoboldUi.Windows;
using KoboldUi.WindowsStack;

namespace KoboldUi.UiAction.Impl.Service
{
    public class CloseWindowAction : AUiAction
    {
        private readonly IWindowsStackHolder _windowsStackHolder;

        private IWindow _windowToClose;
        private bool _useBackLogicIgnorableChecks;

        public CloseWindowAction(
            IUiActionsPool pool,
            IWindowsStackHolder windowsStackHolder
        ) : base(pool)
        {
            _windowsStackHolder = windowsStackHolder;
        }

        public void Setup(bool useBackLogicIgnorableChecks)
        {
            _windowToClose = _windowsStackHolder.CurrentWindow;
            _useBackLogicIgnorableChecks = useBackLogicIgnorableChecks;
        }

        public override void Dispose()
        {
            _windowToClose = null;
        }

        protected override UniTask HandleStart()
        {
            if (_windowToClose == null || _windowsStackHolder.IsEmpty ||
                _windowsStackHolder.CurrentWindow != _windowToClose)
                return UniTask.CompletedTask;

            var currentWindow = _windowsStackHolder.CurrentWindow;
            
            if (_useBackLogicIgnorableChecks && currentWindow.IsBackLogicIgnorable)
                return UniTask.CompletedTask;

            return BackWindow(_windowsStackHolder.Pop());
        }

        protected override void ReturnToPool()
        {
            _windowToClose = null;
            Pool.ReturnAction(this);
        }

        private async UniTask BackWindow(IWindow currentWindow)
        {
            await currentWindow.SetState(EWindowState.Closed, Pool).Start();

            WindowsOrdersManager.HandleWindowDisappear(_windowsStackHolder.Stack, currentWindow);
            Pool.GetAction(out OpenPreviousWindowAction openPreviousWindow);

            await openPreviousWindow.Start();
        }
    }
}
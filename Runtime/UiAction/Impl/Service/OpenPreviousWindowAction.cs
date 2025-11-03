using Cysharp.Threading.Tasks;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using KoboldUi.Windows;
using KoboldUi.WindowsStack;

namespace KoboldUi.UiAction.Impl.Service
{
    /// <summary>
    /// Reopens the window below the top of the stack after another window closes.
    /// </summary>
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

        /// <summary>
        /// Captures the window that should regain focus.
        /// </summary>
        public void Setup()
        {
            _windowToOpen = _windowsStackHolder.CurrentWindow;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            _windowToOpen = null;
        }

        /// <inheritdoc />
        protected override UniTask HandleStart()
        {
            if (_windowToOpen == null || _windowsStackHolder.IsEmpty ||
                _windowsStackHolder.CurrentWindow != _windowToOpen)
                return UniTask.CompletedTask;

            var currentWindow = _windowsStackHolder.CurrentWindow;
            var action = currentWindow.SetState(EWindowState.Active, Pool);
            return action.Start();
        }

        /// <inheritdoc />
        protected override void ReturnToPool()
        {
            _windowToOpen = null;
            Pool.ReturnAction(this);
        }
    }
}
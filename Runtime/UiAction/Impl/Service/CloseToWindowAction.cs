using Cysharp.Threading.Tasks;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using KoboldUi.Windows;
using KoboldUi.WindowsStack;

namespace KoboldUi.UiAction.Impl.Service
{
    /// <summary>
    /// Closes windows until a target window becomes the top of the stack.
    /// </summary>
    public class CloseToWindowAction : AUiAction
    {
        private readonly IWindowsStackHolder _windowsStackHolder;

        private IWindow _targetWindow;
        private bool _useBackLogicIgnorableChecks;

        public CloseToWindowAction(
            IUiActionsPool pool,
            IWindowsStackHolder windowsStackHolder
        ) : base(pool)
        {
            _windowsStackHolder = windowsStackHolder;
        }

        /// <summary>
        /// Configures the window to keep and whether back-logic checks should be enforced.
        /// </summary>
        /// <param name="targetWindow">Window that should remain open.</param>
        /// <param name="useBackLogicIgnorableChecks">When true, respects back-logic ignore flags.</param>
        public void Setup(IWindow targetWindow, bool useBackLogicIgnorableChecks)
        {
            _targetWindow = targetWindow;
            _useBackLogicIgnorableChecks = useBackLogicIgnorableChecks;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            _targetWindow = null;
        }

        /// <inheritdoc />
        protected override UniTask HandleStart()
        {
            if (_targetWindow == null || _windowsStackHolder.IsEmpty ||
                !_windowsStackHolder.Contains(_targetWindow))
                return UniTask.CompletedTask;

            var currentWindow = _windowsStackHolder.CurrentWindow;
            
            if (_useBackLogicIgnorableChecks && currentWindow.IsBackLogicIgnorable)
                return UniTask.CompletedTask;

            return BackToWindow(_targetWindow);
        }

        /// <inheritdoc />
        protected override void ReturnToPool()
        {
            _targetWindow = null;
            _useBackLogicIgnorableChecks = false;
            Pool.ReturnAction(this);
        }

        private async UniTask BackToWindow(IWindow targetWindow)
        {
            var currentWindow = _windowsStackHolder.CurrentWindow;
            while (currentWindow != targetWindow)
            {
                if (_useBackLogicIgnorableChecks && currentWindow.IsBackLogicIgnorable)
                    return;

                _windowsStackHolder.Pop();
                await currentWindow.SetState(EWindowState.Closed, Pool).Start();
                WindowsOrdersManager.UpdateWindowsLayers(_windowsStackHolder.Stack);
                
                currentWindow = _windowsStackHolder.CurrentWindow;
            }
            
            Pool.GetAction(out OpenPreviousWindowAction openPreviousWindow);

            await openPreviousWindow.Start();
        }
    }
}
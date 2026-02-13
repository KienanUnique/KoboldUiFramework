using Cysharp.Threading.Tasks;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using KoboldUi.WindowsStack;

namespace KoboldUi.UiAction.Impl.Service
{
    /// <summary>
    /// Closes all windows from top to bottom until the stack is empty or an ignorable window is reached.
    /// </summary>
    public class CloseAllWindowsAction : AUiAction
    {
        private readonly IWindowsStackHolder _windowsStackHolder;

        private bool _useBackLogicIgnorableChecks;

        public CloseAllWindowsAction(
            IUiActionsPool pool,
            IWindowsStackHolder windowsStackHolder
        ) : base(pool)
        {
            _windowsStackHolder = windowsStackHolder;
        }

        /// <summary>
        /// Configures whether closing should respect back-logic checks.
        /// </summary>
        /// <param name="useBackLogicIgnorableChecks">When true, stops on windows marked as ignorable.</param>
        public void Setup(bool useBackLogicIgnorableChecks)
        {
            _useBackLogicIgnorableChecks = useBackLogicIgnorableChecks;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            _useBackLogicIgnorableChecks = false;
        }

        /// <inheritdoc />
        protected override UniTask HandleStart()
        {
            return _windowsStackHolder.IsEmpty ? UniTask.CompletedTask : CloseAllWindows();
        }

        /// <inheritdoc />
        protected override void ReturnToPool()
        {
            _useBackLogicIgnorableChecks = false;
            Pool.ReturnAction(this);
        }

        private async UniTask CloseAllWindows()
        {
            while (!_windowsStackHolder.IsEmpty)
            {
                var currentWindow = _windowsStackHolder.CurrentWindow;
                if (currentWindow == null)
                    return;

                if (_useBackLogicIgnorableChecks && currentWindow.IsBackLogicIgnorable)
                    return;

                _windowsStackHolder.Pop();
                await currentWindow.SetState(EWindowState.Closed, Pool).Start();
                WindowsOrdersManager.UpdateWindowsLayers(_windowsStackHolder.Stack);
            }
        }
    }
}

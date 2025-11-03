using System.Threading;
using Cysharp.Threading.Tasks;
using KoboldUi.UiAction.Pool;
using KoboldUi.Windows;

namespace KoboldUi.UiAction.Impl.Common
{
    /// <summary>
    /// Waits for a window to finish initialization before completing.
    /// </summary>
    public class WaitInitializationAction : AUiAction
    {
        private CancellationTokenSource _linkedTokenSource;
        private AWindowBase _window;

        public WaitInitializationAction(IUiActionsPool pool) : base(pool)
        {
        }

        /// <summary>
        /// Specifies the window whose initialization should be awaited.
        /// </summary>
        /// <param name="window">Window to monitor.</param>
        public void Setup(AWindowBase window)
        {
            _window = window;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            _linkedTokenSource?.Cancel();
            _linkedTokenSource?.Dispose();
            _linkedTokenSource = null;

            _window = null;
        }

        /// <inheritdoc />
        protected override UniTask HandleStart()
        {
            if (_window.IsInitialized)
                return UniTask.CompletedTask;

            _linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                _window.GetCancellationTokenOnDestroy());

            return UniTask.WaitUntil(
                () => _window.IsInitialized,
                cancellationToken: _linkedTokenSource.Token);
        }

        /// <inheritdoc />
        protected override void ReturnToPool()
        {
            _linkedTokenSource?.Cancel();
            _linkedTokenSource?.Dispose();
            _linkedTokenSource = null;

            _window = null;

            Pool.ReturnAction(this);
        }
    }
}
using System.Threading;
using Cysharp.Threading.Tasks;
using KoboldUi.UiAction.Pool;
using KoboldUi.Windows;

namespace KoboldUi.UiAction.Impl.Common
{
    public class WaitInitializationAction : AUiAction
    {
        private AWindowBase _window;
        private CancellationTokenSource _linkedTokenSource;

        public WaitInitializationAction(IUiActionsPool pool) : base(pool)
        {
        }

        public void Setup(AWindowBase window)
        {
            _window = window;
        }

        public override void Dispose()
        {
            _linkedTokenSource?.Cancel();
            _linkedTokenSource?.Dispose();
            _linkedTokenSource = null;
        
            _window = null;
        }

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

        protected override void ReturnToPool() => Pool.ReturnAction(this);
    }
}
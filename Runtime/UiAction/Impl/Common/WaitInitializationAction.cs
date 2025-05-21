using System.Threading;
using Cysharp.Threading.Tasks;
using KoboldUi.Windows;

namespace KoboldUi.UiAction.Impl.Common
{
    public class WaitInitializationAction : IUiAction
    {
        private AWindowBase _window;
        private CancellationTokenSource _linkedTokenSource;

        public void Setup(AWindowBase window)
        {
            _window = window;
        }

        public UniTask Start()
        {
            if (_window.IsInitialized)
                return UniTask.CompletedTask;
            
            _linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                _window.GetCancellationTokenOnDestroy());
            
            return UniTask.WaitUntil(
                () => _window.IsInitialized,
                cancellationToken: _linkedTokenSource.Token);
        }

        public void Dispose()
        {
            _linkedTokenSource?.Cancel();
            _linkedTokenSource?.Dispose();
            _linkedTokenSource = null;
        
            _window = null;
        }
    }
}
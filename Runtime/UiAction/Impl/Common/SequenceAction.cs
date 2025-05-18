using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace KoboldUi.UiAction.Impl.Common
{
    public class SequenceAction : IUiAction
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private Queue<IUiAction> _actionsQueue;

        public void Setup(Queue<IUiAction> actions)
        {
            _actionsQueue = actions;
        }
        
        public UniTask Start() => RunQueuedActions();

        public void Dispose()
        {
            if (_actionsQueue != null)
            {
                foreach (var uiAction in _actionsQueue)
                    uiAction.Dispose();
            }
            
            _actionsQueue = null;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        
        private async UniTask RunQueuedActions()
        {
            while (_actionsQueue.TryDequeue(out var action)) 
                await action.Start();
            
            _actionsQueue = null;
        }
    }
}
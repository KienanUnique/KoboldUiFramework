using System;
using System.Collections.Concurrent;
using System.Threading;
using Cysharp.Threading.Tasks;
using KoboldUi.UiAction;

namespace KoboldUi.TasksRunner.Impl
{
    public class TaskRunner : ITasksRunner
    {
        private readonly ConcurrentQueue<IUiAction> _actionQueue = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private bool _isRunning;

        public void AddToQueue(IUiAction uiAction)
        {
            _actionQueue.Enqueue(uiAction);
            if (!_isRunning)
            {
                StartProcessing().Forget();
            }
        }

        private async UniTaskVoid StartProcessing()
        {
            if (_isRunning) return;
        
            _isRunning = true;
        
            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested && _actionQueue.TryDequeue(out var action))
                {
                    try
                    {
                        UnityEngine.Debug.Log($"Running action {action.GetType().Name}");
                        await action.Start();
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"[Kobold Ui {nameof(TaskRunner)}] | Error executing UI action: {e}");
                    }
                }
            }
            finally
            {
                _isRunning = false;
            
                if (!_actionQueue.IsEmpty)
                {
                    StartProcessing().Forget();
                }
                else
                {
                    UnityEngine.Debug.Log($"Tasks runner stopped");
                }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            
            while (_actionQueue.TryDequeue(out var action)) 
                action.Dispose();
        }
    }
}
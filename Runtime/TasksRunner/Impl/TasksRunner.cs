using System;
using System.Collections.Concurrent;
using System.Threading;
using Cysharp.Threading.Tasks;
using KoboldUi.UiAction;
using UnityEngine;

namespace KoboldUi.TasksRunner.Impl
{
    /// <summary>
    /// Processes UI actions sequentially on a background task queue.
    /// </summary>
    public class TaskRunner : ITasksRunner
    {
        private readonly ConcurrentQueue<IUiAction> _actionQueue = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private bool _isRunning;

        /// <inheritdoc />
        public void AddToQueue(IUiAction uiAction)
        {
            _actionQueue.Enqueue(uiAction);
            if (!_isRunning)
                StartProcessing().Forget();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            while (_actionQueue.TryDequeue(out var action))
                action.Dispose();
        }

        private async UniTaskVoid StartProcessing()
        {
            if (_isRunning) return;

            _isRunning = true;

            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested && _actionQueue.TryDequeue(out var action))
                    try
                    {
                        await action.Start();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[Kobold Ui {nameof(TaskRunner)}] | Error executing UI action: {e}");
                    }
            }
            finally
            {
                _isRunning = false;

                if (!_actionQueue.IsEmpty)
                    StartProcessing().Forget();
            }
        }
    }
}
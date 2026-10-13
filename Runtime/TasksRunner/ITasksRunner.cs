using System;
using KoboldUi.UiAction;

namespace KoboldUi.TasksRunner
{
    /// <summary>
    /// Executes queued UI actions sequentially.
    /// </summary>
    internal interface ITasksRunner : IDisposable
    {
        /// <summary>
        /// Enqueues an action for execution.
        /// </summary>
        /// <param name="uiAction">Action to run.</param>
        void AddToQueue(IUiAction uiAction);
    }
}

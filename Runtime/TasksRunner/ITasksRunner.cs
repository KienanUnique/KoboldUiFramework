using System;
using KoboldUi.UiAction;

namespace KoboldUi.TasksRunner
{
    public interface ITasksRunner : IDisposable
    {
        void AddToQueue(IUiAction uiAction);
    }
}
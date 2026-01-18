using System;
using KoboldUi.TasksRunner;
using KoboldUi.TasksRunner.Impl;
using KoboldUi.UiAction.Impl.Service;
using KoboldUi.UiAction.Pool;
using KoboldUi.UiAction.Pool.Impl;
using KoboldUi.Windows;
using KoboldUi.WindowsStack;
using KoboldUi.WindowsStack.Impl;
using Zenject;

namespace KoboldUi.Services.WindowsService.Impl
{
    /// <summary>
    /// Base windows service that wires together actions, runners, and the windows stack.
    /// </summary>
    public abstract class AWindowsService : IWindowsService, IDisposable
    {
        private readonly DiContainer _diContainer;
        private readonly ITasksRunner _tasksRunner = new TaskRunner();
        private readonly IUiActionsPool _uiActionsPool;
        private readonly IWindowsStackHolder _windowsStackHolder = new WindowsStackHolder();

        /// <inheritdoc />
        public IWindow CurrentWindow => _windowsStackHolder.CurrentWindow;

        protected AWindowsService(DiContainer diContainer)
        {
            _diContainer = diContainer;
            var uiActionsPool = new UiActionsPool(_windowsStackHolder);
            uiActionsPool.Initialize();
            _uiActionsPool = uiActionsPool;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _tasksRunner?.Dispose();
        }

        /// <inheritdoc />
        public bool IsOpened<TWindow>() where TWindow : IWindow
        {
            return _windowsStackHolder.IsOpened<TWindow>();
        }

        /// <inheritdoc />
        public void OpenWindow<TWindow>(
            Action onComplete = null,
            EPreviousWindowPolicy previousWindowPolicy = EPreviousWindowPolicy.Default
        ) where TWindow : IWindow
        {
            var nextWindow = _diContainer.Resolve(typeof(TWindow)) as IWindow;
            _uiActionsPool.GetAction(out OpenWindowAction openAction, nextWindow, previousWindowPolicy);
            _tasksRunner.AddToQueue(openAction);

            TryAppendCallback(onComplete);
        }

        /// <inheritdoc />
        public void CloseWindow(Action onComplete, bool useBackLogicIgnorableChecks)
        {
            _uiActionsPool.GetAction(out CloseWindowAction tryBackWindowAction, useBackLogicIgnorableChecks);
            _tasksRunner.AddToQueue(tryBackWindowAction);

            TryAppendCallback(onComplete);
        }

        /// <inheritdoc />
        public void CloseToWindow<TWindow>(Action onComplete, bool useBackLogicIgnorableChecks)
        {
            var targetWindow = _diContainer.Resolve(typeof(TWindow)) as IWindow;
            _uiActionsPool.GetAction(out CloseToWindowAction backToWindowAction, targetWindow, useBackLogicIgnorableChecks);

            _tasksRunner.AddToQueue(backToWindowAction);
            TryAppendCallback(onComplete);
        }

        private void TryAppendCallback(Action onComplete)
        {
            if (onComplete == null)
                return;

            _uiActionsPool.GetAction(out var callbackAction, onComplete);
            callbackAction.Setup(onComplete);
            _tasksRunner.AddToQueue(callbackAction);
        }
    }
}

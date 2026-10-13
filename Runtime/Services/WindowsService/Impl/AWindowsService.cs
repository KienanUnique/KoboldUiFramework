using System;
using KoboldUi.TasksRunner;
using KoboldUi.TasksRunner.Impl;
using KoboldUi.UiAction.Impl.Service;
using KoboldUi.UiAction.Pool;
using KoboldUi.UiAction.Pool.Impl;
using KoboldUi.Windows;
using KoboldUi.WindowsStack;
using KoboldUi.WindowsStack.Impl;
using KoboldUi.Composition;

namespace KoboldUi.Services.WindowsService.Impl
{
    /// <summary>
    /// Base windows service that wires together actions, runners, and the windows stack.
    /// </summary>
    public abstract class AWindowsService : IWindowsService, IDisposable
    {
        private readonly WindowRegistry _windowRegistry;
        private readonly ITasksRunner _tasksRunner = new TaskRunner();
        private readonly IUiActionsPool _uiActionsPool;
        private readonly IWindowsStackHolder _windowsStackHolder = new WindowsStackHolder();
        private readonly Func<bool> _ensureReady;
        private readonly Action<Action> _queueAction;

        /// <inheritdoc />
        public IWindow CurrentWindow => _windowsStackHolder.CurrentWindow;

        protected AWindowsService(WindowRegistry windowRegistry, Func<bool> ensureReady = null,
            Action<Action> queueAction = null)
        {
            _windowRegistry = windowRegistry ?? throw new ArgumentNullException(nameof(windowRegistry));
            _ensureReady = ensureReady;
            _queueAction = queueAction;
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
            if (_ensureReady != null && !_ensureReady())
            {
                _queueAction(() => OpenWindow<TWindow>(onComplete, previousWindowPolicy));
                return;
            }
            var nextWindow = _windowRegistry.Get(typeof(TWindow));
            _uiActionsPool.GetAction(out OpenWindowAction openAction, nextWindow, previousWindowPolicy);
            _tasksRunner.AddToQueue(openAction);

            TryAppendCallback(onComplete);
        }

        /// <inheritdoc />
        public void CloseWindow(Action onComplete, bool useBackLogicIgnorableChecks)
        {
            if (_ensureReady != null && !_ensureReady())
            {
                _queueAction(() => CloseWindow(onComplete, useBackLogicIgnorableChecks));
                return;
            }
            _uiActionsPool.GetAction(out CloseWindowAction tryBackWindowAction, useBackLogicIgnorableChecks);
            _tasksRunner.AddToQueue(tryBackWindowAction);

            TryAppendCallback(onComplete);
        }

        /// <inheritdoc />
        public void CloseToWindow<TWindow>(Action onComplete, bool useBackLogicIgnorableChecks)
        {
            if (_ensureReady != null && !_ensureReady())
            {
                _queueAction(() => CloseToWindow<TWindow>(onComplete, useBackLogicIgnorableChecks));
                return;
            }
            var targetWindow = _windowRegistry.Get(typeof(TWindow));
            _uiActionsPool.GetAction(out CloseToWindowAction backToWindowAction, targetWindow, useBackLogicIgnorableChecks);

            _tasksRunner.AddToQueue(backToWindowAction);
            TryAppendCallback(onComplete);
        }

        /// <inheritdoc />
        public void CloseAllWindows(Action onComplete, bool useBackLogicIgnorableChecks)
        {
            if (_ensureReady != null && !_ensureReady())
            {
                _queueAction(() => CloseAllWindows(onComplete, useBackLogicIgnorableChecks));
                return;
            }
            _uiActionsPool.GetAction(out CloseAllWindowsAction closeAllWindowsAction, useBackLogicIgnorableChecks);
            _tasksRunner.AddToQueue(closeAllWindowsAction);

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

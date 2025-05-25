﻿using System;
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
    public abstract class AWindowsService : IWindowsService, IDisposable
    {
        private readonly DiContainer _diContainer;
        private readonly ITasksRunner _tasksRunner = new TaskRunner();
        private readonly IUiActionsPool _uiActionsPool;
        private readonly IWindowsStackHolder _windowsStackHolder = new WindowsStackHolder();

        protected AWindowsService(DiContainer diContainer)
        {
            _diContainer = diContainer;
            var uiActionsPool = new UiActionsPool(_windowsStackHolder);
            uiActionsPool.Initialize();
            _uiActionsPool = uiActionsPool;
        }
        
        // public void CloseWindow(Action onComplete, EAnimationPolitic previousWindowPolitic)
        // {
        //     if (_windowsStack.Count == 0)
        //         throw new Exception("There is no opened windows, so nothing to close");
        //
        //     CloseWindowImpl().Forget();
        //     return;
        //
        //     async UniTaskVoid CloseWindowImpl()
        //     {
        //         var currentWindow = _windowsStack.Pop();
        //
        //         await ChangeWindowState(currentWindow, EWindowState.Closed, previousWindowPolitic);
        //         WindowsOrdersManager.HandleWindowDisappear(_windowsStack, currentWindow);
        //
        //         await OpenPreviousWindow();
        //
        //         onComplete?.Invoke();
        //     }
        // }
        //
        // public void CloseToWindow<TWindow>(Action onComplete, EAnimationPolitic previousWindowsPolitic)
        //     where TWindow : IWindow
        // {
        //     CloseWindowImpl().Forget();
        //     return;
        //
        //     async UniTaskVoid CloseWindowImpl()
        //     {
        //         {
        //             var needWindow = _diContainer.Resolve(typeof(TWindow)) as IWindow;
        //             if (needWindow == null)
        //                 throw new Exception($"Window {typeof(TWindow).Name} was not found");
        //             
        //             if (!_windowsStack.Contains(needWindow))
        //                 throw new Exception(
        //                     $"Window {typeof(TWindow).Name} was not found in stack. It means that window wasn't previously opened");
        //
        //             while (CurrentWindow != needWindow)
        //             {
        //                 var currentWindow = _windowsStack.Peek();
        //
        //                 _windowsStack.Pop();
        //                 await ChangeWindowState(currentWindow, EWindowState.Closed, previousWindowsPolitic);
        //             }
        //
        //             WindowsOrdersManager.UpdateWindowsLayers(_windowsStack);
        //             await OpenPreviousWindow();
        //
        //             onComplete?.Invoke();
        //         }
        //     }
        // }

        public void Dispose()
        {
            _tasksRunner?.Dispose();
        }

        public IWindow CurrentWindow => _windowsStackHolder.CurrentWindow;

        public bool IsOpened<TWindow>() where TWindow : IWindow
        {
            return _windowsStackHolder.IsOpened<TWindow>();
        }

        public void OpenWindow<TWindow>(Action onComplete) where TWindow : IWindow
        {
            var nextWindow = _diContainer.Resolve(typeof(TWindow)) as IWindow;
            _uiActionsPool.GetAction(out OpenWindowAction openAction, nextWindow);
            _tasksRunner.AddToQueue(openAction);

            if (onComplete == null)
                return;

            _uiActionsPool.GetAction(out var callbackAction, onComplete);
            _tasksRunner.AddToQueue(callbackAction);
        }

        public void BackWindow(Action onComplete)
        {
            _uiActionsPool.GetAction(out BackWindowAction tryBackWindowAction);
            _tasksRunner.AddToQueue(tryBackWindowAction);

            if (onComplete == null)
                return;

            _uiActionsPool.GetAction(out var callbackAction, onComplete);
            callbackAction.Setup(onComplete);
            _tasksRunner.AddToQueue(callbackAction);
        }

        public void BackToWindow<TWindow>(Action onComplete = null)
        {
            var targetWindow = _diContainer.Resolve(typeof(TWindow)) as IWindow;
            _uiActionsPool.GetAction(out BackToWindowAction backToWindowAction, targetWindow);
            
            _tasksRunner.AddToQueue(backToWindowAction);

            if (onComplete == null)
                return;

            _uiActionsPool.GetAction(out var callbackAction, onComplete);
            callbackAction.Setup(onComplete);
            _tasksRunner.AddToQueue(callbackAction);
        }
    }
}
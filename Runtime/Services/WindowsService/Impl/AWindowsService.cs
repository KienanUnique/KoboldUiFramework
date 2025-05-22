using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KoboldUi.Interfaces;
using KoboldUi.TasksRunner;
using KoboldUi.TasksRunner.Impl;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Impl.Common;
using KoboldUi.UiAction.Impl.Service;
using KoboldUi.UiAction.Pool;
using KoboldUi.UiAction.Pool.Impl;
using KoboldUi.Utils;
using KoboldUi.Windows;
using KoboldUi.WindowsStack;
using KoboldUi.WindowsStack.Impl;
using Zenject;

namespace KoboldUi.Services.WindowsService.Impl
{
    public abstract class AWindowsService : IWindowsService, IDisposable
    {
        private readonly ITasksRunner _tasksRunner = new TaskRunner();
        private readonly IWindowsStackHolder _windowsStackHolder = new WindowsStackHolder();
        private readonly IUiActionsPool _uiActionsPool;

        private readonly DiContainer _diContainer;

        public IWindow CurrentWindow => _windowsStackHolder.CurrentWindow;

        protected AWindowsService(DiContainer diContainer)
        {
            _diContainer = diContainer;
            _uiActionsPool = new UiActionsPool(_windowsStackHolder);
        }

        public bool IsOpened<TWindow>() where TWindow : IWindow => _windowsStackHolder.IsOpened<TWindow>();

        public void OpenWindow<TWindow>(Action onComplete) where TWindow : IWindow
        {
            var nextWindow = _diContainer.Resolve(typeof(TWindow)) as IWindow;
            _uiActionsPool.GetAction(out OpenWindowAction openAction, nextWindow);
            _tasksRunner.AddToQueue(openAction);

            if (onComplete == null)
                return;
            
            _uiActionsPool.GetAction(out SimpleCallbackAction callbackAction, onComplete);
            _tasksRunner.AddToQueue(callbackAction);
        }

        public void BackWindow(Action onComplete)
        {
            _uiActionsPool.GetAction(out TryBackWindowAction tryBackWindowAction);
            _tasksRunner.AddToQueue(tryBackWindowAction);

            if (onComplete == null) 
                return;
            
            _uiActionsPool.GetAction(out SimpleCallbackAction callbackAction, onComplete);
            callbackAction.Setup(onComplete);
            _tasksRunner.AddToQueue(callbackAction);
        }

        // public void TryBackToWindow<TWindow>(Action<bool> onComplete, EAnimationPolitic previousWindowsPolitic)
        // {
        //     TryBackToWindowImpl().Forget();
        //     return;
        //
        //     async UniTaskVoid TryBackToWindowImpl()
        //     {
        //         var needWindow = _diContainer.Resolve(typeof(TWindow)) as IWindow;
        //         if (needWindow == null)
        //             throw new Exception($"Window {typeof(TWindow).Name} was not found");
        //
        //         if (!_windowsStack.Contains(needWindow))
        //             throw new Exception(
        //                 $"Window {typeof(TWindow).Name} was not found in stack. It means that window wasn't previously opened");
        //
        //         while (CurrentWindow != needWindow)
        //         {
        //             var currentWindow = _windowsStack.Peek();
        //
        //             var windowIgnoreBackSignal = currentWindow is IBackLogicIgnorable;
        //             if (windowIgnoreBackSignal)
        //             {
        //                 onComplete?.Invoke(false);
        //                 return;
        //             }
        //
        //             _windowsStack.Pop();
        //             await ChangeWindowState(currentWindow, EWindowState.Closed, previousWindowsPolitic);
        //         }
        //
        //         WindowsOrdersManager.UpdateWindowsLayers(_windowsStack);
        //         await OpenPreviousWindow();
        //
        //         onComplete?.Invoke(true);
        //     }
        // }
        //
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

        // private async UniTask OpenPreviousWindow()
        // {
        //     if (_windowsStack.Count == 0)
        //         return;
        //
        //     var currentWindow = _windowsStack.Peek();
        //     await currentWindow.SetState(EWindowState.Active);
        // }
        //
        // private static async UniTask ChangeWindowState(IWindow window, EWindowState state)
        // {
        //     var changeStateTask = window.SetState(state);
        //
        //     switch (animationPolitic)
        //     {
        //         case EAnimationPolitic.Wait:
        //             await changeStateTask;
        //             break;
        //         case EAnimationPolitic.DoNotWait:
        //             changeStateTask.Forget();
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException(nameof(animationPolitic), animationPolitic, null);
        //     }
        // }

        public void Dispose()
        {
            _tasksRunner?.Dispose();
        }
    }
}
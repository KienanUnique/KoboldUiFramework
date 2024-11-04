using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KoboldUi.Interfaces;
using KoboldUi.Utils;
using KoboldUi.Windows;
using UniRx;
using Zenject;

namespace KoboldUi.Services.WindowsService.Impl
{
    public abstract class AWindowsService : IWindowsService, IDisposable
    {
        private readonly Stack<IWindow> _windowsStack = new();

        private readonly DiContainer _diContainer;

        public IWindow CurrentWindow => _windowsStack.Peek();

        protected AWindowsService(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public void Dispose()
        {
        }

        public void OpenWindow<TWindow>(Action onComplete) where TWindow : IWindow
        {
            OpenWindowImpl().Forget();
            return;

            async UniTaskVoid OpenWindowImpl()
            {
                var nextWindow = _diContainer.Resolve(typeof(TWindow)) as IWindow;

                var isNextWindowPopUp = nextWindow is IPopUp;
                if (_windowsStack.Count > 0)
                {
                    var currentWindow = _windowsStack.Peek();
                    await currentWindow.SetState(isNextWindowPopUp ? EWindowState.NonFocused : EWindowState.Closed);
                }

                if (!nextWindow!.IsInitialized)
                {
                    await nextWindow.WaitInitialization();
                }

                WindowsOrdersManager.HandleWindowAppear(_windowsStack, nextWindow);

                _windowsStack.Push(nextWindow);
                await nextWindow.SetState(EWindowState.Active);

                onComplete?.Invoke();
            }
        }

        public void TryBackWindow(Action<bool> onComplete)
        {
            TryBackWindowImpl().Forget();
            return;

            async UniTaskVoid TryBackWindowImpl()
            {
                if (_windowsStack.Count == 0)
                {
                    onComplete?.Invoke(false);
                    return;
                }

                var currentWindow = _windowsStack.Pop();

                var windowIgnoreBackSignal = currentWindow is IBackLogicIgnorable;
                if (windowIgnoreBackSignal)
                {
                    onComplete?.Invoke(false);
                    return;
                }

                await currentWindow.SetState(EWindowState.Closed);
                WindowsOrdersManager.HandleWindowDisappear(_windowsStack, currentWindow);
                await OpenPreviousWindow();

                onComplete?.Invoke(false);
            }
        }

        public void TryBackToWindow<TWindow>(Action<bool> onComplete)
        {
            TryBackToWindowImpl().Forget();
            return;

            async UniTaskVoid TryBackToWindowImpl()
            {
                var needWindow = _diContainer.Resolve(typeof(TWindow)) as IWindow;
                if (needWindow == null)
                    throw new Exception($"Window {typeof(TWindow).Name} was not found");

                while (CurrentWindow != needWindow)
                {
                    var currentWindow = _windowsStack.Peek();

                    var windowIgnoreBackSignal = currentWindow is IBackLogicIgnorable;
                    if (windowIgnoreBackSignal)
                    {
                        onComplete?.Invoke(false);
                        return;
                    }

                    _windowsStack.Pop();
                    await currentWindow.SetState(EWindowState.Closed);
                }

                WindowsOrdersManager.UpdateWindowsLayers(_windowsStack);
                await OpenPreviousWindow();

                onComplete?.Invoke(true);
            }
        }

        public void TryBackWindows(int countOfWindowsToClose, Action<bool> onComplete)
        {
            TryBackWindowsImpl().Forget();
            return;

            async UniTaskVoid TryBackWindowsImpl()
            {
                if (_windowsStack.Count < countOfWindowsToClose)
                    throw new Exception(
                        $"Can not close {countOfWindowsToClose} windows because of only {_windowsStack.Count} opened");

                for (var i = 0; i < countOfWindowsToClose; i++)
                {
                    var currentWindow = _windowsStack.Peek();

                    var windowIgnoreBackSignal = currentWindow is IBackLogicIgnorable;
                    if (windowIgnoreBackSignal)
                    {
                        onComplete?.Invoke(false);
                        return;
                    }

                    _windowsStack.Pop();
                    await currentWindow.SetState(EWindowState.Closed);
                }

                WindowsOrdersManager.UpdateWindowsLayers(_windowsStack);
                await OpenPreviousWindow();

                onComplete?.Invoke(true);
            }
        }

        public void CloseWindow(Action onComplete)
        {
            if (_windowsStack.Count == 0)
                throw new Exception("There is no opened windows, so nothing to close");

            CloseWindowImpl().Forget();
            return;

            async UniTaskVoid CloseWindowImpl()
            {
                var currentWindow = _windowsStack.Pop();
                await currentWindow.SetState(EWindowState.Closed);
                WindowsOrdersManager.HandleWindowDisappear(_windowsStack, currentWindow);

                await OpenPreviousWindow();
                onComplete?.Invoke();
            }
        }

        public void CloseWindows(int countOfWindowsToClose, Action onComplete)
        {
            CloseWindowImpl().Forget();
            return;

            async UniTaskVoid CloseWindowImpl()
            {
                if (_windowsStack.Count < countOfWindowsToClose)
                    throw new Exception(
                        $"Can not close {countOfWindowsToClose} windows because of only {_windowsStack.Count} opened");

                for (var i = 0; i < countOfWindowsToClose; i++)
                {
                    var currentWindow = _windowsStack.Pop();
                    await currentWindow.SetState(EWindowState.Closed);
                }

                WindowsOrdersManager.UpdateWindowsLayers(_windowsStack);
                await OpenPreviousWindow();

                onComplete?.Invoke();
            }
        }

        public void CloseToWindow<TWindow>(Action onComplete)
        {
            CloseWindowImpl().Forget();
            return;

            async UniTaskVoid CloseWindowImpl()
            {
                {
                    var needWindow = _diContainer.Resolve(typeof(TWindow)) as IWindow;
                    if (needWindow == null)
                        throw new Exception($"Window {typeof(TWindow).Name} was not found");

                    while (CurrentWindow != needWindow)
                    {
                        var currentWindow = _windowsStack.Peek();

                        _windowsStack.Pop();
                        await currentWindow.SetState(EWindowState.Closed);
                    }

                    WindowsOrdersManager.UpdateWindowsLayers(_windowsStack);
                    await OpenPreviousWindow();
                    
                    onComplete?.Invoke();
                }
            }
        }

        private async UniTask OpenPreviousWindow()
        {
            if (_windowsStack.Count == 0)
                return;

            var currentWindow = _windowsStack.Peek();
            await currentWindow.SetState(EWindowState.Active);
        }
    }
}
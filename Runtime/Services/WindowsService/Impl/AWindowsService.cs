using System;
using System.Collections.Generic;
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

        private IDisposable _waitInitializationDisposable;

        public IWindow CurrentWindow => _windowsStack.Peek();

        protected AWindowsService(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public void Dispose()
        {
            _waitInitializationDisposable?.Dispose();
        }

        public void OpenWindow<TWindow>() where TWindow : IWindow
        {
            var nextWindow = _diContainer.Resolve(typeof(TWindow)) as IWindow;

            var isNextWindowPopUp = nextWindow is IPopUp;
            if (_windowsStack.Count > 0)
            {
                var currentWindow = _windowsStack.Peek();
                currentWindow.SetState(isNextWindowPopUp ? EWindowState.NonFocused : EWindowState.Closed);
            }

            if (!nextWindow!.IsInitialized.Value)
            {
                _waitInitializationDisposable?.Dispose();
                _waitInitializationDisposable = nextWindow.IsInitialized.Subscribe(isInitilized =>
                {
                    if (!isInitilized)
                        return;

                    OpenNewWindow(nextWindow);

                    _waitInitializationDisposable?.Dispose();
                });
                return;
            }

            OpenNewWindow(nextWindow);

            return;

            void OpenNewWindow(IWindow window)
            {
                WindowsOrdersManager.HandleWindowAppear(_windowsStack, window);

                _windowsStack.Push(window);
                window.SetState(EWindowState.Active);
            }
        }

        public bool TryBackWindow()
        {
            if (_windowsStack.Count == 0)
                return false;

            var currentWindow = _windowsStack.Pop();

            var windowIgnoreBackSignal = currentWindow is IBackLogicIgnorable;
            if (windowIgnoreBackSignal)
                return false;

            currentWindow.SetState(EWindowState.Closed);
            WindowsOrdersManager.HandleWindowDisappear(_windowsStack, currentWindow);
            OpenPreviousWindow();
            
            return true;
        }

        public bool TryBackToWindow<TWindow>()
        {
            var needWindow = _diContainer.Resolve(typeof(TWindow)) as IWindow;
            if(needWindow == null)
                throw new Exception($"Window {typeof(TWindow).Name} was not found");
            
            while (CurrentWindow != needWindow)
            {
                var currentWindow = _windowsStack.Peek();

                var windowIgnoreBackSignal = currentWindow is IBackLogicIgnorable;
                if (windowIgnoreBackSignal)
                    return false;

                _windowsStack.Pop();
                currentWindow.SetState(EWindowState.Closed);
            }

            WindowsOrdersManager.UpdateWindowsLayers(_windowsStack);
            OpenPreviousWindow();
            
            return true;
        }

        public bool TryBackWindows(int countOfWindowsToClose)
        {
            if (_windowsStack.Count < countOfWindowsToClose)
                throw new Exception(
                    $"Can not close {countOfWindowsToClose} windows because of only {_windowsStack.Count} opened");
            
            for (var i = 0; i < countOfWindowsToClose; i++)
            {
                var currentWindow = _windowsStack.Peek();

                var windowIgnoreBackSignal = currentWindow is IBackLogicIgnorable;
                if (windowIgnoreBackSignal)
                    return false;

                _windowsStack.Pop();
                currentWindow.SetState(EWindowState.Closed);
            }

            WindowsOrdersManager.UpdateWindowsLayers(_windowsStack);
            OpenPreviousWindow();
            
            return true;
        }

        public void CloseWindow()
        {
            if (_windowsStack.Count == 0)
                return;

            var currentWindow = _windowsStack.Pop();
            currentWindow.SetState(EWindowState.Closed);
            WindowsOrdersManager.HandleWindowDisappear(_windowsStack, currentWindow);
            
            OpenPreviousWindow();
        }

        public void CloseWindows(int countOfWindowsToClose)
        {
            if (_windowsStack.Count < countOfWindowsToClose)
                throw new Exception(
                    $"Can not close {countOfWindowsToClose} windows because of only {_windowsStack.Count} opened");
            
            for (var i = 0; i < countOfWindowsToClose; i++)
            {
                var currentWindow = _windowsStack.Pop();
                currentWindow.SetState(EWindowState.Closed);
            }
            
            WindowsOrdersManager.UpdateWindowsLayers(_windowsStack);
            OpenPreviousWindow();
        }
        
        public void CloseToWindow<TWindow>()
        {
            var needWindow = _diContainer.Resolve(typeof(TWindow)) as IWindow;
            if(needWindow == null)
                throw new Exception($"Window {typeof(TWindow).Name} was not found");
            
            while (CurrentWindow != needWindow)
            {
                var currentWindow = _windowsStack.Peek();

                _windowsStack.Pop();
                currentWindow.SetState(EWindowState.Closed);
            }

            WindowsOrdersManager.UpdateWindowsLayers(_windowsStack);
            OpenPreviousWindow();
        }

        private void OpenPreviousWindow()
        {
            if (_windowsStack.Count == 0)
                return;

            var currentWindow = _windowsStack.Peek();
            currentWindow.SetState(EWindowState.Active);
        }
    }
}
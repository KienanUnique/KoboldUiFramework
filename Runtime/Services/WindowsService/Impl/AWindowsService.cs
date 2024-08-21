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

            if (!nextWindow.IsInitialized.Value)
            {
                _waitInitializationDisposable?.Dispose();
                _waitInitializationDisposable = nextWindow.IsInitialized.Subscribe(isInitilized =>
                {
                    if (!isInitilized)
                        return;

                    ShowWindow(nextWindow);

                    _waitInitializationDisposable?.Dispose();
                });
                return;
            }

            ShowWindow(nextWindow);
        }

        public void BackWindow()
        {
            if (_windowsStack.Count == 0)
                return;

            var currentWindow = _windowsStack.Pop();

            var windowIgnoreBackSignal = currentWindow is IBackLogicIgnorable;
            if (windowIgnoreBackSignal)
                return;

            currentWindow.SetState(EWindowState.Closed);
            OpenPreviousWindow();
        }

        public void ForceCloseWindow()
        {
            if (_windowsStack.Count == 0)
                return;

            var currentWindow = _windowsStack.Pop();
            currentWindow.SetState(EWindowState.Closed);
            OpenPreviousWindow();
        }

        private void ShowWindow(IWindow window)
        {
            _windowsStack.Push(window);
            window.SetState(EWindowState.Active);
            window.SetAsLastSibling();
        }

        private void OpenPreviousWindow()
        {
            if (_windowsStack.Count == 0)
                return;

            var currentWindow = _windowsStack.Peek();
            currentWindow.SetState(EWindowState.Active);

            currentWindow.SetAsTheSecondLastSibling();
        }
    }
}
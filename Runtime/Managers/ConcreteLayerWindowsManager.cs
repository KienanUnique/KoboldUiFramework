using System;
using System.Collections.Generic;
using KoboldUi.Interfaces;
using KoboldUi.Signals;
using KoboldUi.Utils;
using KoboldUi.Windows;
using UniRx;
using Zenject;

namespace KoboldUi.Managers
{   
    public class ConcreteLayerWindowsManager : IDisposable
    {
        private readonly Stack<IWindow> _windowsStack = new();
        private readonly CompositeDisposable _disposables = new();

        private readonly DiContainer _diContainer;
        private readonly SignalBus _signalBus;
        private readonly EWindowLayer _windowLayer;

        private IDisposable _waitInitializationDisposable;

        public ConcreteLayerWindowsManager(
            DiContainer diContainer,
            SignalBus signalBus,
            EWindowLayer windowLayer
        )
        {
            _diContainer = diContainer;
            _signalBus = signalBus;
            _windowLayer = windowLayer;
            
            _signalBus.GetStreamId<SignalOpenWindow>(_windowLayer).Subscribe(OnSignalOpenWindow).AddTo(_disposables);
            _signalBus.GetStreamId<SignalBackWindow>(_windowLayer).Subscribe(OnSignalBackWindow).AddTo(_disposables);
            _signalBus.GetStreamId<SignalCloseWindow>(_windowLayer).Subscribe(OnSignalCloseWindow).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _waitInitializationDisposable?.Dispose();
        }

        private void OnSignalOpenWindow(SignalOpenWindow signal)
        {
            var nextWindow = _diContainer.Resolve(signal.WindowType) as IWindow;

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
                    if(!isInitilized)
                        return;
                    
                    ShowWindow(nextWindow);
                    
                    _waitInitializationDisposable?.Dispose();
                });
                return;
            }
            
            ShowWindow(nextWindow);
        }

        private void ShowWindow(IWindow window)
        {
            _windowsStack.Push(window);
            window.SetState(EWindowState.Active);
            window.SetAsLastSibling();
        }

        private void OnSignalBackWindow(SignalBackWindow signalBackWindow)
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

        private void OnSignalCloseWindow(SignalCloseWindow signalCloseWindow)
        {
            if (_windowsStack.Count == 0)
                return;

            var currentWindow = _windowsStack.Pop();
            currentWindow.SetState(EWindowState.Closed);
            OpenPreviousWindow();
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
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KoboldUi.Interfaces;
using KoboldUi.UiAction.Impl.Common;
using KoboldUi.Utils;
using KoboldUi.Windows;
using KoboldUi.WindowsStack;

namespace KoboldUi.UiAction.Impl.Service
{
    public class OpenWindowAction : IUiAction
    {
        private IWindow _windowToOpen;
        private IWindowsStackHolder _windowsStackHolder;

        public void Setup(IWindow windowToOpen, IWindowsStackHolder windowsStackHolder)
        {
            _windowToOpen = windowToOpen;
            _windowsStackHolder = windowsStackHolder;
        }

        public UniTask Start()
        {
            return _windowsStackHolder.IsOpened(_windowToOpen) ? UniTask.CompletedTask : OpenWindow();
        }

        public void Dispose()
        {
            _windowToOpen = null;
        }

        private UniTask OpenWindow()
        {
            var actionsQueue = new Queue<IUiAction>();
            var isNextWindowPopUp = _windowToOpen is IPopUp;
            if (!_windowsStackHolder.IsEmpty)
            {
                var currentWindow = _windowsStackHolder.CurrentWindow;
                var newState = isNextWindowPopUp ? EWindowState.NonFocused : EWindowState.Closed;
                actionsQueue.Enqueue(currentWindow.SetState(newState));
            }

            if (!_windowToOpen.IsInitialized)
                actionsQueue.Enqueue(_windowToOpen.WaitInitialization());

            var callbackAction = new SimpleCallbackAction();
            callbackAction.Setup(() =>
            {
                WindowsOrdersManager.HandleWindowAppear(_windowsStackHolder.Stack, _windowToOpen);
                _windowsStackHolder.Push(_windowToOpen);
            });
            actionsQueue.Enqueue(callbackAction);

            actionsQueue.Enqueue(_windowToOpen.SetState(EWindowState.Active));

            var sequenceAction = new SequenceAction();
            sequenceAction.Setup(actionsQueue);

            return sequenceAction.Start();
        }
    }
}
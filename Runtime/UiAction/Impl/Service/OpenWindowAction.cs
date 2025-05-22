﻿using Cysharp.Threading.Tasks;
using KoboldUi.Interfaces;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using KoboldUi.Windows;
using KoboldUi.WindowsStack;

namespace KoboldUi.UiAction.Impl.Service
{
    public class OpenWindowAction : AUiAction
    {
        private readonly IWindowsStackHolder _windowsStackHolder;
        
        private IWindow _windowToOpen;

        public OpenWindowAction(
            IUiActionsPool pool, 
            IWindowsStackHolder windowsStackHolder
        ) : base(pool)
        {
            _windowsStackHolder = windowsStackHolder;
        }

        public void Setup(IWindow windowToOpen)
        {
            _windowToOpen = windowToOpen;
        }

        public override void Dispose()
        {
            _windowToOpen = null;
        }

        protected override UniTask HandleStart()
        {
            return _windowsStackHolder.IsOpened(_windowToOpen) ? UniTask.CompletedTask : OpenWindow();
        }
        
        protected override void ReturnToPool() => Pool.ReturnAction(this);

        private async UniTask OpenWindow()
        {
            var isNextWindowPopUp = _windowToOpen is IPopUp;
            if (!_windowsStackHolder.IsEmpty)
            {
                var currentWindow = _windowsStackHolder.CurrentWindow;
                var newState = isNextWindowPopUp ? EWindowState.NonFocused : EWindowState.Closed;
                await currentWindow.SetState(newState).Start();
            }

            if (!_windowToOpen.IsInitialized)
                await _windowToOpen.WaitInitialization().Start();

            WindowsOrdersManager.HandleWindowAppear(_windowsStackHolder.Stack, _windowToOpen);
            _windowsStackHolder.Push(_windowToOpen);

            await _windowToOpen.SetState(EWindowState.Active).Start();
        }
    }
}
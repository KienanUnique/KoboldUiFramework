﻿using Cysharp.Threading.Tasks;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using KoboldUi.Windows;
using KoboldUi.WindowsStack;

namespace KoboldUi.UiAction.Impl.Service
{
    public class BackToWindowAction : AUiAction
    {
        private readonly IWindowsStackHolder _windowsStackHolder;

        private IWindow _targetWindow;

        public BackToWindowAction(
            IUiActionsPool pool,
            IWindowsStackHolder windowsStackHolder
        ) : base(pool)
        {
            _windowsStackHolder = windowsStackHolder;
        }

        public void Setup(IWindow targetWindow)
        {
            _targetWindow = targetWindow;
        }

        public override void Dispose()
        {
            _targetWindow = null;
        }

        protected override UniTask HandleStart()
        {
            if (_targetWindow == null || _windowsStackHolder.IsEmpty ||
                !_windowsStackHolder.Contains(_targetWindow))
                return UniTask.CompletedTask;

            var currentWindow = _windowsStackHolder.CurrentWindow;

            var isWindowIgnoreBackSignal = currentWindow.IsBackLogicIgnorable;
            if (isWindowIgnoreBackSignal)
                return UniTask.CompletedTask;

            return BackToWindow(_targetWindow);
        }

        protected override void ReturnToPool()
        {
            _targetWindow = null;
            Pool.ReturnAction(this);
        }

        private async UniTask BackToWindow(IWindow targetWindow)
        {
            var currentWindow = _windowsStackHolder.CurrentWindow;
            while (currentWindow != targetWindow)
            {
                if (currentWindow.IsBackLogicIgnorable)
                    return;

                _windowsStackHolder.Pop();
                await currentWindow.SetState(EWindowState.Closed, Pool).Start();
                WindowsOrdersManager.UpdateWindowsLayers(_windowsStackHolder.Stack);
                
                currentWindow = _windowsStackHolder.CurrentWindow;
            }
            
            Pool.GetAction(out OpenPreviousWindowAction openPreviousWindow);

            await openPreviousWindow.Start();
        }
    }
}
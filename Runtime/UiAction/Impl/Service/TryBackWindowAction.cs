using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KoboldUi.Interfaces;
using KoboldUi.UiAction.Impl.Common;
using KoboldUi.Utils;
using KoboldUi.WindowsStack;

namespace KoboldUi.UiAction.Impl.Service
{
    public class TryBackWindowAction: IUiAction
    {
        private IWindowsStackHolder _windowsStackHolder;

        public void Setup(IWindowsStackHolder windowsStackHolder)
        {
            _windowsStackHolder = windowsStackHolder;
        }
        
        public UniTask Start()
        {
            if (_windowsStackHolder.IsEmpty)
                return UniTask.CompletedTask;
            
            var actionsQueue = new Queue<IUiAction>();
            
            var currentWindow = _windowsStackHolder.Pop();
            
            var isWindowIgnoreBackSignal = currentWindow is IBackLogicIgnorable;
            if (isWindowIgnoreBackSignal)
                return UniTask.CompletedTask;

            actionsQueue.Enqueue(currentWindow.SetState(EWindowState.Closed));

            WindowsOrdersManager.HandleWindowDisappear(_windowsStackHolder.Stack, currentWindow);
            var openPreviousWindow = new OpenPreviousWindow();
            openPreviousWindow.Setup(_windowsStackHolder);

            actionsQueue.Enqueue(openPreviousWindow);
            
            var sequentialAction = new SequenceAction();
            sequentialAction.Setup(actionsQueue);
            
            return sequentialAction.Start();
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }
    }
}
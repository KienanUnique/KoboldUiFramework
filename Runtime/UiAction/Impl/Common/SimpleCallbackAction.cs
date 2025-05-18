using System;
using Cysharp.Threading.Tasks;

namespace KoboldUi.UiAction.Impl.Common
{
    public class SimpleCallbackAction : IUiAction
    {
        private Action _callback;

        public void Setup(Action callback)
        {
            _callback = callback;
        }

        public UniTask Start()
        {
            _callback.Invoke();
            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            _callback = null;
        }
    }
}
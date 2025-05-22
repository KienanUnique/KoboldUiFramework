using System;
using Cysharp.Threading.Tasks;
using KoboldUi.UiAction.Pool;

namespace KoboldUi.UiAction.Impl.Common
{
    public class SimpleCallbackAction : AUiAction
    {
        private Action _callback;

        public SimpleCallbackAction(IUiActionsPool pool) : base(pool)
        {
        }

        public void Setup(Action callback)
        {
            _callback = callback;
        }
        
        protected override void ReturnToPool() => Pool.ReturnAction(this);

        protected override UniTask HandleStart()
        {
            _callback.Invoke();
            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            _callback = null;
        }
    }
}
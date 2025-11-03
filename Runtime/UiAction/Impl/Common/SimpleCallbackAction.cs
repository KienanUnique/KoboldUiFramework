using System;
using Cysharp.Threading.Tasks;
using KoboldUi.UiAction.Pool;

namespace KoboldUi.UiAction.Impl.Common
{
    /// <summary>
    /// Executes a synchronous callback as a UI action.
    /// </summary>
    public class SimpleCallbackAction : AUiAction
    {
        private Action _callback;

        public SimpleCallbackAction(IUiActionsPool pool) : base(pool)
        {
        }

        /// <summary>
        /// Assigns the callback to invoke when the action starts.
        /// </summary>
        /// <param name="callback">Callback to execute.</param>
        public void Setup(Action callback)
        {
            _callback = callback;
        }

        /// <inheritdoc />
        protected override void ReturnToPool()
        {
            _callback = null;
            Pool.ReturnAction(this);
        }

        /// <inheritdoc />
        protected override UniTask HandleStart()
        {
            _callback.Invoke();
            return UniTask.CompletedTask;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            _callback = null;
        }
    }
}
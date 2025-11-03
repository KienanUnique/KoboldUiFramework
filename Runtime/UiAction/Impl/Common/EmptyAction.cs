using Cysharp.Threading.Tasks;
using KoboldUi.UiAction.Pool;

namespace KoboldUi.UiAction.Impl.Common
{
    /// <summary>
    /// No-op action used when no transition is required.
    /// </summary>
    public class EmptyAction : AUiAction
    {
        public EmptyAction(IUiActionsPool pool) : base(pool)
        {
        }

        /// <inheritdoc />
        protected override UniTask HandleStart()
        {
            return UniTask.CompletedTask;
        }

        /// <inheritdoc />
        protected override void ReturnToPool()
        {
            Pool.ReturnAction(this);
        }
    }
}
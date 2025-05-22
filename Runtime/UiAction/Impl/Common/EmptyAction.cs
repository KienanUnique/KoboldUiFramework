using Cysharp.Threading.Tasks;
using KoboldUi.UiAction.Pool;

namespace KoboldUi.UiAction.Impl.Common
{
    public class EmptyAction : AUiAction
    {
        public EmptyAction(IUiActionsPool pool) : base(pool)
        {
        }

        protected override UniTask HandleStart() => UniTask.CompletedTask;
        protected override void ReturnToPool() => Pool.ReturnAction(this);
    }
}
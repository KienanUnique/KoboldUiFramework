using Cysharp.Threading.Tasks;
using KoboldUi.UiAction.Pool;

namespace KoboldUi.UiAction.Impl
{
    public abstract class AUiAction : IUiAction
    {
        protected readonly IUiActionsPool Pool;

        protected AUiAction(IUiActionsPool pool)
        {
            Pool = pool;
        }

        public async UniTask Start()
        {
            await HandleStart();
            ReturnToPool();
        }

        public virtual void Dispose()
        {
        }

        protected abstract UniTask HandleStart();

        protected abstract void ReturnToPool();
    }
}
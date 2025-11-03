using Cysharp.Threading.Tasks;
using KoboldUi.UiAction.Pool;

namespace KoboldUi.UiAction.Impl
{
    /// <summary>
    /// Base class for pooled UI actions that encapsulate asynchronous operations.
    /// </summary>
    public abstract class AUiAction : IUiAction
    {
        /// <summary>
        /// Pool managing this action instance.
        /// </summary>
        protected readonly IUiActionsPool Pool;

        protected AUiAction(IUiActionsPool pool)
        {
            Pool = pool;
        }

        /// <inheritdoc />
        public async UniTask Start()
        {
            await HandleStart();
            ReturnToPool();
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Performs the specific work for the action.
        /// </summary>
        protected abstract UniTask HandleStart();

        /// <summary>
        /// Releases the action back to the pool once finished.
        /// </summary>
        protected abstract void ReturnToPool();
    }
}
using System;
using Cysharp.Threading.Tasks;
using KoboldUi.Element.Animations;
using KoboldUi.UiAction.Pool;

namespace KoboldUi.UiAction.Impl.Common
{
    /// <summary>
    /// Starts one async animation and optionally waits for its completion.
    /// </summary>
    public sealed class AsyncAnimationAction : IUiAction
    {
        private readonly IUiActionsPool _pool;
        private AUiAsyncAnimation _animation;
        private UiAnimationTransition _transition;
        private bool _needWaitAnimation;
        private bool _started;
        private bool _returned;

        public AsyncAnimationAction(IUiActionsPool pool)
        {
            _pool = pool;
        }

        public void Setup(AUiAsyncAnimation animation, UiAnimationTransition transition, bool needWaitAnimation)
        {
            _animation = animation ?? throw new ArgumentNullException(nameof(animation));
            _transition = transition;
            _needWaitAnimation = needWaitAnimation;
            _started = false;
            _returned = false;
        }

        /// <inheritdoc />
        public async UniTask Start()
        {
            if (_returned || _started)
                throw new InvalidOperationException("Animation action has already been started or returned.");

            _started = true;
            try
            {
                var animationTask = _animation.RunTransitionAsync(_transition);
                if (_needWaitAnimation)
                    await animationTask;
                else
                    animationTask.Forget();
            }
            finally
            {
                ReturnToPool();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_started)
                ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (_returned)
                return;

            _returned = true;
            _animation = null;
            _needWaitAnimation = false;
            _pool.ReturnAction(this);
        }
    }
}

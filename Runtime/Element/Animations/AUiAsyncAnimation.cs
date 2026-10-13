using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Impl.Common;
using KoboldUi.UiAction.Pool;
using UnityEngine;

namespace KoboldUi.Element.Animations
{
    /// <summary>
    /// Lifecycle transition handled by an asynchronous UI animation.
    /// </summary>
    public enum UiAnimationTransition
    {
        Appear,
        Disappear,
        FocusReturn,
        FocusRemoved
    }

    /// <summary>
    /// Base component for custom animations expressed as cancellable UniTasks.
    /// </summary>
    public abstract class AUiAsyncAnimation : AUiAnimationBase
    {
        /// <summary>
        /// Whether navigation waits for the animation to complete. The animation plays in either mode.
        /// </summary>
        [SerializeField] public bool _needWaitAnimation;

        private CancellationTokenSource _currentTokenSource;
        private int _currentRunId;

        /// <inheritdoc />
        public sealed override IUiAction Appear(in IUiActionsPool pool)
        {
            return GetAction(pool, UiAnimationTransition.Appear);
        }

        /// <inheritdoc />
        public sealed override IUiAction Disappear(in IUiActionsPool pool)
        {
            return GetAction(pool, UiAnimationTransition.Disappear);
        }

        /// <inheritdoc />
        public sealed override IUiAction AnimateFocusReturn(in IUiActionsPool pool)
        {
            return GetAction(pool, UiAnimationTransition.FocusReturn);
        }

        /// <inheritdoc />
        public sealed override IUiAction AnimateFocusRemoved(in IUiActionsPool pool)
        {
            return GetAction(pool, UiAnimationTransition.FocusRemoved);
        }

        /// <inheritdoc />
        public sealed override void DisappearInstantly()
        {
            CancelCurrentTransition();
            gameObject.SetActive(false);
        }

        /// <summary>Prepares the object immediately before the appearance transition starts.</summary>
        protected virtual void PrepareToAppear()
        {
        }

        /// <summary>Runs the appearance animation. Every awaited operation must observe the token.</summary>
        protected abstract UniTask AnimateAppearAsync(CancellationToken token);

        /// <summary>Runs the disappearance animation. Every awaited operation must observe the token.</summary>
        protected abstract UniTask AnimateDisappearAsync(CancellationToken token);

        /// <summary>Runs the focus-return animation when invoked by a view.</summary>
        protected virtual UniTask AnimateFocusReturnAsync(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        /// <summary>Runs the focus-removal animation when invoked by a view.</summary>
        protected virtual UniTask AnimateFocusRemovedAsync(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        internal async UniTask RunTransitionAsync(UiAnimationTransition transition)
        {
            CancelCurrentTransition();
            var runId = _currentRunId;
            var tokenSource = new CancellationTokenSource();
            _currentTokenSource = tokenSource;

            try
            {
                switch (transition)
                {
                    case UiAnimationTransition.Appear:
                        gameObject.SetActive(true);
                        PrepareToAppear();
                        await AnimateAppearAsync(tokenSource.Token);
                        break;
                    case UiAnimationTransition.Disappear:
                        await AnimateDisappearAsync(tokenSource.Token);
                        if (runId == _currentRunId && !tokenSource.IsCancellationRequested)
                            gameObject.SetActive(false);
                        break;
                    case UiAnimationTransition.FocusReturn:
                        await AnimateFocusReturnAsync(tokenSource.Token);
                        break;
                    case UiAnimationTransition.FocusRemoved:
                        await AnimateFocusRemovedAsync(tokenSource.Token);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(transition), transition, null);
                }
            }
            catch (OperationCanceledException) when (tokenSource.IsCancellationRequested)
            {
                // A replaced or instantly closed transition has ended normally.
            }
            finally
            {
                if (ReferenceEquals(_currentTokenSource, tokenSource))
                    _currentTokenSource = null;
                tokenSource.Dispose();
            }
        }

        private IUiAction GetAction(in IUiActionsPool pool, UiAnimationTransition transition)
        {
            pool.GetAction(out AsyncAnimationAction action, this, transition, _needWaitAnimation);
            return action;
        }

        private void CancelCurrentTransition()
        {
            _currentRunId++;
            _currentTokenSource?.Cancel();
        }

        private void OnDestroy()
        {
            CancelCurrentTransition();
        }
    }
}

using DG.Tweening;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Impl.Common;
using KoboldUi.UiAction.Pool;
using UnityEngine;

namespace KoboldUi.Element.Animations
{
    /// <summary>Base component for custom DOTween animations without parameter assets.</summary>
    public abstract class AUiTweenAnimation : AUiAnimationBase
    {
        /// <summary>Whether navigation waits for the tween. The tween plays in either mode.</summary>
        [SerializeField] public bool _needWaitAnimation;

        /// <inheritdoc />
        public override IUiAction Appear(in IUiActionsPool pool)
        {
            PrepareToAppear();
            gameObject.SetActive(true);
            var tween = AnimateAppear();
            return SelectCorrectUiAction(pool, tween, _needWaitAnimation);
        }

        /// <inheritdoc />
        public override IUiAction Disappear(in IUiActionsPool pool)
        {
            var tween = AnimateDisappear();
            tween.OnComplete(DisappearInstantly);
            return SelectCorrectUiAction(pool, tween, _needWaitAnimation);
        }

        /// <inheritdoc />
        public override IUiAction AnimateFocusReturn(in IUiActionsPool pool)
        {
            pool.GetAction(out EmptyAction emptyAction);
            return emptyAction;
        }

        /// <inheritdoc />
        public override IUiAction AnimateFocusRemoved(in IUiActionsPool pool)
        {
            pool.GetAction(out EmptyAction emptyAction);
            return emptyAction;
        }

        /// <inheritdoc />
        public override void DisappearInstantly()
        {
            gameObject.SetActive(false);
        }

        /// <summary>Performs setup before appearance starts.</summary>
        protected virtual void PrepareToAppear()
        {
        }

        /// <summary>Builds the appearance tween.</summary>
        protected abstract Tween AnimateAppear();

        /// <summary>Builds the disappearance tween.</summary>
        protected abstract Tween AnimateDisappear();

        private static IUiAction SelectCorrectUiAction(in IUiActionsPool pool, Tween tween,
            bool needWaitAnimation)
        {
            if (needWaitAnimation)
            {
                pool.GetAction(out TweenAction tweenAction, tween);
                return tweenAction;
            }

            pool.GetAction(out EmptyAction emptyAction);
            return emptyAction;
        }
    }
}

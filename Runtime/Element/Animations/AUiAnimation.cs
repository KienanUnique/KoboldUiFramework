using System;
using DG.Tweening;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Impl.Common;
using KoboldUi.UiAction.Pool;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
#if KOBOLD_ALCHEMY_SUPPORT
using Alchemy.Inspector;
#endif

namespace KoboldUi.Element.Animations
{
    /// <summary>
    /// Generic animation component that drives appear and disappear transitions using tween parameters.
    /// </summary>
    /// <typeparam name="TParams">Parameter type describing the animation behaviour.</typeparam>
    public abstract class AUiAnimation<TParams> : AUiAnimationBase where TParams : IUiAnimationParameters
    {
        /// <summary>
        /// Determines whether consumers should wait for the tween to finish before continuing.
        /// </summary>
        [SerializeField] public bool _needWaitAnimation;

        [FormerlySerializedAs("useDefaultParameters")]
        [SerializeField]
        private bool _useDefaultParameters = true;

#if KOBOLD_ALCHEMY_SUPPORT
        [ShowIf(nameof(NeedUseCustomParameters))]
#endif
        [FormerlySerializedAs("animationParameters")]
        [SerializeField]
        private TParams _animationParameters;

        [InjectOptional] private TParams _defaultAnimationParameters;

        /// <summary>
        /// Gets the animation parameters in use, falling back to injected defaults if needed.
        /// </summary>
        protected TParams AnimationParameters
        {
            get
            {
                if (!_useDefaultParameters)
                    return _animationParameters;

                if (_defaultAnimationParameters == null)
                    throw new Exception($"Default animation parameters {typeof(TParams)} wasn't found");

                return _defaultAnimationParameters;
            }
        }

        /// <summary>
        /// Returns whether the component should expose custom parameter controls in the editor.
        /// </summary>
        public bool NeedUseCustomParameters()
        {
            return !_useDefaultParameters;
        }

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


        /// <summary>
        /// Performs any setup required before the appear animation starts.
        /// </summary>
        protected abstract void PrepareToAppear();
        /// <summary>
        /// Builds the tween that plays when the view appears.
        /// </summary>
        protected abstract Tween AnimateAppear();
        /// <summary>
        /// Builds the tween that plays when the view disappears.
        /// </summary>
        protected abstract Tween AnimateDisappear();
        
        private static IUiAction SelectCorrectUiAction(in IUiActionsPool pool, Tween tween, bool needWaitAnimation)
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
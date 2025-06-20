﻿using System;
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
    public abstract class AUiAnimation<TParams> : AUiAnimationBase where TParams : IUiAnimationParameters
    {
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

        public bool NeedUseCustomParameters()
        {
            return !_useDefaultParameters;
        }

        public override IUiAction Appear(in IUiActionsPool pool)
        {
            PrepareToAppear();
            gameObject.SetActive(true);

            var tween = AnimateAppear();
            return SelectCorrectUiAction(pool, tween, _needWaitAnimation);
        }

        public override IUiAction Disappear(in IUiActionsPool pool)
        {
            var tween = AnimateDisappear();
            tween.OnComplete(DisappearInstantly);
            
            return SelectCorrectUiAction(pool, tween, _needWaitAnimation);
        }

        public override IUiAction AnimateFocusReturn(in IUiActionsPool pool)
        {
            pool.GetAction(out EmptyAction emptyAction);
            return emptyAction;
        }

        public override IUiAction AnimateFocusRemoved(in IUiActionsPool pool)
        {
            pool.GetAction(out EmptyAction emptyAction);
            return emptyAction;
        }

        public override void DisappearInstantly()
        {
            gameObject.SetActive(false);
        }


        protected abstract void PrepareToAppear();
        protected abstract Tween AnimateAppear();
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
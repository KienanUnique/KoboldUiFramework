using System;
using DG.Tweening;
using KoboldUi.Element.Animations.Parameters.Impl;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Impl.Common;
using UnityEngine;

namespace KoboldUi.Element.Animations.Impl
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeAnimation : AUiAnimation<FadeAnimationParameters>
    {
        private const float FADE_DISAPPEAR_VALUE = 0f;
        private const float FADE_APPEAR_VALUE = 1f;
        
        private Tween _currentAnimation;
        
        private CanvasGroup _canvasGroup;
        
        protected override void PrepareToAppear()
        {
            _canvasGroup.alpha = FADE_DISAPPEAR_VALUE;
        }

        protected override IUiAction AnimateAppear()
        {
            _currentAnimation?.Kill();

            _currentAnimation = _canvasGroup.DOFade(FADE_APPEAR_VALUE, AnimationParameters.AppearDuration)
                .SetEase(AnimationParameters.Ease)
                .SetUpdate(true)
                .SetLink(_canvasGroup.gameObject);

            var tweenAction = new TweenAction();
            tweenAction.Setup(_currentAnimation);
            return tweenAction;
        }

        protected override IUiAction AnimateDisappear(Action callback)
        {
            _currentAnimation?.Kill();

            _currentAnimation = _canvasGroup.DOFade(FADE_DISAPPEAR_VALUE, AnimationParameters.DisappearDuration)
                .SetEase(AnimationParameters.Ease)
                .SetUpdate(true)
                .SetLink(_canvasGroup.gameObject)
                .OnComplete(callback.Invoke);
            
            var tweenAction = new TweenAction();
            tweenAction.Setup(_currentAnimation);
            return tweenAction;
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
    }
}
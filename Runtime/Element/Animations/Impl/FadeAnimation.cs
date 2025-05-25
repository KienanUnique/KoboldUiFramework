﻿using DG.Tweening;
using KoboldUi.Element.Animations.Parameters.Impl;
using UnityEngine;

namespace KoboldUi.Element.Animations.Impl
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeAnimation : AUiAnimation<FadeAnimationParameters>
    {
        private const float FADE_DISAPPEAR_VALUE = 0f;
        private const float FADE_APPEAR_VALUE = 1f;

        private CanvasGroup _canvasGroup;

        private Tween _currentAnimation;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void PrepareToAppear()
        {
            _canvasGroup.alpha = FADE_DISAPPEAR_VALUE;
        }

        protected override Tween AnimateAppear()
        {
            _currentAnimation?.Kill();

            _currentAnimation = _canvasGroup.DOFade(FADE_APPEAR_VALUE, AnimationParameters.AppearDuration)
                .SetEase(AnimationParameters.Ease)
                .SetUpdate(true)
                .SetLink(_canvasGroup.gameObject);
            
            return _currentAnimation;
        }

        protected override Tween AnimateDisappear()
        {
            _currentAnimation?.Kill();

            _currentAnimation = _canvasGroup.DOFade(FADE_DISAPPEAR_VALUE, AnimationParameters.DisappearDuration)
                .SetEase(AnimationParameters.Ease)
                .SetUpdate(true)
                .SetLink(_canvasGroup.gameObject);
                
            return _currentAnimation;
        }
    }
}
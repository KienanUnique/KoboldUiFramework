using System;
using DG.Tweening;
using KoboldUi.Element.Animations.Parameters.Impl;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
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

        protected override IUiAction AnimateAppear(in IUiActionsPool pool)
        {
            _currentAnimation?.Kill();

            _currentAnimation = _canvasGroup.DOFade(FADE_APPEAR_VALUE, AnimationParameters.AppearDuration)
                .SetEase(AnimationParameters.Ease)
                .SetUpdate(true)
                .SetLink(_canvasGroup.gameObject);

            pool.GetAction(out var tweenAction, _currentAnimation);
            return tweenAction;
        }

        protected override IUiAction AnimateDisappear(in IUiActionsPool pool, Action callback)
        {
            _currentAnimation?.Kill();

            _currentAnimation = _canvasGroup.DOFade(FADE_DISAPPEAR_VALUE, AnimationParameters.DisappearDuration)
                .SetEase(AnimationParameters.Ease)
                .SetUpdate(true)
                .SetLink(_canvasGroup.gameObject)
                .OnComplete(callback.Invoke);

            pool.GetAction(out var tweenAction, _currentAnimation);
            return tweenAction;
        }
    }
}
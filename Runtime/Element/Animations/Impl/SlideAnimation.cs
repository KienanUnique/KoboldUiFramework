using System;
using DG.Tweening;
using KoboldUi.Element.Animations.Parameters.Impl;
using KoboldUi.Utils;
using UnityEngine;

namespace KoboldUi.Element.Animations.Impl
{
    [RequireComponent(typeof(RectTransform))]
    public class SlideAnimation : AUiAnimation<SlideAnimationParameters>
    {
        private Tween _currentAnimation;
        private Vector2 _originalAnchoredPosition;
        private RectTransform _rectTransform;

        protected override void AnimateAppear()
        {
            _currentAnimation?.Kill();

            var startPosition = CalculateTargetPoint(AnimationParameters.AppearTarget);
            _rectTransform.anchoredPosition = startPosition;

            _rectTransform.DOAnchorPos(_originalAnchoredPosition, AnimationParameters.AppearDuration)
                .SetUpdate(true)
                .SetEase(AnimationParameters.AppearEase)
                .SetLink(gameObject);
        }

        protected override void AnimateDisappear(Action callback)
        {
            _currentAnimation?.Kill();

            var endPosition = CalculateTargetPoint(AnimationParameters.DisappearTarget);

            _rectTransform.DOAnchorPos(endPosition, AnimationParameters.DisappearDuration)
                .SetEase(AnimationParameters.DisappearEase)
                .SetUpdate(true)
                .SetLink(gameObject);
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _originalAnchoredPosition = _rectTransform.anchoredPosition;
        }

        private Vector2 CalculateTargetPoint(ESlideTarget target)
        {
            var rect = _rectTransform.rect;
            return target switch
            {
                ESlideTarget.Left => new Vector2(-2 * rect.width, 0f),
                ESlideTarget.Right => new Vector2(2 * rect.width, 0f),
                ESlideTarget.Top => new Vector2(0f, -2 * rect.height),
                ESlideTarget.Bottom => new Vector2(0f, 2 * rect.height),
                _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
            };
        }
    }
}
using DG.Tweening;
using KoboldUi.Element.Animations.Parameters.Impl;
using UnityEngine;

namespace KoboldUi.Element.Animations.Impl
{
    /// <summary>
    /// Animates transform scale to show or hide views.
    /// </summary>
    public class ScaleAnimation : AUiTweenAnimation<ScaleAnimationParameters>
    {
        private const float DEFAULT_DURATION = 0.2f;
        private readonly Vector3 _appearScale = Vector3.one;
        private readonly Vector3 _disappearScale = Vector3.zero;

        private Tween _currentAnimation;

        /// <inheritdoc />
        protected override void PrepareToAppear()
        {
            transform.localScale = _disappearScale;
        }

        /// <inheritdoc />
        protected override Tween AnimateAppear()
        {
            _currentAnimation?.Kill();
            var parameters = OptionalAnimationParameters;
            _currentAnimation = transform.DOScale(_appearScale, parameters?.Duration ?? DEFAULT_DURATION)
                .SetEase(parameters?.AppearEase ?? Ease.OutBack)
                .SetUpdate(true)
                .SetLink(gameObject);

            return _currentAnimation;
        }

        /// <inheritdoc />
        protected override Tween AnimateDisappear()
        {
            _currentAnimation?.Kill();
            var parameters = OptionalAnimationParameters;
            _currentAnimation = transform.DOScale(_disappearScale, parameters?.Duration ?? DEFAULT_DURATION)
                .SetEase(parameters?.DisappearEase ?? Ease.InBack)
                .SetUpdate(true)
                .SetLink(gameObject);

            return _currentAnimation;
        }
    }
}

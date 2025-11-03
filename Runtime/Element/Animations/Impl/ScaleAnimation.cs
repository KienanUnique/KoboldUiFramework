using DG.Tweening;
using KoboldUi.Element.Animations.Parameters.Impl;
using UnityEngine;

namespace KoboldUi.Element.Animations.Impl
{
    /// <summary>
    /// Animates transform scale to show or hide views.
    /// </summary>
    public class ScaleAnimation : AUiAnimation<ScaleAnimationParameters>
    {
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

            _currentAnimation = transform.DOScale(_appearScale, AnimationParameters.Duration)
                .SetEase(AnimationParameters.AppearEase)
                .SetUpdate(true)
                .SetLink(gameObject);

            return _currentAnimation;
        }

        /// <inheritdoc />
        protected override Tween AnimateDisappear()
        {
            _currentAnimation?.Kill();

            _currentAnimation = transform.DOScale(_disappearScale, AnimationParameters.Duration)
                .SetEase(AnimationParameters.DisappearEase)
                .SetUpdate(true)
                .SetLink(gameObject);

            return _currentAnimation;
        }
    }
}
using System;
using DG.Tweening;
using Element.Animations.Parameters.Impl;
using UnityEngine;

namespace Element.Animations.Impl
{
    public class ScaleAnimation : AUiAnimation<ScaleAnimationParameters>
    {
        private readonly Vector3 _disappearScale = Vector3.zero;
        private readonly Vector3 _appearScale = Vector3.one;
        
        private Tween _currentAnimation;
        
        protected override void AnimateAppear()
        {
            _currentAnimation?.Kill();

            transform.localScale = _disappearScale;
            
            _currentAnimation = transform.DOScale(_appearScale, AnimationParameters.Duration)
                .SetEase(AnimationParameters.AppearEase)
                .SetLink(gameObject);
        }

        protected override void AnimateDisappear(Action callback)
        {
            _currentAnimation?.Kill();
            
            _currentAnimation = transform.DOScale(_disappearScale, AnimationParameters.Duration)
                .SetEase(AnimationParameters.DisappearEase)
                .SetLink(gameObject)
                .OnComplete(callback.Invoke);;
        }
    }
}
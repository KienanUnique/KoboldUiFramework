using System;
using KoboldUi.Element.Animations.Parameters.Impl;
using Cysharp.Threading.Tasks;
using DG.Tweening;
#if KOBOLD_ALCHEMY_SUPPORT
using Alchemy.Inspector;
#endif
using UnityEngine;

namespace KoboldUi.Element.Animations.Impl
{
    [RequireComponent(typeof(RectTransform))]
    public class SlideAnimation : AUiAnimation<SlideAnimationParameters>
    {
        [SerializeField] private Vector2 fromAppearAnchoredPosition;
        [SerializeField] private bool disappearToTheSamePlace = true;

#if KOBOLD_ALCHEMY_SUPPORT
      [HideIf(nameof(DisappearToTheSamePlace))]  
#endif
        [SerializeField] private Vector2 toDisappearAnchoredPosition;
        
        private Tween _currentAnimation;
        private Vector2 _originalAnchoredPosition;
        private RectTransform _rectTransform;
        
#if KOBOLD_ALCHEMY_SUPPORT
        public bool DisappearToTheSamePlace() => disappearToTheSamePlace;
#endif
        
        protected override void PrepareToAppear()
        {
            _rectTransform.anchoredPosition = fromAppearAnchoredPosition;
        }

        protected override UniTask AnimateAppear()
        {
            _currentAnimation?.Kill();

            _currentAnimation = _rectTransform
                .DOAnchorPos(_originalAnchoredPosition, AnimationParameters.AppearDuration)
                .SetUpdate(true)
                .SetEase(AnimationParameters.AppearEase)
                .SetLink(gameObject);

            return _currentAnimation.ToUniTask();
        }

        protected override UniTask AnimateDisappear(Action callback)
        {
            _currentAnimation?.Kill();

            var disappearTargetPosition = disappearToTheSamePlace ? fromAppearAnchoredPosition : toDisappearAnchoredPosition;
            
            _currentAnimation = _rectTransform.DOAnchorPos(disappearTargetPosition, AnimationParameters.DisappearDuration)
                .SetEase(AnimationParameters.DisappearEase)
                .SetUpdate(true)
                .SetLink(gameObject)
                .OnComplete(base.DisappearInstantly);

            return _currentAnimation.ToUniTask();
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _originalAnchoredPosition = _rectTransform.anchoredPosition;
        }
    }
}
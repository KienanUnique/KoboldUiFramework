using DG.Tweening;
using KoboldUi.Element.Animations.Parameters.Impl;
using UnityEngine;
#if KOBOLD_ALCHEMY_SUPPORT
using Alchemy.Inspector;
#endif

namespace KoboldUi.Element.Animations.Impl
{
    [RequireComponent(typeof(RectTransform))]
    public class SlideAnimation : AUiAnimation<SlideAnimationParameters>
    {
        [SerializeField] private Vector2 _fromAppearAnchoredPosition;
        [SerializeField] private bool _disappearToTheSamePlace = true;

#if KOBOLD_ALCHEMY_SUPPORT
        [HideIf(nameof(DisappearToTheSamePlace))]
#endif
        [SerializeField]
        private Vector2 _toDisappearAnchoredPosition;

        private Tween _currentAnimation;
        private Vector2 _originalAnchoredPosition;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _originalAnchoredPosition = _rectTransform.anchoredPosition;
        }

#if KOBOLD_ALCHEMY_SUPPORT
        public bool DisappearToTheSamePlace()
        {
            return _disappearToTheSamePlace;
        }
#endif

        protected override void PrepareToAppear()
        {
            _rectTransform.anchoredPosition = _fromAppearAnchoredPosition;
        }

        protected override Tween AnimateAppear()
        {
            _currentAnimation?.Kill();

            _currentAnimation = _rectTransform
                .DOAnchorPos(_originalAnchoredPosition, AnimationParameters.AppearDuration)
                .SetUpdate(true)
                .SetEase(AnimationParameters.AppearEase)
                .SetLink(gameObject);

            return _currentAnimation;
        }

        protected override Tween AnimateDisappear()
        {
            _currentAnimation?.Kill();

            var disappearTargetPosition =
                _disappearToTheSamePlace ? _fromAppearAnchoredPosition : _toDisappearAnchoredPosition;

            _currentAnimation = _rectTransform
                .DOAnchorPos(disappearTargetPosition, AnimationParameters.DisappearDuration)
                .SetEase(AnimationParameters.DisappearEase)
                .SetUpdate(true)
                .SetLink(gameObject);

            return _currentAnimation;
        }
    }
}
using DG.Tweening;
using KoboldUi.Element.Animations.Parameters.Impl;
using UnityEngine;
#if KOBOLD_ALCHEMY_SUPPORT
using Alchemy.Inspector;
#elif KOBOLD_ODIN_SUPPORT
using Sirenix.OdinInspector;
#endif

namespace KoboldUi.Element.Animations.Impl
{
    /// <summary>
    /// Animates a RectTransform between anchored positions to slide views.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class SlideAnimation : AUiTweenAnimation<SlideAnimationParameters>
    {
        private const float DEFAULT_DURATION = 0.4f;
        [SerializeField] private Vector2 _fromAppearAnchoredPosition;
        [SerializeField] private bool _disappearToTheSamePlace = true;

#if KOBOLD_ALCHEMY_SUPPORT || KOBOLD_ODIN_SUPPORT
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

#if KOBOLD_ALCHEMY_SUPPORT || KOBOLD_ODIN_SUPPORT
        /// <summary>
        /// Indicates whether the disappear animation returns to the appear origin.
        /// </summary>
        public bool DisappearToTheSamePlace()
        {
            return _disappearToTheSamePlace;
        }
#endif

        /// <inheritdoc />
        protected override void PrepareToAppear()
        {
            _rectTransform.anchoredPosition = _fromAppearAnchoredPosition;
        }

        /// <inheritdoc />
        protected override Tween AnimateAppear()
        {
            _currentAnimation?.Kill();
            var parameters = OptionalAnimationParameters;
            _currentAnimation = _rectTransform
                .DOAnchorPos(_originalAnchoredPosition, parameters?.AppearDuration ?? DEFAULT_DURATION)
                .SetUpdate(true)
                .SetEase(parameters?.AppearEase ?? Ease.OutBack)
                .SetLink(gameObject);

            return _currentAnimation;
        }

        /// <inheritdoc />
        protected override Tween AnimateDisappear()
        {
            _currentAnimation?.Kill();
            var parameters = OptionalAnimationParameters;
            var disappearTargetPosition =
                _disappearToTheSamePlace ? _fromAppearAnchoredPosition : _toDisappearAnchoredPosition;

            _currentAnimation = _rectTransform
                .DOAnchorPos(disappearTargetPosition, parameters?.DisappearDuration ?? DEFAULT_DURATION)
                .SetEase(parameters?.DisappearEase ?? Ease.InBack)
                .SetUpdate(true)
                .SetLink(gameObject);

            return _currentAnimation;
        }
    }
}

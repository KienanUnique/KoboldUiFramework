using System;
using DG.Tweening;
using KoboldUi.Element.Animations.Parameters.Impl;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
using UnityEngine;
#if KOBOLD_ALCHEMY_SUPPORT
using Alchemy.Inspector;
#endif

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
        [SerializeField]
        private Vector2 toDisappearAnchoredPosition;

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
            return disappearToTheSamePlace;
        }
#endif

        protected override void PrepareToAppear()
        {
            _rectTransform.anchoredPosition = fromAppearAnchoredPosition;
        }

        protected override IUiAction AnimateAppear(in IUiActionsPool pool)
        {
            _currentAnimation?.Kill();

            _currentAnimation = _rectTransform
                .DOAnchorPos(_originalAnchoredPosition, AnimationParameters.AppearDuration)
                .SetUpdate(true)
                .SetEase(AnimationParameters.AppearEase)
                .SetLink(gameObject);

            pool.GetAction(out var tweenAction, _currentAnimation);
            return tweenAction;
        }

        protected override IUiAction AnimateDisappear(in IUiActionsPool pool, Action callback)
        {
            _currentAnimation?.Kill();

            var disappearTargetPosition =
                disappearToTheSamePlace ? fromAppearAnchoredPosition : toDisappearAnchoredPosition;

            _currentAnimation = _rectTransform
                .DOAnchorPos(disappearTargetPosition, AnimationParameters.DisappearDuration)
                .SetEase(AnimationParameters.DisappearEase)
                .SetUpdate(true)
                .SetLink(gameObject)
                .OnComplete(callback.Invoke);

            pool.GetAction(out var tweenAction, _currentAnimation);
            return tweenAction;
        }
    }
}
using DG.Tweening;
using KoboldUi.Utils;
using UnityEngine;

namespace KoboldUi.Element.Animations.Parameters.Impl
{
    /// <summary>
    /// Stores configuration for slide-in and slide-out animations.
    /// </summary>
    [CreateAssetMenu(menuName = AssetMenuPath.ANIMATION_PARAMETERS + nameof(SlideAnimationParameters),
        fileName = nameof(SlideAnimationParameters))]
    public class SlideAnimationParameters : AUiAnimationParameters
    {
        [Header("Appear")] [SerializeField] private Ease appearEase = Ease.OutBack;

        [SerializeField] private float appearDuration = 0.4f;

        [Header("Disappear")] [SerializeField] private Ease disappearEase = Ease.InBack;

        [SerializeField] private float disappearDuration = 0.4f;

        /// <summary>
        /// Gets the easing used when the view slides in.
        /// </summary>
        public Ease AppearEase => appearEase;
        /// <summary>
        /// Gets the easing used when the view slides out.
        /// </summary>
        public Ease DisappearEase => disappearEase;
        /// <summary>
        /// Gets the duration of the appear slide.
        /// </summary>
        public float AppearDuration => appearDuration;
        /// <summary>
        /// Gets the duration of the disappear slide.
        /// </summary>
        public float DisappearDuration => disappearDuration;
    }
}
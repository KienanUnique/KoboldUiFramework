using DG.Tweening;
using KoboldUi.Utils;
using UnityEngine;

namespace KoboldUi.Element.Animations.Parameters.Impl
{
    /// <summary>
    /// Stores configuration for fade-based view animations.
    /// </summary>
    [CreateAssetMenu(menuName = AssetMenuPath.ANIMATION_PARAMETERS + nameof(FadeAnimationParameters),
        fileName = nameof(FadeAnimationParameters))]
    public class FadeAnimationParameters : AUiAnimationParameters
    {
        [SerializeField] private float appearDuration = 0.5f;
        [SerializeField] private float disappearDuration = 0.5f;
        [SerializeField] private Ease ease = Ease.Linear;

        /// <summary>
        /// Gets the easing applied to the fade tween.
        /// </summary>
        public Ease Ease => ease;
        /// <summary>
        /// Gets the duration of the appear tween.
        /// </summary>
        public float AppearDuration => appearDuration;
        /// <summary>
        /// Gets the duration of the disappear tween.
        /// </summary>
        public float DisappearDuration => disappearDuration;
    }
}
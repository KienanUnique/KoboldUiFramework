using DG.Tweening;
using KoboldUi.Utils;
using UnityEngine;

namespace KoboldUi.Element.Animations.Parameters.Impl
{
    /// <summary>
    /// Stores configuration for scale-based animations.
    /// </summary>
    [CreateAssetMenu(menuName = AssetMenuPath.ANIMATION_PARAMETERS + nameof(ScaleAnimationParameters),
        fileName = nameof(ScaleAnimationParameters))]
    public class ScaleAnimationParameters : AUiAnimationParameters
    {
        [SerializeField] private float duration = 0.2f;
        [SerializeField] private Ease appearEase = Ease.OutBack;
        [SerializeField] private Ease disappearEase = Ease.InBack;

        /// <summary>
        /// Gets the duration of the scaling tween.
        /// </summary>
        public float Duration => duration;
        /// <summary>
        /// Gets the easing used when the view scales in.
        /// </summary>
        public Ease AppearEase => appearEase;
        /// <summary>
        /// Gets the easing used when the view scales out.
        /// </summary>
        public Ease DisappearEase => disappearEase;
    }
}
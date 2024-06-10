using DG.Tweening;
using UnityEngine;
using Utils;

namespace Element.Animations.Parameters.Impl
{
    [CreateAssetMenu(menuName = AssetMenuPath.ANIMATION_PARAMETERS + nameof(FadeAnimationParameters),
        fileName = nameof(FadeAnimationParameters))]
    public class FadeAnimationParameters : AUiAnimationParameters
    {
        [SerializeField] private float appearDuration = 0.5f;
        [SerializeField] private float disappearDuration = 0.5f;
        [SerializeField] private Ease ease = Ease.Linear;
        
        public Ease Ease => ease;
        public float AppearDuration => appearDuration;
        public float DisappearDuration => disappearDuration;
    }
}
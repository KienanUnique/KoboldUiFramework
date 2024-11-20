using DG.Tweening;
using KoboldUi.Utils;
using UnityEngine;

namespace KoboldUi.Element.Animations.Parameters.Impl
{
    [CreateAssetMenu(menuName = AssetMenuPath.ANIMATION_PARAMETERS + nameof(SlideAnimationParameters),
        fileName = nameof(SlideAnimationParameters))]
    public class SlideAnimationParameters : AUiAnimationParameters
    {
        [Header("Appear")]
        [SerializeField] private Ease appearEase = Ease.OutBack;
        [SerializeField] private float appearDuration = 0.4f;
        
        [Header("Disappear")]
        [SerializeField] private Ease disappearEase = Ease.InBack;
        [SerializeField] private float disappearDuration = 0.4f;
        
        public Ease AppearEase => appearEase;
        public Ease DisappearEase => disappearEase;
        public float AppearDuration => appearDuration;
        public float DisappearDuration => disappearDuration;
    }
}
using DG.Tweening;
using KoboldUiFramework.Utils;
using UnityEngine;

namespace KoboldUiFramework.Element.Animations.Parameters.Impl
{
    [CreateAssetMenu(menuName = AssetMenuPath.ANIMATION_PARAMETERS + nameof(ScaleAnimationParameters),
        fileName = nameof(ScaleAnimationParameters))]
    public class ScaleAnimationParameters : AUiAnimationParameters
    {
        [SerializeField] private float duration = 0.2f;
        [SerializeField] private Ease appearEase = Ease.OutBack;
        [SerializeField] private Ease disappearEase = Ease.InBack;

        public float Duration => duration;
        public Ease AppearEase => appearEase;
        public Ease DisappearEase => disappearEase;
    }
}
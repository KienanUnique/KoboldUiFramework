using KoboldUi.Element.Animations.Parameters.Impl;
using KoboldUi.Utils;
using UnityEngine;
using Zenject;

namespace KoboldUi.Installers
{
    [CreateAssetMenu(menuName = AssetMenuPath.INSTALLERS + nameof(DefaultAnimationsInstaller),
        fileName = nameof(DefaultAnimationsInstaller))]
    public class DefaultAnimationsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private FadeAnimationParameters fadeAnimationParameters;
        [SerializeField] private ScaleAnimationParameters scaleAnimationParameters;
        [SerializeField] private SlideAnimationParameters slideAnimationParameters;

        public override void InstallBindings()
        {
            Container.BindInstance(fadeAnimationParameters).AsSingle();
            Container.BindInstance(scaleAnimationParameters).AsSingle();
            Container.BindInstance(slideAnimationParameters).AsSingle();
        }
    }
}
using Element.Animations.Parameters.Impl;
using UnityEngine;
using Utils;
using Zenject;

namespace Installers
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
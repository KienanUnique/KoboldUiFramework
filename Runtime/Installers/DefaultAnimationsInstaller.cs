using KoboldUi.Element.Animations.Parameters.Impl;
using UnityEngine;

#if KOBOLD_ZENJECT_SUPPORT
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

#elif KOBOLD_VCONTAINER_SUPPORT
using VContainer;
using VContainer.Unity;


namespace KoboldUi.Installers
{
    public class DefaultAnimationsInstaller : LifetimeScope
    {
        [SerializeField] private FadeAnimationParameters fadeAnimationParameters;
        [SerializeField] private ScaleAnimationParameters scaleAnimationParameters;
        [SerializeField] private SlideAnimationParameters slideAnimationParameters;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(fadeAnimationParameters);
            builder.RegisterInstance(scaleAnimationParameters);
            builder.RegisterInstance(slideAnimationParameters);
        }
    }
}
#endif
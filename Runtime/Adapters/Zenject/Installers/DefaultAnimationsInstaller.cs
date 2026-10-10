using KoboldUi.Element.Animations.Parameters.Impl;
using KoboldUi.Element.Animations;
using KoboldUi.Utils;
using UnityEngine;
using Zenject;

namespace KoboldUi.Installers
{
    /// <summary>
    /// Installs default animation parameter assets into the dependency container.
    /// </summary>
    [CreateAssetMenu(menuName = AssetMenuPath.INSTALLERS + nameof(DefaultAnimationsInstaller),
        fileName = nameof(DefaultAnimationsInstaller))]
    public class DefaultAnimationsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private FadeAnimationParameters fadeAnimationParameters;
        [SerializeField] private ScaleAnimationParameters scaleAnimationParameters;
        [SerializeField] private SlideAnimationParameters slideAnimationParameters;
        [SerializeField] private UiAnimationDefaults animationDefaults;

        /// <inheritdoc />
        public override void InstallBindings()
        {
            Container.BindInstance(fadeAnimationParameters).AsSingle();
            Container.BindInstance(scaleAnimationParameters).AsSingle();
            Container.BindInstance(slideAnimationParameters).AsSingle();
            var defaults = animationDefaults;
            if (defaults == null)
            {
                defaults = CreateInstance<UiAnimationDefaults>();
                if (fadeAnimationParameters != null) defaults.Add(fadeAnimationParameters);
                if (scaleAnimationParameters != null) defaults.Add(scaleAnimationParameters);
                if (slideAnimationParameters != null) defaults.Add(slideAnimationParameters);
            }
            Container.BindInstance(defaults).AsSingle();
        }
    }
}

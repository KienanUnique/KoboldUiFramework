using KoboldUi.Composition;
using KoboldUi.Composition.Zenject;
using KoboldUi.Installers;
using NUnit.Framework;
using UnityEngine;
using System.Reflection;
using KoboldUi.Element.Animations;
using KoboldUi.Element.Animations.Parameters.Impl;
using Zenject;

namespace KoboldUi.Tests.Zenject
{
    public class ZenjectInstallerTests
    {
        [Test]
        public void DefaultAnimationInstallerProvidesExplicitParameterAssets()
        {
            var installer = ScriptableObject.CreateInstance<DefaultAnimationsInstaller>();
            var fade = ScriptableObject.CreateInstance<FadeAnimationParameters>();
            var scale = ScriptableObject.CreateInstance<ScaleAnimationParameters>();
            var slide = ScriptableObject.CreateInstance<SlideAnimationParameters>();
            try
            {
                var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                typeof(DefaultAnimationsInstaller).GetField("fadeAnimationParameters", flags).SetValue(installer, fade);
                typeof(DefaultAnimationsInstaller).GetField("scaleAnimationParameters", flags).SetValue(installer, scale);
                typeof(DefaultAnimationsInstaller).GetField("slideAnimationParameters", flags).SetValue(installer, slide);
                var container = new DiContainer();
                container.Inject(installer);
                installer.InstallBindings();
                Assert.That(container.Resolve<FadeAnimationParameters>(), Is.SameAs(fade));
                Assert.That(container.Resolve<ScaleAnimationParameters>(), Is.SameAs(scale));
                Assert.That(container.Resolve<SlideAnimationParameters>(), Is.SameAs(slide));
            }
            finally
            {
                Object.DestroyImmediate(installer);
                Object.DestroyImmediate(fade);
                Object.DestroyImmediate(scale);
                Object.DestroyImmediate(slide);
            }
        }

        [Test]
        public void LocalInstallerRegistersZenjectResolver()
        {
            var gameObject = new GameObject(nameof(LocalInstallerRegistersZenjectResolver));
            try
            {
                var container = new DiContainer();
                var installer = gameObject.AddComponent<LocalWindowsServiceInstaller>();
                container.Inject(installer);
                installer.InstallBindings();
                Assert.That(container.Resolve<IUiCompositionAdapter>(), Is.TypeOf<ZenjectUiCompositionAdapter>());
                Assert.That(container.Resolve<WindowRegistry>(), Is.Not.Null);
            }
            finally { Object.DestroyImmediate(gameObject); }
        }

        [Test]
        public void ProjectInstallerRegistersZenjectResolver()
        {
            var gameObject = new GameObject(nameof(ProjectInstallerRegistersZenjectResolver));
            try
            {
                var container = new DiContainer();
                var installer = gameObject.AddComponent<ProjectWindowsServiceInstaller>();
                container.Inject(installer);
                installer.InstallBindings();
                Assert.That(container.Resolve<IUiCompositionAdapter>(), Is.TypeOf<ZenjectUiCompositionAdapter>());
                Assert.That(container.Resolve<WindowRegistry>(), Is.Not.Null);
            }
            finally { Object.DestroyImmediate(gameObject); }
        }
    }
}

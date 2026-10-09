using KoboldUi.Composition;
using KoboldUi.Composition.Zenject;
using KoboldUi.Installers;
using NUnit.Framework;
using UnityEngine;
using Zenject;

namespace KoboldUi.Tests.Zenject
{
    public class ZenjectInstallerTests
    {
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
                Assert.That(container.Resolve<IUiResolver>(), Is.TypeOf<ZenjectUiResolver>());
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
                Assert.That(container.Resolve<IUiResolver>(), Is.TypeOf<ZenjectUiResolver>());
                Assert.That(container.Resolve<WindowRegistry>(), Is.Not.Null);
            }
            finally { Object.DestroyImmediate(gameObject); }
        }
    }
}

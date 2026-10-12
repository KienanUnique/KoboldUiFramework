using KoboldUi.Adapters.VContainer;
using KoboldUi.Composition;
using KoboldUi.Services.WindowsService;
using NUnit.Framework;
using UnityEngine;
using VContainer;

namespace KoboldUi.Tests.VContainer
{
    public class VContainerBootstrapTests
    {
        [Test]
        public void ManualPresetWaitsForExplicitInitialization()
        {
            var canvasObject = new GameObject("Canvas", typeof(Canvas));
            var prefabObject = new GameObject("Prefab");
            IObjectResolver container = null;
            UiBootstrap ui = null;
            try
            {
                var builder = new ContainerBuilder();
                ui = builder.RegisterKoboldUi(canvasObject.GetComponent<Canvas>(), WindowScope.Local,
                        initializationMode: UiInitializationMode.Manual)
                    .RegisterWindow(prefabObject.AddComponent<VContainerBootstrapTestWindow>()).Build();
                builder.RegisterInstance(new VContainerBootstrapTestDependency());
                container = builder.Build();
                Assert.That(canvasObject.transform.childCount, Is.Zero);
                Assert.Throws<System.InvalidOperationException>(() => ui.LocalWindows.OpenWindow<VContainerBootstrapTestWindow>());
                ui.Initialize();
                Assert.That(ui.Get<VContainerBootstrapTestWindow>().InitializeCount, Is.EqualTo(1));
            }
            finally
            {
                ui?.Dispose();
                container?.Dispose();
                Object.DestroyImmediate(canvasObject);
                Object.DestroyImmediate(prefabObject);
            }
        }

        [Test]
        public void PresetBindsServiceBeforeInitializingWindowWhenContainerBuilds()
        {
            var canvasObject = new GameObject("Canvas", typeof(Canvas));
            var prefabObject = new GameObject("Prefab");
            IObjectResolver container = null;
            UiBootstrap ui = null;
            try
            {
                var builder = new ContainerBuilder();
                var prefab = prefabObject.AddComponent<VContainerBootstrapTestWindow>();
                ui = builder.RegisterKoboldUi(canvasObject.GetComponent<Canvas>(), WindowScope.Local)
                    .RegisterWindow(prefab).Build();
                builder.RegisterInstance(new VContainerBootstrapTestDependency());
                Assert.That(canvasObject.transform.childCount, Is.Zero);
                container = builder.Build();
                Assert.That(container.Resolve<ILocalWindowsService>(), Is.SameAs(ui.LocalWindows));
                Assert.That(ui.Get<VContainerBootstrapTestWindow>().InitializeCount, Is.EqualTo(1));
            }
            finally
            {
                ui?.Dispose();
                container?.Dispose();
                Object.DestroyImmediate(canvasObject);
                Object.DestroyImmediate(prefabObject);
            }
        }

        [Test]
        public void PresetCanRegisterWindowsInBuildCallback()
        {
            var canvasObject = new GameObject("Canvas", typeof(Canvas));
            var prefabObject = new GameObject("Prefab");
            IObjectResolver container = null;
            UiBootstrap ui = null;
            try
            {
                var builder = new ContainerBuilder();
                var prefab = prefabObject.AddComponent<VContainerBootstrapTestWindow>();
                ui = builder.RegisterKoboldUi(canvasObject.GetComponent<Canvas>(), WindowScope.Local,
                    registerWindows: bootstrap => bootstrap.RegisterWindow(prefab)).Build();
                builder.RegisterInstance(new VContainerBootstrapTestDependency());
                container = builder.Build();
                Assert.That(ui.Get<VContainerBootstrapTestWindow>().Dependency, Is.Not.Null);
            }
            finally
            {
                ui?.Dispose();
                container?.Dispose();
                Object.DestroyImmediate(canvasObject);
                Object.DestroyImmediate(prefabObject);
            }
        }

        [Test]
        public void BootstrapCreatesInjectsAndNavigatesToRegisteredWindow()
        {
            var localObject = new GameObject("Local canvas", typeof(Canvas));
            var projectObject = new GameObject("Project canvas", typeof(Canvas));
            var prefabObject = new GameObject("Window prefab");
            var dependency = new VContainerBootstrapTestDependency();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(dependency);
            var container = builder.Build();
            var bootstrap = new UiBootstrap(localObject.GetComponent<Canvas>(),
                projectObject.GetComponent<Canvas>(), new VContainerUiCompositionAdapter(container));

            try
            {
                var prefab = prefabObject.AddComponent<VContainerBootstrapTestWindow>();
                bootstrap.RegisterWindow(prefab);
                var window = bootstrap.Get<VContainerBootstrapTestWindow>();

                Assert.That(window, Is.Not.SameAs(prefab));
                Assert.That(window.transform.parent, Is.SameAs(localObject.transform));
                Assert.That(window.Dependency, Is.SameAs(dependency));
                Assert.That(window.InitializeCount, Is.EqualTo(1));
                Assert.That(window.LocalWindows, Is.SameAs(bootstrap.LocalWindows));

                bootstrap.Open<VContainerBootstrapTestWindow>();

                Assert.That(bootstrap.LocalWindows.CurrentWindow, Is.SameAs(window));
                Assert.That(bootstrap.LocalWindows.IsOpened<VContainerBootstrapTestWindow>(), Is.True);
                Assert.That(window.LastState, Is.EqualTo(KoboldUi.Utils.EWindowState.Active));
            }
            finally
            {
                bootstrap.Dispose();
                container.Dispose();
                Object.DestroyImmediate(prefabObject);
                Object.DestroyImmediate(projectObject);
                Object.DestroyImmediate(localObject);
            }
        }
    }
}

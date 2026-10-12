using KoboldUi.Composition;
using KoboldUi.Composition.Zenject;
using KoboldUi.Services.WindowsService;
using NUnit.Framework;
using UnityEngine;
using Zenject;

namespace KoboldUi.Tests.Zenject
{
    public class ZenjectUiBootstrapTests
    {
        [Test]
        public void ManualPresetWaitsForExplicitInitialization()
        {
            var canvasObject = new GameObject("Canvas", typeof(Canvas));
            var prefabObject = new GameObject("Prefab");
            UiBootstrap ui = null;
            try
            {
                var container = new DiContainer();
                ui = container.BindKoboldUi(canvasObject.GetComponent<Canvas>(), WindowScope.Local,
                    initializationMode: UiInitializationMode.Manual);
                ui.RegisterWindow(prefabObject.AddComponent<ZenjectBootstrapLocalWindow>());
                container.BindInstance(new ZenjectBootstrapToken("ready")).AsSingle();
                foreach (var initializable in container.ResolveAll<IInitializable>()) initializable.Initialize();
                Assert.That(canvasObject.transform.childCount, Is.Zero);
                Assert.Throws<System.InvalidOperationException>(() => ui.LocalWindows.OpenWindow<ZenjectBootstrapLocalWindow>());
                ui.Initialize();
                Assert.That(ui.Get<ZenjectBootstrapLocalWindow>().IsInitialized, Is.True);
            }
            finally
            {
                ui?.Dispose();
                Object.DestroyImmediate(canvasObject);
                Object.DestroyImmediate(prefabObject);
            }
        }

        [Test]
        public void PresetBindsServiceBeforeInitializingWindowAtZenjectInitialize()
        {
            var canvasObject = new GameObject("Canvas", typeof(Canvas));
            var prefabObject = new GameObject("Prefab");
            UiBootstrap ui = null;
            try
            {
                var container = new DiContainer();
                var prefab = prefabObject.AddComponent<ZenjectBootstrapLocalWindow>();
                var serviceToken = new ZenjectBootstrapToken("late binding");
                ui = container.BindKoboldUi(canvasObject.GetComponent<Canvas>(), WindowScope.Local);
                var windows = container.Resolve<ILocalWindowsService>();
                ui.RegisterWindow(prefab);
                Assert.That(canvasObject.transform.childCount, Is.Zero);
                container.BindInstance(serviceToken).AsSingle();
                foreach (var initializable in container.ResolveAll<IInitializable>()) initializable.Initialize();
                Assert.That(ui.Get<ZenjectBootstrapLocalWindow>().Token, Is.SameAs(serviceToken));
                Assert.That(windows, Is.SameAs(ui.LocalWindows));
            }
            finally
            {
                ui?.Dispose();
                Object.DestroyImmediate(canvasObject);
                Object.DestroyImmediate(prefabObject);
            }
        }

        [Test]
        public void PresetCanRegisterWindowsAfterLateBindings()
        {
            var canvasObject = new GameObject("Canvas", typeof(Canvas));
            var prefabObject = new GameObject("Prefab");
            UiBootstrap ui = null;
            try
            {
                var container = new DiContainer();
                var prefab = prefabObject.AddComponent<ZenjectBootstrapLocalWindow>();
                ui = container.BindKoboldUi(canvasObject.GetComponent<Canvas>(), WindowScope.Local,
                    registerWindows: bootstrap =>
                    {
                        Assert.That(container.Resolve<ZenjectBootstrapToken>().Scope, Is.EqualTo("ready"));
                        bootstrap.RegisterWindow(prefab);
                    });
                container.BindInstance(new ZenjectBootstrapToken("ready")).AsSingle();
                Assert.That(canvasObject.transform.childCount, Is.Zero);
                foreach (var initializable in container.ResolveAll<IInitializable>()) initializable.Initialize();
                Assert.That(ui.Get<ZenjectBootstrapLocalWindow>().IsInitialized, Is.True);
            }
            finally
            {
                ui?.Dispose();
                Object.DestroyImmediate(canvasObject);
                Object.DestroyImmediate(prefabObject);
            }
        }

        [Test]
        public void BootstrapUsesScopedZenjectDependenciesAndWindowServices()
        {
            var localCanvasObject = new GameObject("LocalCanvas", typeof(Canvas));
            var projectCanvasObject = new GameObject("ProjectCanvas", typeof(Canvas));
            var localPrefabObject = new GameObject("LocalPrefab");
            var projectPrefabObject = new GameObject("ProjectPrefab");
            UiBootstrap bootstrap = null;
            try
            {
                var localPrefab = localPrefabObject.AddComponent<ZenjectBootstrapLocalWindow>();
                var projectPrefab = projectPrefabObject.AddComponent<ZenjectBootstrapProjectWindow>();
                var localToken = new ZenjectBootstrapToken("local");
                var projectToken = new ZenjectBootstrapToken("project");
                var localContainer = new DiContainer();
                var projectContainer = new DiContainer();
                localContainer.BindInstance(localToken).AsSingle();
                projectContainer.BindInstance(projectToken).AsSingle();
                bootstrap = new UiBootstrap(localCanvasObject.GetComponent<Canvas>(),
                    projectCanvasObject.GetComponent<Canvas>(),
                    new ZenjectUiCompositionAdapter(projectContainer),
                    new ZenjectUiCompositionAdapter(localContainer), null);
                bootstrap.RegisterWindow(localPrefab);
                bootstrap.RegisterWindow(projectPrefab, WindowScope.Project);
                var localWindow = bootstrap.Get<ZenjectBootstrapLocalWindow>();
                var projectWindow = bootstrap.Get<ZenjectBootstrapProjectWindow>();
                Assert.That(localWindow.Token, Is.SameAs(localToken));
                Assert.That(localWindow.Controller.Token, Is.SameAs(localToken));
                Assert.That(projectWindow.Token, Is.SameAs(projectToken));
                Assert.That(projectWindow.Controller.Token, Is.SameAs(projectToken));
                Assert.That(localWindow.Controller.LocalWindows, Is.SameAs(bootstrap.LocalWindows));
                Assert.That(projectWindow.Controller.LocalWindows, Is.SameAs(bootstrap.LocalWindows));
                Assert.That(localWindow.transform.parent, Is.SameAs(localCanvasObject.transform));
                Assert.That(projectWindow.transform.parent, Is.SameAs(projectCanvasObject.transform));
                bootstrap.Open<ZenjectBootstrapLocalWindow>();
                bootstrap.Open<ZenjectBootstrapProjectWindow>();
                Assert.That(bootstrap.LocalWindows.CurrentWindow, Is.SameAs(localWindow));
                Assert.That(bootstrap.ProjectWindows.CurrentWindow, Is.SameAs(projectWindow));
            }
            finally
            {
                bootstrap?.Dispose();
                Object.DestroyImmediate(localCanvasObject);
                Object.DestroyImmediate(projectCanvasObject);
                Object.DestroyImmediate(localPrefabObject);
                Object.DestroyImmediate(projectPrefabObject);
            }
        }
    }
}

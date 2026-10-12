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
                        initializationMode: UiInitializationMode.Manual)
                    .RegisterWindow(prefabObject.AddComponent<ZenjectBootstrapLocalWindow>()).Build();
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
                ui = container.BindKoboldUi(canvasObject.GetComponent<Canvas>(), WindowScope.Local)
                    .RegisterWindow(prefab).Build();
                var windows = container.Resolve<ILocalWindowsService>();
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
                    }).Build();
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
        public void LocalControllerUsesRegisteredProjectService()
        {
            var projectCanvasObject = new GameObject("ProjectCanvas", typeof(Canvas));
            var localCanvasObject = new GameObject("LocalCanvas", typeof(Canvas));
            var projectPrefabObject = new GameObject("ProjectPrefab");
            var localPrefabObject = new GameObject("LocalPrefab");
            UiBootstrap projectUi = null;
            UiBootstrap localUi = null;
            try
            {
                var projectContainer = new DiContainer();
                projectContainer.BindInstance(new ZenjectBootstrapToken("project")).AsSingle();
                projectUi = projectContainer.BindKoboldUi(projectCanvasObject.GetComponent<Canvas>(),
                        WindowScope.Project)
                    .RegisterWindow(projectPrefabObject.AddComponent<ZenjectProjectNavigationTargetWindow>()).Build();
                foreach (var initializable in projectContainer.ResolveAll<IInitializable>()) initializable.Initialize();

                var localContainer = new DiContainer();
                localContainer.Bind<IProjectWindowsService>().FromInstance(projectUi.ProjectWindows).AsSingle();
                localUi = localContainer.BindKoboldUi(localCanvasObject.GetComponent<Canvas>(), WindowScope.Local)
                    .RegisterWindow(localPrefabObject.AddComponent<ZenjectProjectNavigationWindow>()).Build();
                foreach (var initializable in localContainer.ResolveAll<IInitializable>()) initializable.Initialize();

                var controller = localUi.Get<ZenjectProjectNavigationWindow>().Controller;
                Assert.That(controller.ProjectWindows, Is.SameAs(projectUi.ProjectWindows));
                controller.OpenProjectWindow();
                Assert.That(projectUi.ProjectWindows.CurrentWindow,
                    Is.SameAs(projectUi.Get<ZenjectProjectNavigationTargetWindow>()));
            }
            finally
            {
                localUi?.Dispose();
                projectUi?.Dispose();
                Object.DestroyImmediate(localCanvasObject);
                Object.DestroyImmediate(projectCanvasObject);
                Object.DestroyImmediate(localPrefabObject);
                Object.DestroyImmediate(projectPrefabObject);
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

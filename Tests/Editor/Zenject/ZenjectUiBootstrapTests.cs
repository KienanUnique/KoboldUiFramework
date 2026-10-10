using KoboldUi.Composition;
using KoboldUi.Composition.Zenject;
using NUnit.Framework;
using UnityEngine;
using Zenject;

namespace KoboldUi.Tests.Zenject
{
    public class ZenjectUiBootstrapTests
    {
        [Test]
        public void BootstrapUsesScopedZenjectDependenciesAndWindowServices()
        {
            var localCanvasObject = new GameObject("LocalCanvas", typeof(Canvas));
            var projectCanvasObject = new GameObject("ProjectCanvas", typeof(Canvas));
            var localPrefabObject = new GameObject("LocalPrefab");
            var projectPrefabObject = new GameObject("ProjectPrefab");
            var catalog = ScriptableObject.CreateInstance<WindowCatalog>();
            var bootstrap = new UiBootstrap(localCanvasObject.GetComponent<Canvas>(),
                projectCanvasObject.GetComponent<Canvas>());
            try
            {
                catalog.Add(localPrefabObject.AddComponent<ZenjectBootstrapLocalWindow>(), WindowScope.Local);
                catalog.Add(projectPrefabObject.AddComponent<ZenjectBootstrapProjectWindow>(), WindowScope.Project);
                var localToken = new ZenjectBootstrapToken("local");
                var projectToken = new ZenjectBootstrapToken("project");
                var localContainer = new DiContainer();
                var projectContainer = new DiContainer();
                localContainer.BindInstance(localToken).AsSingle();
                projectContainer.BindInstance(projectToken).AsSingle();
                bootstrap.Initialize(catalog, new ZenjectUiCompositionAdapter(projectContainer),
                    new ZenjectUiCompositionAdapter(localContainer));
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
                bootstrap.Dispose();
                Object.DestroyImmediate(catalog);
                Object.DestroyImmediate(localCanvasObject);
                Object.DestroyImmediate(projectCanvasObject);
                Object.DestroyImmediate(localPrefabObject);
                Object.DestroyImmediate(projectPrefabObject);
            }
        }
    }
}

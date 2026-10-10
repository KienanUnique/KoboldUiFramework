using KoboldUi.Adapters.VContainer;
using KoboldUi.Composition;
using NUnit.Framework;
using UnityEngine;
using VContainer;

namespace KoboldUi.Tests.VContainer
{
    public class VContainerBootstrapTests
    {
        [Test]
        public void BootstrapCreatesInjectsAndNavigatesToCatalogWindow()
        {
            var localObject = new GameObject("Local canvas", typeof(Canvas));
            var projectObject = new GameObject("Project canvas", typeof(Canvas));
            var prefabObject = new GameObject("Window prefab");
            var catalog = ScriptableObject.CreateInstance<WindowCatalog>();
            var dependency = new VContainerBootstrapTestDependency();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(dependency);
            var container = builder.Build();
            var bootstrap = new UiBootstrap(localObject.GetComponent<Canvas>(),
                projectObject.GetComponent<Canvas>());

            try
            {
                var prefab = prefabObject.AddComponent<VContainerBootstrapTestWindow>();
                catalog.Add(prefab, WindowScope.Local);

                bootstrap.Initialize(catalog, new VContainerUiCompositionAdapter(container));
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
                Object.DestroyImmediate(catalog);
                Object.DestroyImmediate(prefabObject);
                Object.DestroyImmediate(projectObject);
                Object.DestroyImmediate(localObject);
            }
        }
    }
}

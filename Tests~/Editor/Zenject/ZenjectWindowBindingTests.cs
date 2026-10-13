using KoboldUi.Composition;
using KoboldUi.Utils;
using NUnit.Framework;
using UnityEngine;
using Zenject;

namespace KoboldUi.Tests.Zenject
{
    public class ZenjectWindowBindingTests
    {
        [Test]
        public void BindWindowFromPrefabDefersInitializationUntilZenjectStarts()
        {
            var canvasObject = new GameObject("Canvas", typeof(Canvas));
            var prefabObject = new GameObject("WindowPrefab");
            try
            {
                var container = new DiContainer();
                var dependency = new ZenjectWindowBindingTestDependency();
                var prefab = prefabObject.AddComponent<ZenjectWindowBindingTestWindow>();
                container.BindWindowFromPrefab(canvasObject.GetComponent<Canvas>(), prefab);
                var window = container.Resolve<ZenjectWindowBindingTestWindow>();
                Assert.That(window, Is.Not.SameAs(prefab));
                Assert.That(window.IsInitialized, Is.False);
                Assert.That(window.Dependency, Is.Null);
                Assert.That(window.gameObject.activeInHierarchy, Is.False);
                Assert.That(container.Resolve<WindowRegistry>().Get<ZenjectWindowBindingTestWindow>(), Is.SameAs(window));
                container.BindInstance(dependency).AsSingle();
                foreach (var initializable in container.ResolveAll<IInitializable>()) initializable.Initialize();
                Assert.That(window.IsInitialized, Is.True);
                Assert.That(window.InitializeCount, Is.EqualTo(1));
                Assert.That(window.Dependency, Is.SameAs(dependency));
                Assert.That(window.Controller.FieldDependency, Is.SameAs(dependency));
                Assert.That(window.Controller.MethodDependency, Is.SameAs(dependency));
                Assert.That(window.gameObject.activeInHierarchy, Is.True);
                Assert.That(window.transform.parent, Is.SameAs(canvasObject.transform));
            }
            finally
            {
                Object.DestroyImmediate(canvasObject);
                Object.DestroyImmediate(prefabObject);
            }
        }
    }
}

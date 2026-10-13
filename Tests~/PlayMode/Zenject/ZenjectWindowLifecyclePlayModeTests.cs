using System.Collections;
using KoboldUi.Utils;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

namespace KoboldUi.Tests.Zenject
{
    public class ZenjectWindowLifecyclePlayModeTests
    {
        [UnityTest]
        public IEnumerator ZenjectWindowReceivesInjectionBeforeOnEnable()
        {
            var canvasObject = new GameObject("Canvas", typeof(Canvas));
            var prefabObject = new GameObject("WindowPrefab");
            var container = new DiContainer();
            try
            {
                var prefab = prefabObject.AddComponent<ZenjectWindowBindingTestWindow>();
                var dependency = new ZenjectWindowBindingTestDependency();
                container.BindWindowFromPrefab(canvasObject.GetComponent<Canvas>(), prefab);
                var window = container.Resolve<ZenjectWindowBindingTestWindow>();
                Assert.That(window.gameObject.activeInHierarchy, Is.False);
                Assert.That(window.EnableCount, Is.Zero);
                container.BindInstance(dependency).AsSingle();
                foreach (var initializable in container.ResolveAll<IInitializable>()) initializable.Initialize();
                yield return null;
                Assert.That(window.gameObject.activeInHierarchy, Is.True);
                Assert.That(window.EnableCount, Is.EqualTo(1));
                Assert.That(window.DependencyInjectedAtEnable, Is.True);
                Assert.That(window.Dependency, Is.SameAs(dependency));
                Assert.That(window.Controller.FieldDependency, Is.SameAs(dependency));
                Assert.That(window.Controller.MethodDependency, Is.SameAs(dependency));
            }
            finally
            {
                Object.Destroy(canvasObject);
                Object.Destroy(prefabObject);
            }
        }
    }
}

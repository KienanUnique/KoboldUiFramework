using System;
using System.Collections;
using KoboldUi.Composition;
using KoboldUi.Composition.Manual;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using KoboldUi.Windows;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace KoboldUi.Tests.PlayMode
{
    public class InjectionLifecyclePlayModeTests
    {
        [UnityTest]
        public IEnumerator CollectionPrefabReceivesDependencyBeforeAwake()
        {
            var dependency = new LifecycleDependency();
            var prefabObject = new GameObject("item prefab");
            var prefab = prefabObject.AddComponent<LifecycleItem>();
            LifecycleItem clone = null;
            try
            {
                clone = new UiActivator(new ManualUiResolver().AddInstance(dependency)).InstantiatePrefab(prefab);
                yield return null;
                Assert.That(clone.AwakeCount, Is.EqualTo(1));
                Assert.That(clone.HadDependencyInAwake, Is.True);
            }
            finally
            {
                if (clone != null) UnityEngine.Object.Destroy(clone.gameObject);
                UnityEngine.Object.Destroy(prefabObject);
            }
        }

        [UnityTest]
        public IEnumerator WindowReceivesDependencyBeforeAwake()
        {
            var dependency = new LifecycleDependency();
            var localObject = new GameObject("local", typeof(Canvas));
            var projectObject = new GameObject("project", typeof(Canvas));
            var prefabObject = new GameObject("window prefab");
            var prefab = prefabObject.AddComponent<LifecycleWindow>();
            var catalog = ScriptableObject.CreateInstance<WindowCatalog>();
            catalog.Add(prefab, WindowScope.Local);
            var ui = new UiBootstrap(localObject.GetComponent<Canvas>(), projectObject.GetComponent<Canvas>());
            try
            {
                ui.Initialize(catalog, new ManualUiResolver().AddInstance(dependency));
                yield return null;
                var window = ui.Get<LifecycleWindow>();
                Assert.That(window.AwakeCount, Is.EqualTo(1));
                Assert.That(window.HadDependencyInAwake, Is.True);
            }
            finally
            {
                ui.Dispose();
                UnityEngine.Object.Destroy(catalog);
                UnityEngine.Object.Destroy(prefabObject);
                UnityEngine.Object.Destroy(localObject);
                UnityEngine.Object.Destroy(projectObject);
            }
        }
    }

    public sealed class LifecycleDependency { }
    public sealed class LifecycleItem : MonoBehaviour
    {
        [UiInject] private LifecycleDependency _dependency;
        public int AwakeCount { get; private set; }
        public bool HadDependencyInAwake { get; private set; }
        private void Awake() { AwakeCount++; HadDependencyInAwake = _dependency != null; }
    }
    public sealed class LifecycleWindow : AWindowBase
    {
        [UiInject] private LifecycleDependency _dependency;
        public int AwakeCount { get; private set; }
        public bool HadDependencyInAwake { get; private set; }
        public override bool IsPopup => false;
        public override bool IsBackLogicIgnorable => false;
        private void Awake() { AwakeCount++; HadDependencyInAwake = _dependency != null; }
        public override IUiAction SetState(EWindowState state, in IUiActionsPool pool) { throw new NotSupportedException(); }
        public override void ApplyOrder(int order) { }
    }
}

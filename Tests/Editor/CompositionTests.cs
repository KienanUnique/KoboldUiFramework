using System;
using System.Collections.Generic;
using KoboldUi.Composition;
using KoboldUi.Composition.Manual;
using KoboldUi.Services.WindowsService;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using KoboldUi.Windows;
using NUnit.Framework;
using UnityEngine;

namespace KoboldUi.Tests
{
    public class CompositionTests
    {
        private readonly List<UnityEngine.Object> _objects = new List<UnityEngine.Object>();
        [TearDown]
        public void TearDown()
        {
            for (var i = _objects.Count - 1; i >= 0; i--)
                if (_objects[i] != null) UnityEngine.Object.DestroyImmediate(_objects[i]);
            _objects.Clear();
        }

        [Test]
        public void InjectGameObjectInjectsInactiveChildrenAndOptionalFields()
        {
            var dependency = new Dependency();
            var root = NewObject("root");
            var child = NewObject("child");
            child.transform.SetParent(root.transform);
            child.SetActive(false);
            var probe = child.AddComponent<InjectionProbe>();
            new UiActivator(new ManualUiResolver().AddInstance(dependency)).InjectGameObject(root);
            Assert.That(probe.Dependency, Is.SameAs(dependency));
            Assert.That(probe.Optional, Is.Null);
            Assert.That(probe.MethodCalls, Is.EqualTo(1));
        }

        [Test]
        public void MissingRequiredDependencyReportsWindowAndComponent()
        {
            var root = NewObject("SettingsWindow");
            root.AddComponent<InjectionProbe>();
            var error = Assert.Throws<UiActivationException>(() =>
                new UiActivator(new ManualUiResolver()).InjectGameObject(root));
            Assert.That(error.Message, Does.Contain("SettingsWindow"));
            Assert.That(error.Message, Does.Contain(nameof(InjectionProbe)));
            Assert.That(error.Message, Does.Contain(nameof(Dependency)));
        }

        [Test]
        public void InstantiatePrefabInjectsNewItem()
        {
            var dependency = new Dependency();
            var prefab = NewObject("item prefab").AddComponent<InjectionProbe>();
            var clone = new UiActivator(new ManualUiResolver().AddInstance(dependency)).InstantiatePrefab(prefab);
            _objects.Add(clone.gameObject);
            Assert.That(clone.Dependency, Is.SameAs(dependency));
            Assert.That(clone.MethodCalls, Is.EqualTo(1));
        }

        [Test]
        public void ControllerReceivesExplicitViewAndResolvedService()
        {
            var dependency = new Dependency();
            var view = new ViewToken();
            var controller = new UiActivator(new ManualUiResolver().AddInstance(dependency))
                .CreateController<ConstructorController, ViewToken>(view);
            Assert.That(controller.View, Is.SameAs(view));
            Assert.That(controller.Dependency, Is.SameAs(dependency));
        }

        [Test]
        public void ControllerUsesOptionalConstructorDefaultWhenServiceMissing()
        {
            var controller = new UiActivator(new ManualUiResolver())
                .CreateController<OptionalController, ViewToken>(new ViewToken());
            Assert.That(controller.Retries, Is.EqualTo(7));
        }

        [Test]
        public void ResolverReturningWrongTypeIsRejectedWithContext()
        {
            var root = NewObject("SettingsWindow");
            root.AddComponent<InjectionProbe>();
            var error = Assert.Throws<UiActivationException>(() =>
                new UiActivator(new WrongTypeResolver()).InjectGameObject(root));
            Assert.That(error.Message, Does.Contain(nameof(Dependency)));
            Assert.That(error.Message, Does.Contain("SettingsWindow"));
        }

        [Test]
        public void RegistryRejectsDuplicateTypesAndAmbiguousContracts()
        {
            var registry = new WindowRegistry();
            var first = NewObject("first").AddComponent<ProbeWindow>();
            var second = NewObject("second").AddComponent<ProbeWindow>();
            registry.Register(first);
            Assert.That(registry.Get<ProbeWindow>(), Is.SameAs(first));
            Assert.That(registry.Get<IWindow>(), Is.SameAs(first));
            Assert.Throws<InvalidOperationException>(() => registry.Register(second));
            Assert.Throws<InvalidOperationException>(() => registry.Get(typeof(OtherWindow)));
            registry.Register(NewObject("other").AddComponent<OtherWindow>());
            Assert.That(() => registry.Get<IWindow>(), Throws.InvalidOperationException.With.Message.Contains("Several"));
        }

        [Test]
        public void BootstrapRegistersAndInitializesWindowOnce()
        {
            var local = NewObject("local").AddComponent<Canvas>();
            var project = NewObject("project").AddComponent<Canvas>();
            var prefab = NewObject("prefab").AddComponent<ProbeWindow>();
            var catalog = ScriptableObject.CreateInstance<WindowCatalog>();
            _objects.Add(catalog);
            catalog.Add(prefab, WindowScope.Local);
            var ui = new UiBootstrap(local, project);
            try
            {
                ui.Initialize(catalog, new ManualUiResolver());
                var window = ui.Get<ProbeWindow>();
                Assert.That(window, Is.Not.SameAs(prefab));
                Assert.That(window.transform.parent, Is.SameAs(local.transform));
                Assert.That(window.Service, Is.SameAs(ui.LocalWindows));
                Assert.That(window.InitializeCount, Is.EqualTo(1));
                Assert.Throws<InvalidOperationException>(() => ui.Initialize(catalog, new ManualUiResolver()));
            }
            finally { ui.Dispose(); }
        }

        [Test]
        public void BootstrapUsesSeparateLocalAndProjectResolvers()
        {
            var local = NewObject("local").AddComponent<Canvas>();
            var project = NewObject("project").AddComponent<Canvas>();
            var catalog = ScriptableObject.CreateInstance<WindowCatalog>();
            _objects.Add(catalog);
            catalog.Add(NewObject("local prefab").AddComponent<LocalScopeWindow>(), WindowScope.Local);
            catalog.Add(NewObject("project prefab").AddComponent<ProjectScopeWindow>(), WindowScope.Project);
            var projectDependency = new Dependency();
            var localDependency = new Dependency();
            var projectResolver = new ManualUiResolver().AddInstance(projectDependency);
            var localResolver = projectResolver.CreateScope().AddInstance(localDependency);
            var ui = new UiBootstrap(local, project);
            try
            {
                ui.Initialize(catalog, projectResolver, localResolver);
                Assert.That(ui.Get<LocalScopeWindow>().Dependency, Is.SameAs(localDependency));
                Assert.That(ui.Get<ProjectScopeWindow>().Dependency, Is.SameAs(projectDependency));
            }
            finally { ui.Dispose(); }
        }

        [Test]
        public void BootstrapCanReinitializeAfterDispose()
        {
            var local = NewObject("local").AddComponent<Canvas>();
            var project = NewObject("project").AddComponent<Canvas>();
            var catalog = ScriptableObject.CreateInstance<WindowCatalog>();
            _objects.Add(catalog);
            catalog.Add(NewObject("prefab").AddComponent<ProbeWindow>(), WindowScope.Local);
            var ui = new UiBootstrap(local, project);
            try
            {
                ui.Initialize(catalog, new ManualUiResolver());
                var first = ui.Get<ProbeWindow>();
                ui.Dispose();
                ui.Initialize(catalog, new ManualUiResolver());
                Assert.That(ui.Get<ProbeWindow>(), Is.Not.SameAs(first));
            }
            finally { ui.Dispose(); }
        }

        private GameObject NewObject(string name)
        {
            var obj = new GameObject(name);
            _objects.Add(obj);
            return obj;
        }

        public sealed class Dependency { }
        public sealed class ViewToken { }
        private sealed class WrongTypeResolver : IUiResolver
        { public bool TryResolve(Type type, out object value) { value = new object(); return true; } }
        public sealed class ConstructorController
        {
            public ViewToken View { get; }
            public Dependency Dependency { get; }
            public ConstructorController(ViewToken view, Dependency dependency) { View = view; Dependency = dependency; }
        }
        public sealed class OptionalController
        {
            public int Retries { get; }
            public OptionalController(ViewToken view, int retries = 7) { Retries = retries; }
        }
        public sealed class InjectionProbe : MonoBehaviour
        {
            [UiInject] private Dependency _dependency;
            [UiInject(Optional = true)] private IDisposable _optional;
            public Dependency Dependency => _dependency;
            public IDisposable Optional => _optional;
            public int MethodCalls { get; private set; }
            [UiInject] private void Construct(Dependency dependency) { MethodCalls++; }
        }
        public sealed class ProbeWindow : AWindowBase
        {
            [UiInject] private ILocalWindowsService _service;
            public ILocalWindowsService Service => _service;
            public int InitializeCount { get; private set; }
            public override bool IsPopup => false;
            public override bool IsBackLogicIgnorable => false;
            public override void Initialize() { InitializeCount++; base.Initialize(); }
            public override IUiAction SetState(EWindowState state, in IUiActionsPool pool) { throw new NotSupportedException(); }
            public override void ApplyOrder(int order) { }
        }
        public sealed class OtherWindow : AWindowBase
        {
            public override bool IsPopup => false;
            public override bool IsBackLogicIgnorable => false;
            public override IUiAction SetState(EWindowState state, in IUiActionsPool pool) { throw new NotSupportedException(); }
            public override void ApplyOrder(int order) { }
        }
        public sealed class LocalScopeWindow : AWindowBase
        {
            [UiInject] private Dependency _dependency;
            public Dependency Dependency => _dependency;
            public override bool IsPopup => false;
            public override bool IsBackLogicIgnorable => false;
            public override IUiAction SetState(EWindowState state, in IUiActionsPool pool) { throw new NotSupportedException(); }
            public override void ApplyOrder(int order) { }
        }
        public sealed class ProjectScopeWindow : AWindowBase
        {
            [UiInject] private Dependency _dependency;
            public Dependency Dependency => _dependency;
            public override bool IsPopup => false;
            public override bool IsBackLogicIgnorable => false;
            public override IUiAction SetState(EWindowState state, in IUiActionsPool pool) { throw new NotSupportedException(); }
            public override void ApplyOrder(int order) { }
        }
    }
}

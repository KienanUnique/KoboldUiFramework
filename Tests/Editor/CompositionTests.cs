using System;
using System.Collections.Generic;
using KoboldUi.Composition;
using KoboldUi.Element.Animations.Parameters.Impl;
using KoboldUi.Services.WindowsService.Impl;
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

        [Test]
        public void BuilderRegistersWindowsAndExplicitAnimationParameters()
        {
            var canvas = NewObject("canvas").AddComponent<Canvas>();
            var prefab = NewObject("prefab").AddComponent<ProbeWindow>();
            var fade = ScriptableObject.CreateInstance<FadeAnimationParameters>();
            _objects.Add(fade);
            var ui = new UiBootstrapBuilder(canvas, canvas)
                .AddDefaultParameters(fade)
                .RegisterWindow(prefab)
                .Build();
            try
            {
                Assert.That(ui.Get<ProbeWindow>().IsInitialized, Is.True);
                Assert.That(ui.AnimationDefaults.TryGet<FadeAnimationParameters>(out var selected), Is.True);
                Assert.That(selected, Is.SameAs(fade));
            }
            finally { ui.Dispose(); }
        }

        [TearDown]
        public void TearDown()
        {
            for (var i = _objects.Count - 1; i >= 0; i--)
                if (_objects[i] != null) UnityEngine.Object.DestroyImmediate(_objects[i]);
            _objects.Clear();
        }

        [Test]
        public void RegisterWindowBuildsAllWindowsOnFirstGet()
        {
            var local = NewObject("local").AddComponent<Canvas>();
            var project = NewObject("project").AddComponent<Canvas>();
            var first = NewObject("first prefab").AddComponent<LocalWindow>();
            var second = NewObject("second prefab").AddComponent<ProjectWindow>();
            var ui = new UiBootstrap(local, project);
            try
            {
                ui.RegisterWindow(first);
                ui.RegisterWindow(second, WindowScope.Project);
                Assert.That(ui.Get<LocalWindow>(), Is.Not.SameAs(first));
                Assert.That(ui.Get<ProjectWindow>(), Is.Not.SameAs(second));
                Assert.That(ui.Get<LocalWindow>().IsInitialized, Is.True);
                Assert.That(ui.Get<ProjectWindow>().IsInitialized, Is.True);
            }
            finally { ui.Dispose(); }
        }

        [Test]
        public void RegisterWindowRejectsDuplicateAndLateRegistration()
        {
            var local = NewObject("local").AddComponent<Canvas>();
            var project = NewObject("project").AddComponent<Canvas>();
            var prefab = NewObject("prefab").AddComponent<ProbeWindow>();
            var ui = new UiBootstrap(local, project);
            try
            {
                ui.RegisterWindow(prefab);
                Assert.Throws<InvalidOperationException>(() => ui.RegisterWindow(prefab));
                ui.Get<ProbeWindow>();
                Assert.Throws<InvalidOperationException>(() => ui.RegisterWindow(
                    NewObject("late prefab").AddComponent<LocalWindow>()));
            }
            finally { ui.Dispose(); }
        }

        [Test]
        public void AccessingWindowServiceDoesNotBuildRegisteredWindows()
        {
            var local = NewObject("local").AddComponent<Canvas>();
            var project = NewObject("project").AddComponent<Canvas>();
            var prefab = NewObject("prefab").AddComponent<ProbeWindow>();
            var ui = new UiBootstrap(local, project);
            try
            {
                ui.RegisterWindow(prefab);
                var service = ui.LocalWindows;
                Assert.That(service, Is.Not.Null);
                Assert.That(local.transform.childCount, Is.Zero);
                Assert.That(ui.Get<ProbeWindow>().IsInitialized, Is.True);
            }
            finally { ui.Dispose(); }
        }

        [Test]
        public void ServiceCanBeResolvedBeforeRegistrationAndInitializesOnFirstOpen()
        {
            var local = NewObject("local").AddComponent<Canvas>();
            var project = NewObject("project").AddComponent<Canvas>();
            var prefab = NewObject("prefab").AddComponent<ProbeWindow>();
            var ui = new UiBootstrap(local, project);
            try
            {
                var service = ui.LocalWindows;
                ui.RegisterWindow(prefab);
                service.OpenWindow<ProbeWindow>();
                Assert.That(service.CurrentWindow, Is.SameAs(ui.Get<ProbeWindow>()));
                Assert.That(ui.Get<ProbeWindow>().InitializeCount, Is.EqualTo(1));
            }
            finally { ui.Dispose(); }
        }

        [Test]
        public void ManualModeExposesServiceButRequiresExplicitInitialization()
        {
            var local = NewObject("local").AddComponent<Canvas>();
            var project = NewObject("project").AddComponent<Canvas>();
            var prefab = NewObject("prefab").AddComponent<ProbeWindow>();
            var ui = new UiBootstrap(local, project, initializationMode: UiInitializationMode.Manual);
            try
            {
                var service = ui.LocalWindows;
                ui.RegisterWindow(prefab);
                Assert.Throws<InvalidOperationException>(() => service.OpenWindow<ProbeWindow>());
                ui.Initialize();
                ui.Initialize();
                service.OpenWindow<ProbeWindow>();
                Assert.That(ui.Get<ProbeWindow>().InitializeCount, Is.EqualTo(1));
            }
            finally { ui.Dispose(); }
        }

        [Test]
        public void NavigationRequestedDuringWindowInitializationRunsAfterAllWindowsAreReady()
        {
            var local = NewObject("local").AddComponent<Canvas>();
            var project = NewObject("project").AddComponent<Canvas>();
            var target = NewObject("target").AddComponent<ProbeWindow>();
            var opening = NewObject("opening").AddComponent<OpeningWindow>();
            var ui = new UiBootstrap(local, project);
            try
            {
                ui.RegisterWindow(target);
                ui.RegisterWindow(opening);
                ui.Initialize();
                Assert.That(ui.LocalWindows.CurrentWindow, Is.SameAs(ui.Get<ProbeWindow>()));
                Assert.That(ui.Get<ProbeWindow>().InitializeCount, Is.EqualTo(1));
            }
            finally { ui.Dispose(); }
        }

        [Test]
        public void BootstrapInitializesWithoutCompositionAdapter()
        {
            var local = NewObject("local").AddComponent<Canvas>();
            var project = NewObject("project").AddComponent<Canvas>();
            var prefab = NewObject("prefab").AddComponent<ProbeWindow>();
            var ui = new UiBootstrap(local, project);
            try
            {
                ui.RegisterWindow(prefab);
                Assert.That(ui.Get<ProbeWindow>(), Is.Not.SameAs(prefab));
                Assert.That(ui.Get<ProbeWindow>().IsInitialized, Is.True);
                Assert.That(ui.Get<ProbeWindow>().InitializeCount, Is.EqualTo(1));
                Assert.Throws<InvalidOperationException>(() => ui.RegisterWindow(prefab));
            }
            finally { ui.Dispose(); }
        }

        [Test]
        public void BootstrapConfiguresWindowExplicitlyBeforeInitialization()
        {
            var local = NewObject("local").AddComponent<Canvas>();
            var project = NewObject("project").AddComponent<Canvas>();
            var prefab = NewObject("prefab").AddComponent<ProbeWindow>();
            var dependency = new Dependency();
            var ui = new UiBootstrap(local, project);
            try
            {
                ui.RegisterWindow(prefab, configure: window => window.SetDependency(dependency));
                Assert.That(ui.Get<ProbeWindow>().Dependency, Is.SameAs(dependency));
                Assert.That(ui.Get<ProbeWindow>().WasInjectedBeforeInitialize, Is.True);
            }
            finally { ui.Dispose(); }
        }

        [Test]
        public void ConfigurationRunsAfterAllWindowsAreRegistered()
        {
            var local = NewObject("local").AddComponent<Canvas>();
            var project = NewObject("project").AddComponent<Canvas>();
            var localPrefab = NewObject("local prefab").AddComponent<LocalWindow>();
            var projectPrefab = NewObject("project prefab").AddComponent<ProjectWindow>();
            var ui = new UiBootstrap(local, project);
            try
            {
                ui.RegisterWindow(localPrefab, configure: _ =>
                {
                    Assert.That(ui.Get<LocalWindow>(), Is.Not.Null);
                    Assert.That(ui.Get<ProjectWindow>(), Is.Not.Null);
                });
                ui.RegisterWindow(projectPrefab, WindowScope.Project);
                ui.Get<LocalWindow>();
            }
            finally { ui.Dispose(); }
        }

        [Test]
        public void BootstrapCanBeDisposedAgainAfterConfigurationFails()
        {
            var local = NewObject("local").AddComponent<Canvas>();
            var project = NewObject("project").AddComponent<Canvas>();
            var prefab = NewObject("prefab").AddComponent<ProbeWindow>();
            var ui = new UiBootstrap(local, project);
            ui.RegisterWindow(prefab, configure: _ => throw new InvalidOperationException("Setup failed"));
            Assert.Throws<InvalidOperationException>(() =>
                ui.Get<ProbeWindow>());
            Assert.DoesNotThrow(() => ui.Dispose());
        }

        [Test]
        public void BootstrapUsesSeparateAdaptersForLocalAndProjectWindows()
        {
            var local = NewObject("local").AddComponent<Canvas>();
            var project = NewObject("project").AddComponent<Canvas>();
            var localPrefab = NewObject("local prefab").AddComponent<LocalWindow>();
            var projectPrefab = NewObject("project prefab").AddComponent<ProjectWindow>();
            var localDependency = new Dependency();
            var projectDependency = new Dependency();
            var ui = new UiBootstrap(local, project, new ProbeAdapter(projectDependency),
                new ProbeAdapter(localDependency), null);
            try
            {
                ui.RegisterWindow(localPrefab);
                ui.RegisterWindow(projectPrefab, WindowScope.Project);
                Assert.That(ui.Get<LocalWindow>().Dependency, Is.SameAs(localDependency));
                Assert.That(ui.Get<ProjectWindow>().Dependency, Is.SameAs(projectDependency));
            }
            finally { ui.Dispose(); }
        }

        [Test]
        public void AdapterInjectsInactiveChildBeforeWindowInitialization()
        {
            var local = NewObject("local").AddComponent<Canvas>();
            var project = NewObject("project").AddComponent<Canvas>();
            var prefab = NewObject("prefab").AddComponent<ProbeWindow>();
            var child = NewObject("child");
            child.transform.SetParent(prefab.transform);
            child.SetActive(false);
            child.AddComponent<ProbeComponent>();
            var dependency = new Dependency();
            var ui = new UiBootstrap(local, project, new ProbeAdapter(dependency));
            try
            {
                ui.RegisterWindow(prefab);
                var instance = ui.Get<ProbeWindow>();
                Assert.That(instance.GetComponentInChildren<ProbeComponent>(true).Dependency, Is.SameAs(dependency));
                Assert.That(instance.WasInjectedBeforeInitialize, Is.True);
            }
            finally { ui.Dispose(); }
        }

        [Test]
        public void AdapterInjectsWindowBeforeConfigurationCallback()
        {
            var local = NewObject("local").AddComponent<Canvas>();
            var project = NewObject("project").AddComponent<Canvas>();
            var prefab = NewObject("prefab").AddComponent<ProbeWindow>();
            var dependency = new Dependency();
            var ui = new UiBootstrap(local, project, new ProbeAdapter(dependency));
            try
            {
                ui.RegisterWindow(prefab, configure: window =>
                    Assert.That(window.Dependency, Is.SameAs(dependency)));
                ui.Get<ProbeWindow>();
            }
            finally { ui.Dispose(); }
        }

        [Test]
        public void ExplicitFactoryCreatesControllerWithoutAdapter()
        {
            var view = new ViewToken();
            var dependency = new Dependency();
            var controller = new UiActivator().CreateController(view, v => new ConstructorController(v, dependency));
            Assert.That(controller.View, Is.SameAs(view));
            Assert.That(controller.Dependency, Is.SameAs(dependency));
        }

        [Test]
        public void ParameterlessControllerIsCreatedWithoutAdapter()
        {
            var view = new ViewToken();
            var controller = new UiActivator().CreateController<ParameterlessController, ViewToken>(view);
            Assert.That(controller.View, Is.SameAs(view));
        }

        [Test]
        public void AdapterCreatesControllerWithExplicitViewAndDependency()
        {
            var view = new ViewToken();
            var dependency = new Dependency();
            var controller = new UiActivator(new ProbeAdapter(dependency))
                .CreateController<ConstructorController, ViewToken>(view);
            Assert.That(controller.View, Is.SameAs(view));
            Assert.That(controller.Dependency, Is.SameAs(dependency));
        }

        [Test]
        public void AdapterReceivesWindowServicesAsConstructorArguments()
        {
            var view = new ViewToken();
            var localWindows = new LocalWindowsService(new WindowRegistry());
            var projectWindows = new ProjectWindowsService(new WindowRegistry());
            var adapter = new CaptureAdapter();
            try
            {
                new UiActivator(adapter, localWindows: localWindows, projectWindows: projectWindows)
                    .CreateController<ParameterlessController, ViewToken>(view);
                Assert.That(adapter.Arguments, Is.EqualTo(new object[] { view, localWindows, projectWindows }));
            }
            finally
            {
                localWindows.Dispose();
                projectWindows.Dispose();
            }
        }

        [Test]
        public void ControllerWithoutDefaultConstructorReportsError()
        {
            Assert.That(() => new UiActivator().CreateController<ConstructorController, ViewToken>(new ViewToken()),
                Throws.TypeOf<UiActivationException>().With.Message.Contains("parameterless constructor"));
        }

        [Test]
        public void BootstrapRejectsUseAfterDispose()
        {
            var local = NewObject("local").AddComponent<Canvas>();
            var project = NewObject("project").AddComponent<Canvas>();
            var prefab = NewObject("prefab").AddComponent<ProbeWindow>();
            var ui = new UiBootstrap(local, project);
            try
            {
                ui.RegisterWindow(prefab);
                ui.Get<ProbeWindow>();
                ui.Dispose();
                Assert.Throws<ObjectDisposedException>(() => ui.Get<ProbeWindow>());
                Assert.Throws<ObjectDisposedException>(() => ui.RegisterWindow(prefab));
            }
            finally { ui.Dispose(); }
        }

        private GameObject NewObject(string name)
        {
            var result = new GameObject(name);
            _objects.Add(result);
            return result;
        }

        public sealed class Dependency { }
        public sealed class ViewToken { }

        public sealed class ParameterlessController : KoboldUi.Element.Controller.IUiViewReceiver<ViewToken>
        {
            public ViewToken View { get; private set; }
            public void SetView(ViewToken view) { View = view; }
        }

        public sealed class ConstructorController
        {
            public ViewToken View { get; }
            public Dependency Dependency { get; }
            public ConstructorController(ViewToken view, Dependency dependency)
            { View = view; Dependency = dependency; }
        }

        private sealed class ProbeAdapter : IUiCompositionAdapter
        {
            private readonly Dependency _dependency;
            public ProbeAdapter(Dependency dependency) { _dependency = dependency; }
            public void Inject(object instance)
            {
                if (instance is ProbeWindow window) window.SetDependency(_dependency);
                if (instance is LocalWindow local) local.SetDependency(_dependency);
                if (instance is ProjectWindow project) project.SetDependency(_dependency);
                if (instance is ProbeComponent component) component.SetDependency(_dependency);
            }
            public T Create<T>(params object[] arguments) where T : class
            {
                if (typeof(T) == typeof(ConstructorController))
                    return new ConstructorController((ViewToken)arguments[0], _dependency) as T;
                throw new NotSupportedException(typeof(T).Name);
            }
        }

        private sealed class CaptureAdapter : IUiCompositionAdapter
        {
            public object[] Arguments { get; private set; }
            public void Inject(object instance) { }
            public T Create<T>(params object[] arguments) where T : class
            {
                Arguments = arguments;
                return new ParameterlessController() as T;
            }
        }

        public sealed class ProbeComponent : MonoBehaviour
        {
            public Dependency Dependency { get; private set; }
            public void SetDependency(Dependency dependency) { Dependency = dependency; }
        }

        public sealed class ProbeWindow : AWindowBase
        {
            public Dependency Dependency { get; private set; }
            public bool WasInjectedBeforeInitialize { get; private set; }
            public int InitializeCount { get; private set; }
            public void SetDependency(Dependency dependency) { Dependency = dependency; }
            public override bool IsPopup => false;
            public override bool IsBackLogicIgnorable => false;
            public override void Initialize()
            {
                WasInjectedBeforeInitialize = Dependency != null;
                InitializeCount++;
                base.Initialize();
            }
            public override IUiAction SetState(EWindowState state, in IUiActionsPool pool)
            {
                pool.GetAction(out KoboldUi.UiAction.Impl.Common.EmptyAction action);
                return action;
            }
            public override void ApplyOrder(int order) { }
        }

        public sealed class OpeningWindow : AWindowBase
        {
            public override bool IsPopup => false;
            public override bool IsBackLogicIgnorable => false;
            public override void Initialize()
            {
                Activator.LocalWindows.OpenWindow<ProbeWindow>();
                base.Initialize();
            }
            public override IUiAction SetState(EWindowState state, in IUiActionsPool pool) =>
                throw new NotSupportedException();
            public override void ApplyOrder(int order) { }
        }

        public sealed class LocalWindow : AWindowBase
        {
            public Dependency Dependency { get; private set; }
            public void SetDependency(Dependency dependency) { Dependency = dependency; }
            public override bool IsPopup => false;
            public override bool IsBackLogicIgnorable => false;
            public override IUiAction SetState(EWindowState state, in IUiActionsPool pool) => throw new NotSupportedException();
            public override void ApplyOrder(int order) { }
        }

        public sealed class ProjectWindow : AWindowBase
        {
            public Dependency Dependency { get; private set; }
            public void SetDependency(Dependency dependency) { Dependency = dependency; }
            public override bool IsPopup => false;
            public override bool IsBackLogicIgnorable => false;
            public override IUiAction SetState(EWindowState state, in IUiActionsPool pool) => throw new NotSupportedException();
            public override void ApplyOrder(int order) { }
        }
    }
}

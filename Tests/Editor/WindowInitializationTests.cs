using System.Collections.Generic;
using System.Reflection;
using KoboldUi.Composition;
using KoboldUi.Element.Controller;
using KoboldUi.Element.View;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
using KoboldUi.UiAction.Pool.Impl;
using KoboldUi.Utils;
using KoboldUi.Windows;
using KoboldUi.WindowsStack.Impl;
using NUnit.Framework;
using UnityEngine;

namespace KoboldUi.Tests
{
    public class WindowInitializationTests
    {
        private GameObject _root;

        [TearDown]
        public void TearDown()
        {
            if (_root != null)
                Object.DestroyImmediate(_root);
        }

        [Test]
        public void WindowCreatesParameterlessControllerWithoutAdapter()
        {
            _root = new GameObject("Simple window");
            var view = _root.AddComponent<RecordingView>();
            _root.AddComponent<ParameterlessControllerProbe>();
            var window = _root.AddComponent<ParameterlessTestWindow>();
            window.ViewComponent = view;
            typeof(AWindow)
                .GetField("_animatedEmptyViews", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(window, new List<KoboldUi.Element.View.Impl.AnimatedEmptyView>());

            window.Configure(new UiActivator());
            window.Initialize();

            Assert.That(window.Controller, Is.Not.Null);
            Assert.That(window.Controller.WasInitializedWithView, Is.True);
            Assert.That(view.WasClosedInstantly, Is.True);
        }

        [Test]
        public void InitializeInjectsWindowAndControllerBeforeTheirInitialization()
        {
            var dependency = new TestDependency();

            _root = new GameObject("Test window");
            var view = _root.AddComponent<RecordingView>();
            var window = _root.AddComponent<RecordingTestWindow>();
            window.ViewComponent = view;
            typeof(AWindow)
                .GetField("_animatedEmptyViews", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(window, new List<KoboldUi.Element.View.Impl.AnimatedEmptyView>());

            var activator = new UiActivator(new TestAdapter(dependency));
            window.Configure(activator);
            activator.PrepareGameObject(_root);
            window.Initialize();

            Assert.That(window.IsInitialized, Is.True);
            Assert.That(window.InjectedDependency, Is.SameAs(dependency));
            Assert.That(view.WasInitialized, Is.True);
            Assert.That(window.Controller, Is.Not.Null);
            Assert.That(window.Controller.WasInitializedWithView, Is.True);
            Assert.That(window.Controller.Dependency, Is.SameAs(dependency));
            Assert.That(view.WasClosedInstantly, Is.True);
        }

        [Test]
        public void ControllerTransitionsUseInjectedView()
        {
            var dependency = new TestDependency();

            _root = new GameObject("Test window");
            var view = _root.AddComponent<RecordingView>();
            var window = _root.AddComponent<RecordingTestWindow>();
            window.ViewComponent = view;
            typeof(AWindow)
                .GetField("_animatedEmptyViews", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(window, new List<KoboldUi.Element.View.Impl.AnimatedEmptyView>());
            var activator = new UiActivator(new TestAdapter(dependency));
            window.Configure(activator);
            activator.PrepareGameObject(_root);
            window.Initialize();

            var pool = new UiActionsPool(new WindowsStackHolder());
            pool.Initialize();
            try
            {
                window.Controller.SetState(EWindowState.Active, pool).Start().GetAwaiter().GetResult();
                window.Controller.SetState(EWindowState.NonFocused, pool).Start().GetAwaiter().GetResult();
                window.Controller.SetState(EWindowState.Active, pool).Start().GetAwaiter().GetResult();
                window.Controller.SetState(EWindowState.Closed, pool).Start().GetAwaiter().GetResult();

                Assert.That(view.Transitions, Is.EqualTo(new[]
                {
                    "Open", "RemoveFocus", "ReturnFocus", "Close"
                }));
            }
            finally
            {
                pool.Dispose();
            }
        }

        [Test]
        public void InstantiateWindowFromPrefabCreatesAndRegistersChildOfCanvas()
        {
            var canvasObject = new GameObject("Canvas");
            var prefabObject = new GameObject("Window prefab");
            RecordingTestWindow instance = null;

            try
            {
                var canvas = canvasObject.AddComponent<Canvas>();
                var prefab = prefabObject.AddComponent<RecordingTestWindow>();
                var dependency = new TestDependency();
                var activator = new UiActivator(new TestAdapter(dependency));
                var registry = new WindowRegistry();

                instance = Object.Instantiate(prefab, canvas.transform);
                instance.Configure(activator);
                activator.PrepareGameObject(instance.gameObject);
                registry.Register(instance);

                Assert.That(instance, Is.Not.SameAs(prefab));
                Assert.That(instance.transform.parent, Is.SameAs(canvas.transform));
                Assert.That(instance.InjectedDependency, Is.SameAs(dependency));
                Assert.That(registry.Get<RecordingTestWindow>(), Is.SameAs(instance));
            }
            finally
            {
                if (instance != null)
                    Object.DestroyImmediate(instance.gameObject);
                Object.DestroyImmediate(prefabObject);
                Object.DestroyImmediate(canvasObject);
            }
        }

        [Test]
        public void WindowStateControlsCanvasInteractivityAndViewTransitions()
        {
            var dependency = new TestDependency();
            _root = new GameObject("Test window");
            var view = _root.AddComponent<RecordingView>();
            var window = _root.AddComponent<RecordingTestWindow>();
            window.ViewComponent = view;
            typeof(AWindow)
                .GetField("_animatedEmptyViews", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(window, new List<KoboldUi.Element.View.Impl.AnimatedEmptyView>());
            var activator = new UiActivator(new TestAdapter(dependency));
            window.Configure(activator);
            activator.PrepareGameObject(_root);
            window.Initialize();
            typeof(AWindow)
                .GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(window, null);

            var pool = new UiActionsPool(new WindowsStackHolder());
            pool.Initialize();
            try
            {
                window.SetState(EWindowState.Active, pool).Start().GetAwaiter().GetResult();
                Assert.That(_root.GetComponent<CanvasGroup>().interactable, Is.True);

                window.SetState(EWindowState.Closed, pool).Start().GetAwaiter().GetResult();
                Assert.That(_root.GetComponent<CanvasGroup>().interactable, Is.False);
                Assert.That(view.Transitions, Is.EqualTo(new[] { "Open", "Close" }));
            }
            finally
            {
                pool.Dispose();
            }
        }
    }

    public sealed class TestDependency
    {
    }

    public sealed class ParameterlessTestWindow : AWindow
    {
        public RecordingView ViewComponent { get; set; }
        public ParameterlessRecordingController Controller { get; private set; }

        protected override void AddControllers()
        {
            AddController<ParameterlessRecordingController, RecordingView>(ViewComponent);
            Controller = ViewComponent.GetComponent<ParameterlessControllerProbe>().Controller;
        }
    }

    public sealed class ParameterlessRecordingController : AUiController<RecordingView>
    {
        public bool WasInitializedWithView { get; private set; }
        public override void Initialize()
        {
            WasInitializedWithView = View != null && View.WasInitialized;
            View.GetComponent<ParameterlessControllerProbe>().Controller = this;
        }
    }

    public sealed class ParameterlessControllerProbe : MonoBehaviour
    {
        public ParameterlessRecordingController Controller { get; set; }
    }

    public sealed class TestAdapter : IUiCompositionAdapter
    {
        private readonly TestDependency _dependency;
        public TestAdapter(TestDependency dependency) { _dependency = dependency; }
        public void Inject(object instance)
        {
            if (instance is RecordingTestWindow window) window.SetDependency(_dependency);
        }
        public T Create<T>(params object[] arguments) where T : class =>
            throw new System.NotSupportedException(typeof(T).Name);
    }

    public sealed class RecordingTestWindow : AWindow
    {
        private TestDependency _dependency;
        public void SetDependency(TestDependency dependency) { _dependency = dependency; }

        public RecordingView ViewComponent { get; set; }
        public RecordingController Controller { get; private set; }
        public TestDependency InjectedDependency => _dependency;

        protected override void AddControllers()
        {
            AddController(ViewComponent, _ => new RecordingController(_dependency));
            // AWindow owns its controller list; capture the instance through the view's initialization hook.
            Controller = ViewComponent.Controller;
        }
    }

    public sealed class RecordingController : AUiController<RecordingView>
    {
        public RecordingController(TestDependency dependency)
        {
            Dependency = dependency;
        }

        public TestDependency Dependency { get; }
        public bool WasInitializedWithView { get; private set; }

        public override void Initialize()
        {
            WasInitializedWithView = View != null && View.WasInitialized;
            View.Controller = this;
        }
    }

    public sealed class RecordingView : AUiView
    {
        public RecordingController Controller { get; set; }
        public bool WasInitialized { get; private set; }
        public bool WasClosedInstantly { get; private set; }
        public List<string> Transitions { get; } = new List<string>();

        public override void Initialize()
        {
            WasInitialized = true;
        }

        public override void CloseInstantly()
        {
            WasClosedInstantly = true;
        }

        protected override IUiAction OnOpen(in IUiActionsPool pool)
        {
            Transitions.Add("Open");
            return base.OnOpen(pool);
        }

        protected override IUiAction OnReturnFocus(in IUiActionsPool pool)
        {
            Transitions.Add("ReturnFocus");
            return base.OnReturnFocus(pool);
        }

        protected override IUiAction OnRemoveFocus(in IUiActionsPool pool)
        {
            Transitions.Add("RemoveFocus");
            return base.OnRemoveFocus(pool);
        }

        protected override IUiAction OnClose(in IUiActionsPool pool)
        {
            Transitions.Add("Close");
            return base.OnClose(pool);
        }
    }
}

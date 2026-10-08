using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using KoboldUi.Element.Animations.Impl;
using KoboldUi.Element.Animations.Parameters.Impl;
using KoboldUi.Element.Controller;
using KoboldUi.Element.View;
using KoboldUi.UiAction.Pool.Impl;
using KoboldUi.Utils;
using KoboldUi.Windows;
using KoboldUi.WindowsStack.Impl;
using NUnit.Framework;
using UniRx;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Zenject;

namespace KoboldUi.Tests.PlayMode
{
    public class WindowPrefabPlayModeTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private readonly List<ScriptableObject> _assets = new List<ScriptableObject>();

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            foreach (var item in _objects)
            {
                if (item != null)
                    Object.Destroy(item);
            }

            foreach (var asset in _assets)
            {
                if (asset != null)
                    Object.Destroy(asset);
            }

            _objects.Clear();
            _assets.Clear();
            yield return null;
        }

        [Test]
        public void FadeAnimationAppearsAndDisappearsWithInjectedDefaults()
        {
            var parameters = ScriptableObject.CreateInstance<FadeAnimationParameters>();
            _assets.Add(parameters);
            var animationObject = new GameObject("Fade animation");
            _objects.Add(animationObject);
            var animation = animationObject.AddComponent<FadeAnimation>();
            var container = new DiContainer();
            container.BindInstance(parameters).AsSingle();
            container.InjectGameObject(animationObject);
            var pool = new UiActionsPool(new WindowsStackHolder());
            pool.Initialize();

            try
            {
                animation.Appear(pool).Start().GetAwaiter().GetResult();
                DOTween.CompleteAll();
                Assert.That(animationObject.GetComponent<CanvasGroup>().alpha, Is.EqualTo(1f).Within(0.001f));

                animation.Disappear(pool).Start().GetAwaiter().GetResult();
                DOTween.CompleteAll();
                Assert.That(animationObject.activeSelf, Is.False);
            }
            finally
            {
                DOTween.KillAll();
                pool.Dispose();
            }
        }

        [UnityTest]
        public IEnumerator PrefabWindowInjectsControllerAndHandlesButtonClick()
        {
            var service = new ClickService();
            var container = new DiContainer();
            container.BindInstance(service).AsSingle();

            var canvasObject = new GameObject("Canvas", typeof(Canvas));
            _objects.Add(canvasObject);
            var canvas = canvasObject.GetComponent<Canvas>();

            var prefabObject = new GameObject("Window prefab", typeof(RectTransform));
            _objects.Add(prefabObject);
            var prefab = prefabObject.AddComponent<ButtonWindow>();
            var view = prefabObject.AddComponent<ButtonView>();
            var buttonObject = new GameObject("Button", typeof(RectTransform), typeof(Button));
            _objects.Add(buttonObject);
            buttonObject.transform.SetParent(prefabObject.transform);
            view.Configure(buttonObject.GetComponent<Button>());
            prefab.Configure(view);
            typeof(AWindow)
                .GetField("_animatedEmptyViews", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(prefab, new List<KoboldUi.Element.View.Impl.AnimatedEmptyView>());

            container.BindWindowFromPrefab(canvas, prefab);
            var instance = container.Resolve<ButtonWindow>();
            _objects.Add(instance.gameObject);
            instance.Initialize();

            Assert.That(instance.IsInitialized, Is.True);
            Assert.That(instance.InjectedService, Is.SameAs(service));
            Assert.That(instance.Controller, Is.Not.Null);
            Assert.That(instance.Controller.Service, Is.SameAs(service));

            instance.View.Button.onClick.Invoke();
            Assert.That(service.ClickCount, Is.EqualTo(1));
            yield return null;
        }
    }

    public sealed class ClickService
    {
        public int ClickCount { get; private set; }

        public void RecordClick()
        {
            ClickCount++;
        }
    }

    public sealed class ButtonWindow : AWindow
    {
        [SerializeField] private ButtonView view;
        [Inject] private ClickService _service;

        public ButtonView View => view;
        public ButtonController Controller => view.Controller;
        public ClickService InjectedService => _service;

        public void Configure(ButtonView buttonView)
        {
            view = buttonView;
        }

        protected override void AddControllers()
        {
            AddController<ButtonController, ButtonView>(view);
        }
    }

    public sealed class ButtonView : AUiView
    {
        [SerializeField] private Button button;

        public Button Button => button;
        public ButtonController Controller { get; set; }

        public void Configure(Button targetButton)
        {
            button = targetButton;
        }

        public override void CloseInstantly()
        {
        }
    }

    public sealed class ButtonController : AUiController<ButtonView>
    {
        public ButtonController(ClickService service)
        {
            Service = service;
        }

        public ClickService Service { get; }

        public override void Initialize()
        {
            View.Controller = this;
            View.Button.OnClickAsObservable().Subscribe(_ => Service.RecordClick()).AddTo(View);
        }
    }
}

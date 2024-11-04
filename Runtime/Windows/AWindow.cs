using System.Collections.Generic;
using KoboldUi.Element.Controller;
using KoboldUi.Element.Controller.Impl;
using KoboldUi.Element.View;
using KoboldUi.Element.View.Impl;
using KoboldUi.Utils;
using UnityEngine;
using Zenject;

namespace KoboldUi.Windows
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class AWindow : AWindowBase
    {
        [SerializeField] private List<AnimatedEmptyView> animatedEmptyViews;

        private readonly List<IUIController> _childControllers = new();

        private DiContainer _container;
        private CanvasGroup _canvasGroup;

        public sealed override void InstallBindings(DiContainer container)
        {
            _container = container;
        }

        public sealed override void Initialize()
        {
            AddControllers();
            AddEmptyElements();
            base.Initialize();
        }

        public override void SetState(EWindowState state)
        {
            switch (state)
            {
                case EWindowState.Active:
                    _canvasGroup.interactable = true;
                    break;
                case EWindowState.Closed:
                case EWindowState.NonFocused:
                    _canvasGroup.interactable = false;
                    break;
            }

            foreach (var controller in _childControllers)
            {
                controller.SetState(state);
            }
        }

        public sealed override void ApplyOrder(int order)
        {
            transform.SetSiblingIndex(order);
        }

        protected abstract void AddControllers();

        protected void AddController<TController, TView>(TView viewInstance)
            where TView : IUiView
            where TController : AUiController<TView>
        {
            if (viewInstance is AUiAnimatedView viewAsAnimatedView)
            {
                foreach (var aUiAnimationBase in viewAsAnimatedView.AnimationsForInjecting)
                {
                    _container.Inject(aUiAnimationBase);
                }
            }

            var controller = _container.Instantiate<TController>(new List<object> {viewInstance});

            _childControllers.Add(controller);
            controller.Initialize();
            controller.CloseInstantly();
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.interactable = false;
        }

        private void AddEmptyElements()
        {
            foreach (var animatedEmptyView in animatedEmptyViews)
                AddController<AnimatedEmptyController, AnimatedEmptyView>(animatedEmptyView);
        }
    }
}
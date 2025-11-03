using System.Collections.Generic;
using KoboldUi.Element.Controller;
using KoboldUi.Element.Controller.Impl;
using KoboldUi.Element.View;
using KoboldUi.Element.View.Impl;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using UnityEngine;
using Zenject;

#if KOBOLD_ALCHEMY_SUPPORT
using Alchemy.Inspector;
using UnityEditor;
#endif

namespace KoboldUi.Windows
{
    /// <summary>
    /// Base implementation for windows composed of controllers and views.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class AWindow : AWindowBase, IAutoFillable
    {
        [Header("Behaviour")]
        /// <summary>
        /// Indicates whether the window should be treated as a popup.
        /// </summary>
        [SerializeField] public bool _isPopup;
        /// <summary>
        /// Indicates whether back navigation logic may ignore this window.
        /// </summary>
        [SerializeField] public bool _isBackLogicIgnorable;
        
        [Space]
        [Header("Views")]
        [SerializeField] private List<AnimatedEmptyView> _animatedEmptyViews;

        private readonly List<IUIController> _childControllers = new();
        private CanvasGroup _canvasGroup;

        private DiContainer _container;
        
        /// <inheritdoc />
        public override bool IsPopup => _isPopup;
        /// <inheritdoc />
        public override bool IsBackLogicIgnorable => _isBackLogicIgnorable;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.interactable = false;
        }

        /// <inheritdoc />
        public sealed override void InstallBindings(DiContainer container)
        {
            _container = container;
        }

        /// <inheritdoc />
        public sealed override void Initialize()
        {
            AddControllers();
            AddEmptyElements();
            base.Initialize();
        }

        /// <inheritdoc />
        public override IUiAction SetState(EWindowState state, in IUiActionsPool pool)
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

            var actions = new List<IUiAction>();

            if (!IsInitialized)
                Debug.LogError(
                    $"[Kobold Ui {nameof(AWindow)}] | {gameObject.name} is not initialized. Change State logic is invalid!");

            foreach (var controller in _childControllers)
                actions.Add(controller.SetState(state, pool));

            pool.GetAction(out var parallelAction, actions);

            return parallelAction;
        }

        /// <inheritdoc />
        public sealed override void ApplyOrder(int order)
        {
            transform.SetSiblingIndex(order);
        }

        protected abstract void AddControllers();

        /// <summary>
        /// Adds a controller bound to the specified view instance.
        /// </summary>
        /// <typeparam name="TController">Controller type to instantiate.</typeparam>
        /// <typeparam name="TView">View type managed by the controller.</typeparam>
        /// <param name="viewInstance">View instance associated with the controller.</param>
        protected void AddController<TController, TView>(TView viewInstance)
            where TView : IUiView
            where TController : AUiController<TView>
        {
            var controller = _container.Instantiate<TController>(new List<object> {viewInstance});

            _container.InjectGameObject(gameObject);

            _childControllers.Add(controller);

            viewInstance.Initialize();
            controller.Initialize();
            controller.CloseInstantly();
        }

        private void AddEmptyElements()
        {
            foreach (var animatedEmptyView in _animatedEmptyViews)
                AddController<AnimatedEmptyController, AnimatedEmptyView>(animatedEmptyView);
        }

#if KOBOLD_ALCHEMY_SUPPORT && UNITY_EDITOR
        [Button]
        /// <inheritdoc />
        public void AutoFill()
        {
            _animatedEmptyViews = new List<AnimatedEmptyView>();
            var animatedEmptyViews = GetComponentsInChildren<AnimatedEmptyView>();
            _animatedEmptyViews.AddRange(animatedEmptyViews);
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
#endif
    }
}
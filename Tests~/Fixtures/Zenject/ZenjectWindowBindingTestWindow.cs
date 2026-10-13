using System;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using KoboldUi.Windows;
using Zenject;

namespace KoboldUi.Tests.Zenject
{
    public sealed class ZenjectWindowBindingTestWindow : AWindowBase
    {
        [Inject] public ZenjectWindowBindingTestDependency Dependency;
        public ZenjectWindowBindingController Controller { get; private set; }
        public int InitializeCount { get; private set; }
        public int EnableCount { get; private set; }
        public bool DependencyInjectedAtEnable { get; private set; }
        public override bool IsPopup => false;
        public override bool IsBackLogicIgnorable => false;

        public override void Initialize()
        {
            InitializeCount++;
            Controller = Activator.CreateController<ZenjectWindowBindingController, object>(new object());
            base.Initialize();
        }

        private void OnEnable()
        {
            EnableCount++;
            DependencyInjectedAtEnable = Dependency != null;
        }

        public override IUiAction SetState(EWindowState state, in IUiActionsPool pool)
        {
            throw new NotSupportedException();
        }

        public override void ApplyOrder(int order) { }
    }

    public sealed class ZenjectWindowBindingTestDependency { }

    public sealed class ZenjectWindowBindingController
    {
        [Inject] public ZenjectWindowBindingTestDependency FieldDependency;
        public ZenjectWindowBindingTestDependency MethodDependency { get; private set; }
        public ZenjectWindowBindingController(object view) { }

        [Inject]
        public void SetDependency(ZenjectWindowBindingTestDependency dependency)
        {
            MethodDependency = dependency;
        }
    }
}

using VContainer;
using KoboldUi.Services.WindowsService;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Impl.Common;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using KoboldUi.Windows;

namespace KoboldUi.Tests.VContainer
{
    public sealed class VContainerBootstrapTestWindow : AWindowBase
    {
        [Inject] private VContainerBootstrapTestDependency _dependency;

        public VContainerBootstrapTestDependency Dependency => _dependency;
        public ILocalWindowsService LocalWindows => Activator.LocalWindows;
        public int InitializeCount { get; private set; }
        public EWindowState? LastState { get; private set; }
        public override bool IsPopup => false;
        public override bool IsBackLogicIgnorable => false;

        public override void Initialize()
        {
            InitializeCount++;
            base.Initialize();
        }

        public override IUiAction SetState(EWindowState state, in IUiActionsPool pool)
        {
            LastState = state;
            pool.GetAction(out EmptyAction action);
            return action;
        }

        public override void ApplyOrder(int order)
        {
        }
    }

    public sealed class VContainerBootstrapTestDependency
    {
    }
}

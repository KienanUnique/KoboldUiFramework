using KoboldUi.Composition;
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
        [UiInject] private VContainerBootstrapTestDependency _dependency;
        [UiInject] private ILocalWindowsService _localWindows;

        public VContainerBootstrapTestDependency Dependency => _dependency;
        public ILocalWindowsService LocalWindows => _localWindows;
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

using KoboldUi.Services.WindowsService;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Impl.Common;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using KoboldUi.Windows;

namespace KoboldUi.Tests.Zenject
{
    public sealed class ZenjectProjectNavigationWindow : AWindowBase
    {
        public ZenjectProjectNavigationController Controller { get; private set; }
        public override bool IsPopup => false;
        public override bool IsBackLogicIgnorable => false;

        public override void Initialize()
        {
            Controller = Activator.CreateController<ZenjectProjectNavigationController,
                ZenjectProjectNavigationView>(new ZenjectProjectNavigationView());
            base.Initialize();
        }

        public override IUiAction SetState(EWindowState state, in IUiActionsPool pool)
        {
            pool.GetAction(out EmptyAction action);
            return action;
        }

        public override void ApplyOrder(int order) { }
    }

    public sealed class ZenjectProjectNavigationView { }

    public sealed class ZenjectProjectNavigationTargetWindow : AWindowBase
    {
        public override bool IsPopup => false;
        public override bool IsBackLogicIgnorable => false;

        public override IUiAction SetState(EWindowState state, in IUiActionsPool pool)
        {
            pool.GetAction(out EmptyAction action);
            return action;
        }

        public override void ApplyOrder(int order) { }
    }

    public sealed class ZenjectProjectNavigationController
    {
        public IProjectWindowsService ProjectWindows { get; }

        public ZenjectProjectNavigationController(ZenjectProjectNavigationView view,
            IProjectWindowsService projectWindows)
        {
            ProjectWindows = projectWindows;
        }

        public void OpenProjectWindow()
        {
            ProjectWindows.OpenWindow<ZenjectProjectNavigationTargetWindow>();
        }
    }
}

using KoboldUi.Composition;
using KoboldUi.Services.WindowsService;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Impl.Common;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using KoboldUi.Windows;

namespace KoboldUi.Tests.Zenject
{
    public abstract class ZenjectBootstrapWindow : AWindowBase
    {
        [UiInject] public ZenjectBootstrapToken Token;
        public ZenjectBootstrapController Controller { get; private set; }
        public override bool IsPopup => false;
        public override bool IsBackLogicIgnorable => false;

        public override void Initialize()
        {
            Controller = Activator.CreateController<ZenjectBootstrapController, object>(new object());
            base.Initialize();
        }

        public override IUiAction SetState(EWindowState state, in IUiActionsPool pool)
        {
            pool.GetAction(out EmptyAction action);
            return action;
        }

        public override void ApplyOrder(int order) { }
    }

    public sealed class ZenjectBootstrapToken
    {
        public readonly string Scope;
        public ZenjectBootstrapToken(string scope) { Scope = scope; }
    }

    public sealed class ZenjectBootstrapController
    {
        public readonly ZenjectBootstrapToken Token;
        public readonly ILocalWindowsService LocalWindows;

        public ZenjectBootstrapController(object view, ZenjectBootstrapToken token,
            ILocalWindowsService localWindows)
        {
            Token = token;
            LocalWindows = localWindows;
        }
    }
}

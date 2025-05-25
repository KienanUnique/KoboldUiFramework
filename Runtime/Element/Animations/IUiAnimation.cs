using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;

namespace KoboldUi.Element.Animations
{
    public interface IUiAnimation
    {
        IUiAction Appear(in IUiActionsPool pool, bool needWaitAnimation);
        IUiAction AnimateFocusReturn(in IUiActionsPool pool);
        IUiAction AnimateFocusRemoved(in IUiActionsPool pool);
        IUiAction Disappear(in IUiActionsPool pool, bool needWaitAnimation);
    }
}
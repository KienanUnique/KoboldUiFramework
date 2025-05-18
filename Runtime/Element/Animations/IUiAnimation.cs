using KoboldUi.UiAction;

namespace KoboldUi.Element.Animations
{
    public interface IUiAnimation
    {
        IUiAction Appear();
        IUiAction AnimateFocusReturn();
        IUiAction AnimateFocusRemoved();
        IUiAction Disappear();
    }
}
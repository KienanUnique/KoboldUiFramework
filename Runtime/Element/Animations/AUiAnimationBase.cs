using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
using UnityEngine;

namespace KoboldUi.Element.Animations
{
    public abstract class AUiAnimationBase : MonoBehaviour, IUiAnimation
    {
        public abstract IUiAction Appear(in IUiActionsPool pool, bool needWaitAnimation);
        public abstract IUiAction AnimateFocusReturn(in IUiActionsPool pool);
        public abstract IUiAction AnimateFocusRemoved(in IUiActionsPool pool);
        public abstract IUiAction Disappear(in IUiActionsPool pool, bool needWaitAnimation);
        public abstract void DisappearInstantly();
    }
}
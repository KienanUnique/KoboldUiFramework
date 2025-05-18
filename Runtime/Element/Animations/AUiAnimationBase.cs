using KoboldUi.UiAction;
using UnityEngine;

namespace KoboldUi.Element.Animations
{
    public abstract class AUiAnimationBase : MonoBehaviour, IUiAnimation 
    {
        public abstract IUiAction Appear();
        public abstract IUiAction AnimateFocusReturn();
        public abstract IUiAction AnimateFocusRemoved();
        public abstract IUiAction Disappear();
        public abstract void DisappearInstantly();
}
}
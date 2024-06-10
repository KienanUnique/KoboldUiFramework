using UnityEngine;

namespace KoboldUiFramework.Element.Animations
{
    public abstract class AUiAnimationBase : MonoBehaviour, IUiAnimation 
    {
        public abstract void Appear();
        public abstract void AnimateFocusReturn();
        public abstract void AnimateFocusRemoved();
        public abstract void Disappear();
    }
}
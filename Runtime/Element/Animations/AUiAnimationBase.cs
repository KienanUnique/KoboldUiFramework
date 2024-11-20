using Cysharp.Threading.Tasks;
using UnityEngine;

namespace KoboldUi.Element.Animations
{
    public abstract class AUiAnimationBase : MonoBehaviour, IUiAnimation 
    {
        public abstract UniTask Appear();
        public abstract UniTask AnimateFocusReturn();
        public abstract UniTask AnimateFocusRemoved();
        public abstract UniTask Disappear();
        public abstract void DisappearInstantly();
}
}
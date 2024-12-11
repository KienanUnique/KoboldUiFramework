using Cysharp.Threading.Tasks;
using KoboldUi.Element.Animations;
using UnityEngine;

namespace KoboldUi.Element.View
{
    public class AUiAnimatedView : AUiView
    {
        [SerializeField] private AUiAnimationBase openAnimation;
        [SerializeField] private AUiAnimationBase closeAnimation;

        public sealed override UniTask Open()
        {
            return UniTask.WhenAll(openAnimation.Appear(), base.Open());
        }
        
        public sealed override UniTask ReturnFocus()
        {
            return base.ReturnFocus();
        }

        public sealed override UniTask RemoveFocus()
        {
            return base.RemoveFocus();
        }

        public sealed override UniTask Close()
        {
            return UniTask.WhenAll(closeAnimation.Disappear(), base.Close());
        }
        
        public sealed override void CloseInstantly()
        {
            closeAnimation.DisappearInstantly();
        }
    }
}
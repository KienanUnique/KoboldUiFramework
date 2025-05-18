using KoboldUi.Element.Animations;
using KoboldUi.UiAction;
using UnityEngine;

namespace KoboldUi.Element.View
{
    public class AUiAnimatedView : AUiView
    {
        [SerializeField] private AUiAnimationBase openAnimation;
        [SerializeField] private AUiAnimationBase closeAnimation;

        public sealed override IUiAction Open()
        {
            return openAnimation.Appear();
        }
        
        public sealed override IUiAction ReturnFocus()
        {
            return base.ReturnFocus();
        }

        public sealed override IUiAction RemoveFocus()
        {
            return base.RemoveFocus();
        }

        public sealed override IUiAction Close()
        {
            return closeAnimation.Disappear();
        }
        
        public sealed override void CloseInstantly()
        {
            closeAnimation.DisappearInstantly();
        }
    }
}
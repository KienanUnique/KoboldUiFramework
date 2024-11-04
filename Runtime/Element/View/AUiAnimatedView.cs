using System.Collections.Generic;
using KoboldUi.Element.Animations;
using UnityEngine;

namespace KoboldUi.Element.View
{
    public class AUiAnimatedView : AUiView
    {
        [SerializeField] private AUiAnimationBase openAnimation;
        [SerializeField] private AUiAnimationBase closeAnimation;
        
        public HashSet<AUiAnimationBase> AnimationsForInjecting => new() {openAnimation, closeAnimation};

        public sealed override void Open()
        {
            openAnimation.Appear();
            base.Open();
        }
        
        public sealed override void ReturnFocus()
        {
            base.ReturnFocus();
        }

        public sealed override void RemoveFocus()
        {
            base.RemoveFocus();
        }

        public sealed override void Close()
        {
            closeAnimation.Disappear();
            base.Close();
        }
        
        public sealed override void CloseInstantly()
        {
            closeAnimation.DisappearInstantly();
        }
    }
}
using KoboldUi.Element.Animations;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
using UnityEngine;

namespace KoboldUi.Element.View
{
    public class AUiAnimatedView : AUiView
    {
        [SerializeField] private AUiAnimationBase openAnimation;
        [SerializeField] private AUiAnimationBase closeAnimation;

        public sealed override IUiAction Open(in IUiActionsPool pool)
        {
            return openAnimation == null ? base.Open(pool) : openAnimation.Appear(pool);
        }

        public sealed override IUiAction ReturnFocus(in IUiActionsPool pool)
        {
            return base.ReturnFocus(pool);
        }

        public sealed override IUiAction RemoveFocus(in IUiActionsPool pool)
        {
            return base.RemoveFocus(pool);
        }

        public sealed override IUiAction Close(in IUiActionsPool pool)
        {
            return closeAnimation == null ? base.Close(pool) : closeAnimation.Disappear(pool);
        }

        public sealed override void CloseInstantly()
        {
            closeAnimation.DisappearInstantly();
        }
    }
}
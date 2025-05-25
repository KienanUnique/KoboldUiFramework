using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using UnityEngine;

#if KOBOLD_ALCHEMY_SUPPORT
using Alchemy.Inspector;
#endif

namespace KoboldUi.Element.View
{
#if KOBOLD_ALCHEMY_SUPPORT
    [DisableAlchemyEditor]
#endif
    public class AUiAnimatedView : AUiView
    {
        [SerializeField] private AnimationData _openAnimation;
        [SerializeField] private AnimationData _closeAnimation;

        public sealed override IUiAction Open(in IUiActionsPool pool)
        {
            if (_openAnimation.Animation == null)
                return base.Open(pool);
            
            return _openAnimation.Animation.Appear(pool, _openAnimation.NeedWaitAnimation);
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
            if (_closeAnimation.Animation == null)
                return base.Close(pool);
            
            return _closeAnimation.Animation.Disappear(pool, _closeAnimation.NeedWaitAnimation);
        }

        public sealed override void CloseInstantly()
        {
            if (_closeAnimation.Animation != null)
                _closeAnimation.Animation.DisappearInstantly();
            else
                gameObject.SetActive(false);
        }
    }
}
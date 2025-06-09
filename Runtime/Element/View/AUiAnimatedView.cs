using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
using UnityEngine;
using KoboldUi.Element.Animations;

#if KOBOLD_ALCHEMY_SUPPORT
using Alchemy.Inspector;
using UnityEditor;
#endif

namespace KoboldUi.Element.View
{
    public class AUiAnimatedView : AUiView
    {
        [SerializeField] private AUiAnimationBase _openAnimation;
        [SerializeField] private AUiAnimationBase _closeAnimation;

        public sealed override IUiAction Open(in IUiActionsPool pool)
        {
            return _openAnimation ? _openAnimation.Appear(pool) : base.Open(pool);
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
            return _closeAnimation ? _closeAnimation.Disappear(pool) : base.Close(pool);
        }

        public sealed override void CloseInstantly()
        {
            if (_closeAnimation != null)
                _closeAnimation.DisappearInstantly();
            else
                gameObject.SetActive(false);
        }
        
#if KOBOLD_ALCHEMY_SUPPORT && UNITY_EDITOR
        [Button]
        public void AutoFill()
        {
            if(!TryGetComponent<AUiAnimationBase>(out var uiAnimation))
                return;

            if (!_openAnimation)
                _openAnimation = uiAnimation;

            if (!_closeAnimation)
                _closeAnimation = uiAnimation;
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
#endif
    }
}
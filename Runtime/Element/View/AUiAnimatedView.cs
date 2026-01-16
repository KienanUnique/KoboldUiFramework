using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
using UnityEngine;
using KoboldUi.Element.Animations;

#if KOBOLD_ALCHEMY_SUPPORT
using Alchemy.Inspector;
#elif KOBOLD_ODIN_SUPPORT
using Sirenix.OdinInspector;
#endif

#if (KOBOLD_ALCHEMY_SUPPORT || KOBOLD_ODIN_SUPPORT) && UNITY_EDITOR
using UnityEditor;
#endif

namespace KoboldUi.Element.View
{
    /// <summary>
    /// View that plays configured animations when opening or closing.
    /// </summary>
    public class AUiAnimatedView : AUiView, IAutoFillable
    {
#if KOBOLD_ODIN_SUPPORT
        [InfoBox("Optional. If null, the animation is replaced with SetActive(true)", InfoMessageType.Info, nameof(IsOpenAnimationMissing))]
#endif
        [SerializeField] private AUiAnimationBase _openAnimation;
#if KOBOLD_ODIN_SUPPORT
        [InfoBox("Optional. If null, the animation is replaced with SetActive(false)", InfoMessageType.Info, nameof(IsCloseAnimationMissing))]
#endif
        [SerializeField] private AUiAnimationBase _closeAnimation;

#if KOBOLD_ODIN_SUPPORT
        private bool IsOpenAnimationMissing => _openAnimation == null;
        private bool IsCloseAnimationMissing => _closeAnimation == null;
#endif

        /// <inheritdoc />
        public sealed override IUiAction Open(in IUiActionsPool pool)
        {
            if (_openAnimation)
                return _openAnimation.Appear(pool);

            gameObject.SetActive(true);
            return base.Open(pool);
        }

        /// <inheritdoc />
        public sealed override IUiAction ReturnFocus(in IUiActionsPool pool)
        {
            return base.ReturnFocus(pool);
        }

        /// <inheritdoc />
        public sealed override IUiAction RemoveFocus(in IUiActionsPool pool)
        {
            return base.RemoveFocus(pool);
        }

        /// <inheritdoc />
        public sealed override IUiAction Close(in IUiActionsPool pool)
        {
            if (_closeAnimation)
                return _closeAnimation.Disappear(pool);
            
            gameObject.SetActive(false);
            return base.Close(pool);
        }

        /// <inheritdoc />
        public sealed override void CloseInstantly()
        {
            if (_closeAnimation != null)
                _closeAnimation.DisappearInstantly();
            else
                gameObject.SetActive(false);
        }

#if (KOBOLD_ALCHEMY_SUPPORT || KOBOLD_ODIN_SUPPORT) && UNITY_EDITOR
        [Button]
        /// <inheritdoc />
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

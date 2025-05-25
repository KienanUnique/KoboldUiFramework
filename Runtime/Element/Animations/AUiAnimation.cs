using System;
using DG.Tweening;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Impl.Common;
using KoboldUi.UiAction.Pool;
using UnityEngine;
using Zenject;
#if KOBOLD_ALCHEMY_SUPPORT
using Alchemy.Inspector;
#endif

namespace KoboldUi.Element.Animations
{
    public abstract class AUiAnimation<TParams> : AUiAnimationBase where TParams : IUiAnimationParameters
    {
        [SerializeField] private bool useDefaultParameters = true;

#if KOBOLD_ALCHEMY_SUPPORT
        [ShowIf(nameof(NeedUseCustomParameters))]
#endif
        [SerializeField] private TParams animationParameters;

        [InjectOptional] private TParams _defaultAnimationParameters;

        private Sequence _sequence;

        protected TParams AnimationParameters
        {
            get
            {
                if (!useDefaultParameters) 
                    return animationParameters;
                
                if (_defaultAnimationParameters == null)
                    throw new Exception($"Default animation parameters {typeof(TParams)} wasn't found");
                
                return _defaultAnimationParameters;
            }
        }
        
        public bool NeedUseCustomParameters() => !useDefaultParameters;

        public override IUiAction Appear(in IUiActionsPool pool)
        {
            PrepareToAppear();
            gameObject.SetActive(true);
            return AnimateAppear(pool);
        }

        public override IUiAction Disappear(in IUiActionsPool pool)
        {
            return AnimateDisappear(pool, DisappearInstantly);
        }
        
        public override IUiAction AnimateFocusReturn(in IUiActionsPool pool)
        {
            pool.GetAction(out EmptyAction emptyAction);
            return emptyAction;
        }

        public override IUiAction AnimateFocusRemoved(in IUiActionsPool pool)
        {
            pool.GetAction(out EmptyAction emptyAction);
            return emptyAction;
        }

        public override void DisappearInstantly()
        {
            Debug.Log($"@@@@ DisappearInstantly ");
            gameObject.SetActive(false);
        }
        
        
        protected abstract void PrepareToAppear();
        protected abstract IUiAction AnimateAppear(in IUiActionsPool pool);
        protected abstract IUiAction AnimateDisappear(in IUiActionsPool pool, Action callback);
    }
}
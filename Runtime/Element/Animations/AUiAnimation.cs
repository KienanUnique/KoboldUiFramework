using System;
using DG.Tweening;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Impl;
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

        public override IUiAction Appear()
        {
            PrepareToAppear();
            gameObject.SetActive(true);
            return AnimateAppear();
        }

        public override IUiAction Disappear()
        {
            return AnimateDisappear(DisappearInstantly);
        }
        
        public override IUiAction AnimateFocusReturn() => new EmptyAction();

        public override IUiAction AnimateFocusRemoved() => new EmptyAction();
        
        public override void DisappearInstantly()
        {
            gameObject.SetActive(false);
        }
        
        
        protected abstract void PrepareToAppear();
        protected abstract IUiAction AnimateAppear();
        protected abstract IUiAction AnimateDisappear(Action callback);
    }
}
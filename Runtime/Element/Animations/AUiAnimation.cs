using System;
using DG.Tweening;
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

        public override void Appear()
        {
            AnimateAppear();
            gameObject.SetActive(true);
        }

        public override void Disappear()
        {
            AnimateDisappear(DisappearInstantly);
        }
        
        public override void AnimateFocusReturn()
        {
        }

        public override void AnimateFocusRemoved()
        {
        }

        public override void DisappearInstantly()
        {
            gameObject.SetActive(false);
        }
        
        protected abstract void AnimateAppear();
        protected abstract void AnimateDisappear(Action callback);
    }
}
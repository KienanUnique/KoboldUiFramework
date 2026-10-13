using System;
using UnityEngine;
using UnityEngine.Serialization;
#if KOBOLD_ALCHEMY_SUPPORT
using Alchemy.Inspector;
#elif KOBOLD_ODIN_SUPPORT
using Sirenix.OdinInspector;
#endif

namespace KoboldUi.Element.Animations
{
    /// <summary>Base component for custom UniTask animations with parameter assets.</summary>
    public abstract class AUiAsyncAnimation<TParams> : AUiAsyncAnimation, IUiAnimationDefaultsReceiver
        where TParams : class, IAnimationParameters
    {
        [FormerlySerializedAs("useDefaultParameters")]
        [SerializeField]
        private bool _useDefaultParameters = true;

#if KOBOLD_ALCHEMY_SUPPORT || KOBOLD_ODIN_SUPPORT
        [ShowIf(nameof(NeedUseCustomParameters))]
#endif
        [FormerlySerializedAs("animationParameters")]
        [SerializeField]
        private TParams _animationParameters;

        private TParams _defaultAnimationParameters;

        public void ApplyDefaults(UiAnimationDefaults defaults)
        {
            if (defaults == null) throw new ArgumentNullException(nameof(defaults));
            defaults.TryGet(out _defaultAnimationParameters);
        }

        /// <summary>Gets explicit or default parameters when available.</summary>
        protected TParams OptionalAnimationParameters
        {
            get
            {
                if (_useDefaultParameters) return _defaultAnimationParameters;
                if (_animationParameters == null)
                    throw new InvalidOperationException(
                        $"Custom animation parameters {typeof(TParams)} weren't assigned.");
                return _animationParameters;
            }
        }

        /// <summary>Gets required parameters, throwing when none are configured.</summary>
        protected TParams AnimationParameters
        {
            get
            {
                var parameters = OptionalAnimationParameters;
                if (parameters == null)
                    throw new Exception($"Default animation parameters {typeof(TParams)} wasn't found");
                return parameters;
            }
        }

        /// <summary>Whether the inspector should display custom parameter controls.</summary>
        public bool NeedUseCustomParameters()
        {
            return !_useDefaultParameters;
        }
    }
}

using System;
using System.Collections.Generic;

namespace KoboldUi.Element.Animations
{
    /// <summary>Explicit overrides for animation parameters in one UI scope.</summary>
    public sealed class UiAnimationDefaults
    {
        private readonly Dictionary<Type, IAnimationParameters> _parameters =
            new Dictionary<Type, IAnimationParameters>();
        private readonly UiAnimationDefaults _projectDefaults;

        public UiAnimationDefaults(UiAnimationDefaults projectDefaults = null)
        {
            _projectDefaults = projectDefaults;
        }

        public bool TryGet<T>(out T value) where T : class, IAnimationParameters
        {
            if (_parameters.TryGetValue(typeof(T), out var parameter))
            {
                value = (T)parameter;
                return true;
            }
            if (_projectDefaults != null) return _projectDefaults.TryGet(out value);
            value = null;
            return false;
        }

        public void Add(IAnimationParameters parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            Add(parameter.GetType(), parameter);
        }

        internal void Add(Type parameterType, IAnimationParameters parameter)
        {
            if (parameterType == null) throw new ArgumentNullException(nameof(parameterType));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            if (!parameterType.IsInstanceOfType(parameter))
                throw new ArgumentException($"{parameter.GetType().FullName} is not assignable to {parameterType.FullName}.",
                    nameof(parameter));
            if (_parameters.ContainsKey(parameterType))
                throw new InvalidOperationException($"Defaults already contain {parameterType.FullName}.");
            _parameters.Add(parameterType, parameter);
        }
    }
}

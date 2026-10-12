using System;
using System.Collections.Generic;

namespace KoboldUi.Element.Animations
{
    /// <summary>Explicit overrides for animation parameters in one UI scope.</summary>
    public sealed class UiAnimationDefaults
    {
        private readonly Dictionary<Type, IKoboldDefaultAnimationParameters> _parameters =
            new Dictionary<Type, IKoboldDefaultAnimationParameters>();
        private readonly UiAnimationDefaults _projectDefaults;

        public UiAnimationDefaults(UiAnimationDefaults projectDefaults = null)
        {
            _projectDefaults = projectDefaults;
        }

        public bool TryGet<T>(out T value) where T : class, IKoboldDefaultAnimationParameters
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

        public void Add(IKoboldDefaultAnimationParameters parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            var type = parameter.GetType();
            if (_parameters.ContainsKey(type))
                throw new InvalidOperationException($"Defaults already contain {type.FullName}.");
            _parameters.Add(type, parameter);
        }
    }
}

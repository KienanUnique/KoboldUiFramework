using System;
using System.Collections.Generic;
using KoboldUi.Element.Animations.Parameters;
using UnityEngine;

namespace KoboldUi.Element.Animations
{
    [CreateAssetMenu(menuName = "KoboldUi/Animation Defaults", fileName = "UiAnimationDefaults")]
    public sealed class UiAnimationDefaults : ScriptableObject
    {
        [SerializeField] private List<AUiAnimationParameters> parameters = new List<AUiAnimationParameters>();

        public bool TryGet<T>(out T value) where T : IUiAnimationParameters
        {
            foreach (var parameter in parameters)
            {
                if (parameter is T match)
                {
                    value = match;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public void Add(AUiAnimationParameters parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            foreach (var current in parameters)
                if (current != null && current.GetType() == parameter.GetType())
                    throw new InvalidOperationException($"Defaults already contain {parameter.GetType().FullName}.");
            parameters.Add(parameter);
        }
    }
}

using System;
using KoboldUi.Element.Animations;
using UnityEngine;

namespace KoboldUi.Utils
{
    [Serializable]
    public class AnimationData
    {
        [field: SerializeField] public AUiAnimationBase Animation { get; private set; }
        [field: SerializeField] public bool NeedWaitAnimation { get; private set; }
    }
}
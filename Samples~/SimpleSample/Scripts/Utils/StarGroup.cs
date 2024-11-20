using System;
using UnityEngine;

namespace Samples.Simple_Sample.Scripts.Utils
{
    [Serializable]
    public class StarGroup
    {
        [SerializeField] private GameObject achievedState;
        [SerializeField] private GameObject nonAchievedState;

        public void SetState(bool isAchieved)
        {
            achievedState.SetActive(isAchieved);
            nonAchievedState.SetActive(!isAchieved);
        }
    }
}
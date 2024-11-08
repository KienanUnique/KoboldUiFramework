using System.Collections.Generic;
using Samples.Simple_Sample.Scripts.Utils;
using UnityEngine;
using Zenject;

namespace Samples.Simple_Sample.Scripts.Services.LevelProgression.Impl
{
    public class LevelProgressionService : ILevelProgressionService, IInitializable
    {
        private readonly List<LevelData> _progression = new();

        public IReadOnlyList<LevelData> Progression => _progression;
        
        public void Initialize()
        {
            const int levelsCount = 12;

            var firstLockedLevel = Random.Range(6, levelsCount);
            var lastPassedLevel = Random.Range(1, firstLockedLevel - 1);

            for (var i = 1; i < levelsCount; i++)
            {
                LevelData newLevelData;
                var name = $"Level {i}";
                
                if (i <= lastPassedLevel)
                {
                    var starsCount = Random.Range(1, 3);
                    newLevelData = new LevelData(name, true, true, starsCount);
                }
                else if (i <= firstLockedLevel)
                {
                    newLevelData = new LevelData(name, true, false, -1);
                }
                else
                {
                    newLevelData = new LevelData(name, false, false, -1);
                }
                
                _progression.Add(newLevelData);
            }
        }
    }
}
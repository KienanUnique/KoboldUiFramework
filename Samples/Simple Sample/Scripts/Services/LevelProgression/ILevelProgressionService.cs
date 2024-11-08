using System.Collections.Generic;
using Samples.Simple_Sample.Scripts.Utils;

namespace Samples.Simple_Sample.Scripts.Services.LevelProgression
{
    public interface ILevelProgressionService
    {
        IReadOnlyList<LevelData> Progression { get; }
    }
}
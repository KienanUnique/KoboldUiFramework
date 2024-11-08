namespace Samples.Simple_Sample.Scripts.Utils
{
    public struct LevelData
    {
        public string Name { get; }
        public bool IsUnlocked { get; }
        public bool IsPassed { get; }
        public int StarsCount { get; }

        public LevelData(string name, bool isUnlocked, bool isPassed, int starsCount)
        {
            Name = name;
            IsUnlocked = isUnlocked;
            IsPassed = isPassed;
            StarsCount = starsCount;
        }
    }
}
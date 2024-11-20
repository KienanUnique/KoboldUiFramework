namespace Samples.Simple_Sample.Scripts.Utils
{
    public struct LevelData
    {
        public string Name { get; }
        public bool IsUnlocked { get; }
        public int StarsCount { get; }

        public LevelData(string name, bool isUnlocked, int starsCount)
        {
            Name = name;
            IsUnlocked = isUnlocked;
            StarsCount = starsCount;
        }
    }
}
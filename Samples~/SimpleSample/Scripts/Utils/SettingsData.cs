namespace Samples.Simple_Sample.Scripts.Utils
{
    public struct SettingsData
    {
        public float SoundsVolume { get; }
        public float MusicVolume { get; }
        public bool IsEasyModeEnabled { get; }

        public SettingsData(float soundsVolume, float musicVolume, bool isEasyModeEnabled)
        {
            SoundsVolume = soundsVolume;
            MusicVolume = musicVolume;
            IsEasyModeEnabled = isEasyModeEnabled;
        }
    }
}
using Samples.Simple_Sample.Scripts.Utils;

namespace Samples.Simple_Sample.Scripts.Services.SettingsStorage.Impl
{
    public class SettingsStorageService : ISettingsStorageService
    {
        public SettingsData CurrentSettings { get; private set; }
        public SettingsData? UnsavedSettings { get; private set; }

        public void ApplySettings(SettingsData settingsData)
        {
            CurrentSettings = settingsData;
        }

        public void RememberUnsavedSettings(SettingsData settingsData)
        {
            UnsavedSettings = settingsData;
        }

        public void ApplyUnsavedSettings()
        {
            CurrentSettings = UnsavedSettings!.Value;
            UnsavedSettings = null;
        }

        public void ForgetUnsavedSettings()
        {
            UnsavedSettings = null;
        }
    }
}
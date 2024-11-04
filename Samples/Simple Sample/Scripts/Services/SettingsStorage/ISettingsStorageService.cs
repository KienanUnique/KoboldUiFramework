using Samples.Simple_Sample.Scripts.Utils;

namespace Samples.Simple_Sample.Scripts.Services.SettingsStorage
{
    public interface ISettingsStorageService
    {
        SettingsData CurrentSettings { get; }
        SettingsData? UnsavedSettings { get; }
        
        void ApplySettings(SettingsData settingsData);
        void RememberUnsavedSettings(SettingsData settingsData);
        void ApplyUnsavedSettings();
        void ForgetUnsavedSettings();
    }
}
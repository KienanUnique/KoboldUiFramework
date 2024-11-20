using System;
using Samples.Simple_Sample.Scripts.Utils;
using SampleUnirx;

namespace Samples.Simple_Sample.Scripts.Services.SettingsStorage
{
    public interface ISettingsStorageService
    {
        SettingsData CurrentSettings { get; }
        SettingsData? UnsavedSettings { get; }
        IObservable<Unit> UnsavedSettingsForgotten { get; }

        void ApplySettings(SettingsData settingsData);
        void RememberUnsavedSettings(SettingsData settingsData);
        void ApplyUnsavedSettings();
        void ForgetUnsavedSettings();
    }
}
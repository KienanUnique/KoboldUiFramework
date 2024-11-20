using System;
using Samples.Simple_Sample.Scripts.Utils;
using SampleUnirx;

namespace Samples.Simple_Sample.Scripts.Services.SettingsStorage.Impl
{
    public class SettingsStorageService : ISettingsStorageService
    {
        private readonly ReactiveCommand _unsavedSettingsForgotten = new();
        
        public SettingsData CurrentSettings { get; private set; }
        public SettingsData? UnsavedSettings { get; private set; }
        public IObservable<Unit> UnsavedSettingsForgotten => _unsavedSettingsForgotten;

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
            ForgetUnsavedSettings();
        }

        public void ForgetUnsavedSettings()
        {
            UnsavedSettings = null;
            _unsavedSettingsForgotten.Execute();
        }
    }
}
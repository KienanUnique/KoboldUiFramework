using KoboldUi.Element.Controller;
using KoboldUi.Services.WindowsService;
using Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.SettingsChangeConfirmation;
using Samples.Simple_Sample.Scripts.Services.SettingsStorage;
using Samples.Simple_Sample.Scripts.Utils;
using UniRx;

namespace Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.Settings.Settings
{
    public class SettingsController : AUiController<SettingsView>
    {
        private readonly ISettingsStorageService _settingsStorageService;
        private readonly ILocalWindowsService _localWindowsService;
        private readonly ReactiveProperty<bool> _wasSomethingChanged = new();

        public SettingsController(
            ISettingsStorageService settingsStorageService, 
            ILocalWindowsService localWindowsService
        )
        {
            _settingsStorageService = settingsStorageService;
            _localWindowsService = localWindowsService;
        }

        public override void Initialize()
        { 
            View.musicVolume.OnValueChangedAsObservable().Subscribe(_ => RememberThatSomethingChanged()).AddTo(View);
            View.soundVolume.OnValueChangedAsObservable().Subscribe(_ => RememberThatSomethingChanged()).AddTo(View);

            View.easyModeToggle.OnValueChangedAsObservable().Subscribe(_ => RememberThatSomethingChanged()).AddTo(View);
            
            View.applyButton.OnClickAsObservable().Subscribe(_ => OnApplyButtonClick()).AddTo(View);
            View.cancelButton.OnClickAsObservable().Subscribe(_ => OnCancelButtonClick()).AddTo(View);
            View.closeButton.OnClickAsObservable().Subscribe(_ => OnCloseButtonClick()).AddTo(View);
            
            _settingsStorageService.UnsavedSettingsForgotten.Subscribe(_ => ResetSettings()).AddTo(View);

            _wasSomethingChanged.Subscribe(OnSomethingChanged).AddTo(View);
        }

        protected override void OnOpen() => ResetSettings();

        private void ResetSettings()
        {
            var currentSettings = _settingsStorageService.CurrentSettings;
            View.musicVolume.value = currentSettings.MusicVolume;
            View.soundVolume.value = currentSettings.SoundsVolume;
            View.easyModeToggle.isOn = currentSettings.IsEasyModeEnabled;
            
            _wasSomethingChanged.Value = false;
        }

        private void OnSomethingChanged(bool isSomethingChanged) => View.applyButton.interactable = isSomethingChanged;
        private void RememberThatSomethingChanged() => _wasSomethingChanged.Value = true;

        private void OnCloseButtonClick()
        {
            if (_wasSomethingChanged.Value)
            {
                var currentSettings = CreateSettingsData();
                _settingsStorageService.RememberUnsavedSettings(currentSettings);
                
                _localWindowsService.OpenWindow<SettingsChangeConfirmationWindow>();
            }
            else
            {
                _localWindowsService.TryBackWindow();
            }
        }

        private void OnCancelButtonClick()
        {
            _settingsStorageService.ForgetUnsavedSettings();
            ResetSettings();
            
            _localWindowsService.TryBackWindow();
        }

        private void OnApplyButtonClick()
        {
            var currentSettings = CreateSettingsData();
            _settingsStorageService.ApplySettings(currentSettings);
            _wasSomethingChanged.Value = false;
        }

        private SettingsData CreateSettingsData()
        {
            return new SettingsData(
                View.musicVolume.value,
                View.soundVolume.value, 
                View.easyModeToggle.isOn
            );
        }
    }
}
using KoboldUi.Element.Controller;
using KoboldUi.Services.WindowsService;
using KoboldUi.Utils;
using Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.MainMenu;
using Samples.Simple_Sample.Scripts.Services.SettingsStorage;
using UniRx;

namespace Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.SettingsChangeConfirmation.SettingsChangeConfirmation
{
    public class SettingsChangeConfirmationController : AUiController<SettingsChangeConfirmationView>
    {
        private readonly ILocalWindowsService _localWindowsService;
        private readonly ISettingsStorageService _settingsStorageService;

        public SettingsChangeConfirmationController(
            ILocalWindowsService localWindowsService,
            ISettingsStorageService settingsStorageService
        )
        {
            _localWindowsService = localWindowsService;
            _settingsStorageService = settingsStorageService;
        }

        public override void Initialize()
        {
            View.yesButton.OnClickAsObservable().Subscribe(_ => OnYesButtonClicked()).AddTo(View);
            View.noButton.OnClickAsObservable().Subscribe(_ => OnNoButtonClicked()).AddTo(View);
        }

        private void OnNoButtonClicked()
        {
            _localWindowsService.TryBackWindow();
        }

        private void OnYesButtonClicked()
        {
            _settingsStorageService.ApplyUnsavedSettings();
            _localWindowsService.TryBackToWindow<MainMenuWindow>(previousWindowsPolitic: EAnimationPolitic.Wait);
        }
    }
}
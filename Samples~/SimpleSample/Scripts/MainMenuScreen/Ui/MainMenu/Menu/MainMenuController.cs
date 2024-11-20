using KoboldUi.Element.Controller;
using KoboldUi.Services.WindowsService;
using Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.LevelSelector;
using Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.Settings;
using SampleUnirx;
using UnityEngine;

namespace Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.MainMenu.Menu
{
    public class MainMenuController : AUiController<MainMenuView>
    {
        private readonly ILocalWindowsService _localWindowsService;

        public MainMenuController(ILocalWindowsService localWindowsService)
        {
            _localWindowsService = localWindowsService;
        }

        public override void Initialize()
        {
            View.startButton.OnClickAsObservable().Subscribe(_ => OnStartButtonClick()).AddTo(View);
            View.settingsButton.OnClickAsObservable().Subscribe(_ => OnSettingsButtonClick()).AddTo(View);
            View.exitButton.OnClickAsObservable().Subscribe(_ => OnExitButtonClick()).AddTo(View);
        }

        private void OnStartButtonClick() => _localWindowsService.OpenWindow<LevelSelectorWindow>();

        private void OnSettingsButtonClick() => _localWindowsService.OpenWindow<SettingsWindow>();
        private void OnExitButtonClick() => Application.Quit();
    }
}
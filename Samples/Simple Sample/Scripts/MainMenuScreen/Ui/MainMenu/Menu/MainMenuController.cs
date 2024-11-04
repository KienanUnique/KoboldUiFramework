using KoboldUi.Element.Controller;
using KoboldUi.Services.WindowsService;
using Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.Settings;
using Samples.Simple_Sample.Scripts.Services.Scenes;
using UniRx;
using UnityEngine;

namespace Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.MainMenu.Menu
{
    public class MainMenuController : AUiController<MainMenuView>
    {
        private readonly IScenesService _scenesService;
        private readonly ILocalWindowsService _localWindowsService;

        public MainMenuController(
            IScenesService scenesService, 
            ILocalWindowsService localWindowsService
        )
        {
            _scenesService = scenesService;
            _localWindowsService = localWindowsService;
        }

        public override void Initialize()
        {
            View.startButton.OnClickAsObservable().Subscribe(_ => OnStartButtonClick()).AddTo(View);
            View.settingsButton.OnClickAsObservable().Subscribe(_ => OnSettingsButtonClick()).AddTo(View);
            View.exitButton.OnClickAsObservable().Subscribe(_ => OnExitButtonClick()).AddTo(View);
        }

        private void OnStartButtonClick() => _scenesService.ReloadCurrentScene(); // TODO: change to select level
        private void OnSettingsButtonClick() => _localWindowsService.OpenWindow<SettingsWindow>();
        private void OnExitButtonClick() => Application.Quit();
    }
}
using System;
using KoboldUi.Services.WindowsService;
using Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.MainMenu;
using Samples.Simple_Sample.Scripts.Services.Scenes;
using SampleUnirx;
using Zenject;

namespace Samples.Simple_Sample.Scripts.Services.Bootstrap
{
    public class Bootstrap : IInitializable, IDisposable
    {
        private readonly CompositeDisposable _compositeDisposable = new();

        private readonly ILocalWindowsService _localWindowsService;
        private readonly IScenesService _scenesService;

        public Bootstrap(
            ILocalWindowsService localWindowsService,
            IScenesService scenesService
        )
        {
            _localWindowsService = localWindowsService;
            _scenesService = scenesService;
        }

        public void Initialize()
        {
            if (_scenesService.IsLoadingCompleted.Value)
                OpenMainMenu();
            else
                _scenesService.IsLoadingCompleted.Subscribe(OnLoadingComplete).AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }

        private void OnLoadingComplete(bool isComplete)
        {
            if (!isComplete)
                return;

            OpenMainMenu();
        }

        private void OpenMainMenu()
        {
            _localWindowsService.OpenWindow<MainMenuWindow>();
            _compositeDisposable.Dispose();
        }
    }
}
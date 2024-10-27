using KoboldUi.Services.WindowsService;
using Samples.Simple_Sample.Scripts.MainMenuScreen.Ui.MainMenu;
using Zenject;

namespace Samples.Simple_Sample.Scripts.Services.Bootstrap
{
    public class Bootstrap : IInitializable
    {
        private readonly ILocalWindowsService _localWindowsService;

        public Bootstrap(ILocalWindowsService localWindowsService)
        {
            _localWindowsService = localWindowsService;
        }

        public void Initialize()
        {
            _localWindowsService.OpenWindow<MainMenuWindow>();
        }
    }
}
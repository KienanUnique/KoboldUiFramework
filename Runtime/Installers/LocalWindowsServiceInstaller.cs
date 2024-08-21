using KoboldUi.Services.WindowsService.Impl;
using Zenject;

namespace KoboldUi.Installers
{
    public class LocalWindowsServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<LocalWindowsService>().AsSingle().NonLazy();
        }
    }
}
using KoboldUi.Services.WindowsService.Impl;
using Zenject;

namespace KoboldUi.Installers
{
    public class ProjectWindowsServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ProjectWindowsService>().AsSingle().NonLazy();
        }
    }
}
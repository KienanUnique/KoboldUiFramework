using Samples.Simple_Sample.Scripts.Services.Scenes.Impl;
using Samples.Simple_Sample.Scripts.Services.SettingsStorage.Impl;
using Zenject;

namespace Samples.Simple_Sample.Scripts.Installers
{
    public class ProjectServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ScenesService>().AsSingle();
            Container.BindInterfacesTo<SettingsStorageService>().AsSingle();
        }
    }
}
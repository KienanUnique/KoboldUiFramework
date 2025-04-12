#if KOBOLD_ZENJECT_SUPPORT
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
#elif KOBOLD_VCONTAINER_SUPPORT
using KoboldUi.Services.WindowsService.Impl;
using VContainer;
using VContainer.Unity;

namespace KoboldUi.Installers
{
    public class ProjectWindowsServiceInstaller : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ProjectWindowsService>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}
#endif
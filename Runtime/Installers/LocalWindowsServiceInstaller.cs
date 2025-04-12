#if KOBOLD_ZENJECT_SUPPORT
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
#elif KOBOLD_VCONTAINER_SUPPORT
using KoboldUi.Services.WindowsService.Impl;
using VContainer;
using VContainer.Unity;

namespace KoboldUi.Installers
{
    public class LocalWindowsServiceInstaller : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<LocalWindowsService>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}
#endif
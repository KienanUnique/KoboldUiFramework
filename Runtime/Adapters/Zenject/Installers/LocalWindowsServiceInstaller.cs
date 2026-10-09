using KoboldUi.Services.WindowsService.Impl;
using Zenject;
using KoboldUi.Composition;
using KoboldUi.Composition.Zenject;

namespace KoboldUi.Installers
{
    /// <summary>
    /// Registers the local windows service with the DI container.
    /// </summary>
    public class LocalWindowsServiceInstaller : MonoInstaller
    {
        /// <inheritdoc />
        public override void InstallBindings()
        {
            Container.Bind<IUiResolver>().To<ZenjectUiResolver>().AsSingle().IfNotBound();
            Container.Bind<WindowRegistry>().AsSingle().IfNotBound();
            Container.BindInterfacesTo<LocalWindowsService>().AsSingle().NonLazy();
        }
    }
}

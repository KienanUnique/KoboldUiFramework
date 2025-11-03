using KoboldUi.Services.WindowsService.Impl;
using Zenject;

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
            Container.BindInterfacesTo<LocalWindowsService>().AsSingle().NonLazy();
        }
    }
}
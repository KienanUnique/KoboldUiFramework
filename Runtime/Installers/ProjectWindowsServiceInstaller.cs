using KoboldUi.Services.WindowsService.Impl;
using Zenject;

namespace KoboldUi.Installers
{
    /// <summary>
    /// Registers the project-scoped windows service with the DI container.
    /// </summary>
    public class ProjectWindowsServiceInstaller : MonoInstaller
    {
        /// <inheritdoc />
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ProjectWindowsService>().AsSingle().NonLazy();
        }
    }
}
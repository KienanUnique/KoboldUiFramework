using KoboldUi.Services.WindowsService.Impl;
using Zenject;
using KoboldUi.Composition;
using KoboldUi.Composition.Zenject;

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
            Container.Bind<IUiCompositionAdapter>().To<ZenjectUiCompositionAdapter>().AsSingle().IfNotBound();
            Container.Bind<WindowRegistry>().AsSingle().IfNotBound();
            Container.BindInterfacesTo<ProjectWindowsService>().AsSingle().NonLazy();
        }
    }
}

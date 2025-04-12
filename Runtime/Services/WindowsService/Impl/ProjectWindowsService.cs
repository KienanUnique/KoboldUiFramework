#if KOBOLD_ZENJECT_SUPPORT
using Zenject;
#elif KOBOLD_VCONTAINER_SUPPORT
using VContainer;
#endif

namespace KoboldUi.Services.WindowsService.Impl
{
    public class ProjectWindowsService : AWindowsService, IProjectWindowsService
    {
#if KOBOLD_ZENJECT_SUPPORT
        public ProjectWindowsService(DiContainer diContainer) : base(diContainer)
        {
        }
#elif KOBOLD_VCONTAINER_SUPPORT
        public ProjectWindowsService(IObjectResolver diContainer) : base(diContainer)
        {
        }
#endif
}
}
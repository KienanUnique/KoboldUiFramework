#if KOBOLD_ZENJECT_SUPPORT
using Zenject;
#elif KOBOLD_VCONTAINER_SUPPORT
using VContainer;
#endif

namespace KoboldUi.Services.WindowsService.Impl
{
    public class LocalWindowsService : AWindowsService, ILocalWindowsService
    {
#if KOBOLD_ZENJECT_SUPPORT
        public LocalWindowsService(DiContainer diContainer) : base(diContainer)
        {
        }
#elif KOBOLD_VCONTAINER_SUPPORT
        public LocalWindowsService(IObjectResolver diContainer) : base(diContainer)
        {
        }
#endif
    }
}
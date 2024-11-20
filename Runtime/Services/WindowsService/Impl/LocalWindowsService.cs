using Zenject;

namespace KoboldUi.Services.WindowsService.Impl
{
    public class LocalWindowsService : AWindowsService, ILocalWindowsService
    {
        public LocalWindowsService(DiContainer diContainer) : base(diContainer)
        {
        }
    }
}
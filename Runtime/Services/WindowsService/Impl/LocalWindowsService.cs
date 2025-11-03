using Zenject;

namespace KoboldUi.Services.WindowsService.Impl
{
    /// <summary>
    /// Windows service instance meant to be installed within local contexts.
    /// </summary>
    public class LocalWindowsService : AWindowsService, ILocalWindowsService
    {
        public LocalWindowsService(DiContainer diContainer) : base(diContainer)
        {
        }
    }
}
using Zenject;

namespace KoboldUi.Services.WindowsService.Impl
{
    /// <summary>
    /// Windows service intended for application-wide usage.
    /// </summary>
    public class ProjectWindowsService : AWindowsService, IProjectWindowsService
    {
        public ProjectWindowsService(DiContainer diContainer) : base(diContainer)
        {
        }
    }
}
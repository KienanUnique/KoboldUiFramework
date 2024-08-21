using Zenject;

namespace KoboldUi.Services.WindowsService.Impl
{
    public class ProjectWindowsService : AWindowsService, IProjectWindowsService
    {
        public ProjectWindowsService(DiContainer diContainer) : base(diContainer)
        {
        }
    }
}
using KoboldUi.Composition;

namespace KoboldUi.Services.WindowsService.Impl
{
    /// <summary>
    /// Windows service intended for application-wide usage.
    /// </summary>
    public class ProjectWindowsService : AWindowsService, IProjectWindowsService
    {
        public ProjectWindowsService(WindowRegistry windowRegistry, System.Func<bool> ensureReady = null,
            System.Action<System.Action> queueAction = null) : base(windowRegistry, ensureReady, queueAction)
        {
        }
    }
}

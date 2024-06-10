using Zenject;

namespace KoboldUiFramework.Installers
{
    public class ProjectSignalBusInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
        }
    }
}
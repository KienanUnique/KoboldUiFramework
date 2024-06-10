using Zenject;

namespace KoboldUi.Installers
{
    public class ProjectSignalBusInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
        }
    }
}
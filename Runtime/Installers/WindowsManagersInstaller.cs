using Managers;
using Signals;
using UnityEngine;
using Utils;
using Zenject;

namespace Installers
{
    public class WindowsManagersInstaller : MonoInstaller
    {
        [SerializeField] private EWindowLayer windowLayer;
        public override void InstallBindings()
        {
            InstallSignals();
            InstallManager();
        }

        private void InstallSignals()
        {
            Container.DeclareSignal<SignalOpenWindow>().WithId(windowLayer);
            Container.DeclareSignal<SignalCloseWindow>().WithId(windowLayer);
            Container.DeclareSignal<SignalBackWindow>().WithId(windowLayer);
        }
        
        private void InstallManager()
        {
            Container.BindInterfacesTo<ConcreteLayerWindowsManager>().AsSingle().WithArguments(windowLayer).NonLazy();
        }
    }
}
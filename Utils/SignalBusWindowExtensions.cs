using KoboldUiFramework.Signals;
using KoboldUiFramework.Windows;
using Zenject;

namespace KoboldUiFramework.Utils
{
    public static class SignalBusWindowExtensions
    {
        public static void OpenWindow<TWindow>(this SignalBus signalBus, EWindowLayer windowLayer = EWindowLayer.Local)
            where TWindow : IWindow
        {
            signalBus.FireId(windowLayer, new SignalOpenWindow(typeof(TWindow)));
        }

        public static void CloseWindow(this SignalBus signalBus, EWindowLayer windowLayer = EWindowLayer.Local)
        {
            signalBus.FireId<SignalCloseWindow>(windowLayer);
        }
        
        public static void BackWindow(this SignalBus signalBus, EWindowLayer windowLayer = EWindowLayer.Local)
        {
            signalBus.FireId<SignalBackWindow>(windowLayer);
        }
    }
}
using System;

namespace KoboldUiFramework.Signals
{
    public readonly struct SignalOpenWindow
    {
        public readonly Type WindowType;

        public SignalOpenWindow(Type windowType)
        {
            WindowType = windowType;
        }
    }
}
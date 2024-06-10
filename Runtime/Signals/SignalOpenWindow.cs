using System;

namespace KoboldUi.Signals
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
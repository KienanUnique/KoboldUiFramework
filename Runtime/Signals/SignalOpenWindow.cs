using System;

namespace Signals
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
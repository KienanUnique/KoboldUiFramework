using System;

namespace KoboldUi.Composition
{
    public sealed class UiActivationException : Exception
    {
        public UiActivationException(string message) : base(message) { }
        public UiActivationException(string message, Exception inner) : base(message, inner) { }
    }
}

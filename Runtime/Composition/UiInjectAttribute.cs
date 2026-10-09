using System;

namespace KoboldUi.Composition
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class UiInjectAttribute : Attribute
    {
        public bool Optional { get; set; }
    }
}

using System;

namespace KoboldUi.Composition
{
    public interface IUiResolver
    {
        bool TryResolve(Type type, out object value);
    }
}

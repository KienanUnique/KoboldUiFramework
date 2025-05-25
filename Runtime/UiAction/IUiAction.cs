using System;
using Cysharp.Threading.Tasks;

namespace KoboldUi.UiAction
{
    public interface IUiAction : IDisposable
    {
        UniTask Start();
    }
}
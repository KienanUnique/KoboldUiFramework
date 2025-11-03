using System;
using Cysharp.Threading.Tasks;

namespace KoboldUi.UiAction
{
    /// <summary>
    /// Represents an asynchronous UI operation that can be executed and disposed.
    /// </summary>
    public interface IUiAction : IDisposable
    {
        /// <summary>
        /// Starts the action and returns a task that completes when the action finishes.
        /// </summary>
        UniTask Start();
    }
}

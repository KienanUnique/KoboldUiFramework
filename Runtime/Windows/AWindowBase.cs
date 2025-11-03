using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using UnityEngine;
using Zenject;

namespace KoboldUi.Windows
{
    /// <summary>
    /// Base window behaviour that integrates with the window stack and DI container.
    /// </summary>
    public abstract class AWindowBase : MonoBehaviour, IWindow, IInitializable
    {
        /// <inheritdoc />
        public virtual void Initialize()
        {
            IsInitialized = true;
        }

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }
        /// <inheritdoc />
        public virtual string Name => gameObject.name;

        /// <inheritdoc />
        public abstract bool IsPopup { get; }
        /// <inheritdoc />
        public abstract bool IsBackLogicIgnorable { get; }

        /// <inheritdoc />
        public IUiAction WaitInitialization(in IUiActionsPool pool)
        {
            pool.GetAction(out var action, this);
            return action;
        }

        /// <inheritdoc />
        public abstract IUiAction SetState(EWindowState state, in IUiActionsPool pool);
        /// <inheritdoc />
        public abstract void ApplyOrder(int order);

        /// <inheritdoc />
        public abstract void InstallBindings(DiContainer container);
    }
}
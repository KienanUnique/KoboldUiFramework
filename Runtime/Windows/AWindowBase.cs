using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using UnityEngine;
using Zenject;

namespace KoboldUi.Windows
{
    public abstract class AWindowBase : MonoBehaviour, IWindow, IInitializable
    {
        public virtual void Initialize()
        {
            IsInitialized = true;
        }

        public bool IsInitialized { get; private set; }
        public virtual string Name => gameObject.name;
        
        public abstract bool IsPopup { get; }
        public abstract bool IsBackLogicIgnorable { get; }

        public IUiAction WaitInitialization(in IUiActionsPool pool)
        {
            pool.GetAction(out var action, this);
            return action;
        }

        public abstract IUiAction SetState(EWindowState state, in IUiActionsPool pool);
        public abstract void ApplyOrder(int order);

        public abstract void InstallBindings(DiContainer container);
    }
}
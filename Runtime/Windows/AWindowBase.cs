using KoboldUi.UiAction;
using KoboldUi.UiAction.Impl;
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

        public IUiAction WaitInitialization()
        {
            var action = new WaitInitializationAction();
            action.Setup(this);
            return action;
        }

        public abstract void InstallBindings(DiContainer container);
        public abstract IUiAction SetState(EWindowState state);
        public abstract void ApplyOrder(int order);
    }
}
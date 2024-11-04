using Cysharp.Threading.Tasks;
using KoboldUi.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace KoboldUi.Windows
{
    public abstract class AWindowBase : MonoBehaviour, IWindow, IInitializable
    {
        private readonly ReactiveCommand initialized = new();

        public virtual void Initialize()
        {
            IsInitialized = true;
            initialized.Execute();
        }

        public bool IsInitialized { get; private set; }
        public virtual string Name => gameObject.name;

        public UniTask WaitInitialization() => initialized.ToUniTask();
        
        public abstract void InstallBindings(DiContainer container);
        public abstract UniTask SetState(EWindowState state);
        public abstract void ApplyOrder(int order);
    }
}
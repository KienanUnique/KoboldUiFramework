using Cysharp.Threading.Tasks;
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

        public UniTask WaitInitialization() => UniTask.WaitUntil(() => IsInitialized,
            cancellationToken: this.GetCancellationTokenOnDestroy());
        
        public abstract void InstallBindings(DiContainer container);
        public abstract UniTask SetState(EWindowState state);
        public abstract void ApplyOrder(int order);
    }
}
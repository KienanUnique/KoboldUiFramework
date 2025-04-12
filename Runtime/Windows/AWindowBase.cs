using Cysharp.Threading.Tasks;
using KoboldUi.Utils;
using UnityEngine;

#if KOBOLD_ZENJECT_SUPPORT
using Zenject;
#elif KOBOLD_VCONTAINER_SUPPORT
using VContainer;
using VContainer.Unity;
#endif

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
        
#if KOBOLD_ZENJECT_SUPPORT
        public abstract void InstallBindings(DiContainer container);
#elif KOBOLD_VCONTAINER_SUPPORT
        public abstract void InstallBindings(IObjectResolver container);
#endif

        public abstract UniTask SetState(EWindowState state);
        public abstract void ApplyOrder(int order);
    }
}
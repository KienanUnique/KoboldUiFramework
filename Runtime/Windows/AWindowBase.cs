using KoboldUi.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace KoboldUi.Windows
{
    public abstract class AWindowBase : MonoBehaviour, IWindow, IInitializable
    {
        private readonly ReactiveProperty<bool> _isInitialized = new(false);

        public virtual void Initialize()
        {
            _isInitialized.Value = true;
        }

        public IReactiveProperty<bool> IsInitialized => _isInitialized;
        public virtual string Name => gameObject.name;

        public abstract void InstallBindings(DiContainer container);

        public abstract void SetState(EWindowState state);
        public abstract void ApplyOrder(int order);
    }
}
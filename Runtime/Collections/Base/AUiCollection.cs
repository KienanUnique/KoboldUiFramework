using System.Collections;
using System.Collections.Generic;
using KoboldUi.Element.View;
using UnityEngine;

#if KOBOLD_ZENJECT_SUPPORT
using Zenject;
#elif KOBOLD_VCONTAINER_SUPPORT
using VContainer;
#endif

namespace KoboldUi.Collections.Base
{
    public abstract class AUiCollection<TView> : MonoBehaviour, IUiCollection<TView>
        where TView : MonoBehaviour, IUiView
    {
        [SerializeField] protected TView prefab;
        [SerializeField] protected Transform collectionContainer;

#if KOBOLD_ZENJECT_SUPPORT
        [Inject] protected IInstantiator Instantiator;
#elif KOBOLD_VCONTAINER_SUPPORT
        [Inject] protected IObjectResolver ObjectResolver;
#endif

        public abstract int Count { get; }

        protected virtual void OnCreated(TView view)
        {
            view.SetParent(collectionContainer);
            view.Open();
        }

        public abstract void Clear();

        public abstract IEnumerator<TView> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
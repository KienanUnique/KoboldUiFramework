using System.Collections;
using System.Collections.Generic;
using KoboldUi.Element.View;
using UnityEngine;
using Zenject;

namespace KoboldUi.Collections.Base
{
    public abstract class AUiCollection<TView> : MonoBehaviour, IUiCollectionBase<TView>
        where TView : MonoBehaviour, IUiView
    {
        [SerializeField] protected TView prefab; // TODO: add alchemy support
        [SerializeField] protected Transform collectionContainer;

        [Inject] protected IInstantiator Instantiator;
        
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
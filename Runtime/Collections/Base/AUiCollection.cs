using System.Collections;
using System.Collections.Generic;
using KoboldUi.Element.View;
using UnityEngine;
using Zenject;

namespace KoboldUi.Collections.Base
{
    public abstract class AUiCollection<TView> : MonoBehaviour, IUiCollection<TView>
        where TView : MonoBehaviour, IUiCollectionView
    {
        [SerializeField] protected TView prefab;
        [SerializeField] protected Transform collectionContainer;

        [Inject] protected IInstantiator Instantiator;
        
        public abstract int Count { get; }

        protected virtual void OnCreated(TView view)
        {
            view.SetParent(collectionContainer);
            view.Appear();
        }

        public abstract void Clear();

        public abstract IEnumerator<TView> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
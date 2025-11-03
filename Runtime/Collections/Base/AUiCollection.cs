using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace KoboldUi.Collections.Base
{
    /// <summary>
    /// Base behaviour for UI collections that manage pooled view instances.
    /// </summary>
    /// <typeparam name="TView">Type of view stored in the collection.</typeparam>
    public abstract class AUiCollection<TView> : MonoBehaviour, IUiCollection<TView>
        where TView : MonoBehaviour, IUiCollectionView
    {
        /// <summary>
        /// Prefab used to create new view instances for the collection.
        /// </summary>
        [SerializeField] protected TView prefab;
        /// <summary>
        /// Container transform that hosts instantiated views.
        /// </summary>
        [SerializeField] protected Transform collectionContainer;

        /// <summary>
        /// Instantiator used to spawn new view instances.
        /// </summary>
        [Inject] protected IInstantiator Instantiator;

        /// <inheritdoc />
        public abstract int Count { get; }

        /// <inheritdoc />
        public abstract void Clear();

        /// <inheritdoc />
        public abstract IEnumerator<TView> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Configures a view when it has been created or retrieved from the pool.
        /// </summary>
        /// <param name="view">View instance entering the collection.</param>
        protected virtual void OnCreated(TView view)
        {
            view.SetParent(collectionContainer);
            view.Appear();
        }
    }
}
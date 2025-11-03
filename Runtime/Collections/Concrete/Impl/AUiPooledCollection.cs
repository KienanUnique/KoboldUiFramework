using System.Collections.Generic;
using KoboldUi.Collections.Base;
using UnityEngine;

namespace KoboldUi.Collections.Concrete.Impl
{
    /// <summary>
    /// Base collection that reuses views via an internal pool.
    /// </summary>
    public abstract class AUiPooledCollection<TView> : AUiCollection<TView>, IUiPooledCollection<TView>
        where TView : MonoBehaviour, IUiCollectionView
    {
        private readonly List<TView> _pool = new();
        private readonly List<TView> _views = new();

        /// <inheritdoc />
        public override int Count => _pool.Count;

        /// <inheritdoc />
        public TView Create()
        {
            TView view;
            if (_pool.Count != 0)
            {
                view = _pool[0];
                _pool.RemoveAt(0);
            }
            else
            {
                view = Instantiator.InstantiatePrefabForComponent<TView>(prefab);
            }

            OnCreated(view);
            return view;
        }

        /// <inheritdoc />
        public void ReturnToPool(TView view)
        {
            _views.Remove(view);
            view.Disappear();

            OnReturnToPool(view);
            _pool.Add(view);
        }

        /// <inheritdoc />
        public override IEnumerator<TView> GetEnumerator()
        {
            return _views.GetEnumerator();
        }

        /// <inheritdoc />
        public override void Clear()
        {
            while (_views.Count > 0)
                ReturnToPool(_views[0]);
        }

        /// <inheritdoc />
        protected override void OnCreated(TView view)
        {
            base.OnCreated(view);
            _views.Add(view);
        }

        /// <summary>
        /// Allows derived classes to customize logic when a view returns to the pool.
        /// </summary>
        /// <param name="view">View being recycled.</param>
        protected virtual void OnReturnToPool(TView view)
        {
        }
    }
}
using System.Collections.Generic;
using KoboldUi.Collections.Base;
using UnityEngine;

namespace KoboldUi.Collections.Concrete.Impl
{
    /// <summary>
    /// Base collection that keeps an ordered list of views without pooling.
    /// </summary>
    public abstract class AUiListCollection<TView> : AUiCollection<TView>, IUiListCollection<TView>
        where TView : MonoBehaviour, IUiCollectionView
    {
        private readonly List<TView> _views = new();

        /// <inheritdoc />
        public override void Clear()
        {
            foreach (var item in _views)
                item.Destroy();
            _views.Clear();
        }

        /// <inheritdoc />
        public override int Count => _views.Count;

        /// <inheritdoc />
        public TView this[int index] => _views[index];

        /// <inheritdoc />
        public void Remove(TView view)
        {
            var indexOf = _views.IndexOf(view);
            RemoveAt(indexOf);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            var view = _views[index];
            view.Destroy();
            _views.RemoveAt(index);
        }

        /// <inheritdoc />
        public override IEnumerator<TView> GetEnumerator()
        {
            return _views.GetEnumerator();
        }

        /// <inheritdoc />
        public TView Create()
        {
            var view = Instantiator.InstantiatePrefabForComponent<TView>(prefab);
            OnCreated(view);
            return view;
        }

        /// <inheritdoc />
        protected override void OnCreated(TView view)
        {
            base.OnCreated(view);
            _views.Add(view);
        }
    }
}
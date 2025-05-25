using System.Collections.Generic;
using KoboldUi.Collections.Base;
using UnityEngine;

namespace KoboldUi.Collections.Concrete.Impl
{
    public abstract class AUiListCollection<TView> : AUiCollection<TView>, IUiListCollection<TView>
        where TView : MonoBehaviour, IUiCollectionView
    {
        private readonly List<TView> _views = new();

        public override void Clear()
        {
            foreach (var item in _views)
                item.Destroy();
            _views.Clear();
        }

        public override int Count => _views.Count;

        public TView this[int index] => _views[index];

        public void Remove(TView view)
        {
            var indexOf = _views.IndexOf(view);
            RemoveAt(indexOf);
        }

        public void RemoveAt(int index)
        {
            var view = _views[index];
            view.Destroy();
            _views.RemoveAt(index);
        }

        public override IEnumerator<TView> GetEnumerator()
        {
            return _views.GetEnumerator();
        }

        public TView Create()
        {
            var view = Instantiator.InstantiatePrefabForComponent<TView>(prefab);
            OnCreated(view);
            return view;
        }

        protected override void OnCreated(TView view)
        {
            base.OnCreated(view);
            _views.Add(view);
        }
    }
}
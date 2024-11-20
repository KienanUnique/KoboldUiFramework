using System.Collections.Generic;
using KoboldUi.Collections.Base;
using KoboldUi.Element.View;
using UnityEngine;

namespace KoboldUi.Collections.Concrete.Impl
{
    public abstract class AUiPooledCollection<TView> : AUiCollection<TView>, IUiPooledCollection<TView>
        where TView : MonoBehaviour, IUiView
    {
        private readonly List<TView> _pool = new();
        private readonly List<TView> _views = new();

        public override int Count => _pool.Count;

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

        public void ReturnToPool(TView view)
        {
            _views.Remove(view);
            view.Close();

            OnReturnToPool(view);
            _pool.Add(view);
        }

        public override IEnumerator<TView> GetEnumerator() => _views.GetEnumerator();

        public override void Clear()
        {
            while (_views.Count > 0)
                ReturnToPool(_views[0]);
        }

        protected override void OnCreated(TView view)
        {
            base.OnCreated(view);
            _views.Add(view);
        }

        protected virtual void OnReturnToPool(TView view)
        {
        }
    }
}
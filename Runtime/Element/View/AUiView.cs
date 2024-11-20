using Cysharp.Threading.Tasks;
using UnityEngine;

namespace KoboldUi.Element.View
{
    public abstract class AUiView : MonoBehaviour, IUiView
    {
        public virtual UniTask Open() => OnOpen();

        public virtual UniTask ReturnFocus() => OnReturnFocus();

        public virtual UniTask RemoveFocus() => OnRemoveFocus();

        public virtual UniTask Close() => OnClose();

        public abstract void CloseInstantly();

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        protected virtual UniTask OnOpen()
        {
            return UniTask.NextFrame();
        }

        protected virtual UniTask OnReturnFocus()
        {
            return UniTask.NextFrame();
        }

        protected virtual UniTask OnRemoveFocus()
        {
            return UniTask.NextFrame();
        }

        protected virtual UniTask OnClose()
        {
            return UniTask.NextFrame();
        }
    }
}
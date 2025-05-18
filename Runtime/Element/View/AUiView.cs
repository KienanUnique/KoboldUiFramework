using Cysharp.Threading.Tasks;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Impl.Common;
using UnityEngine;

namespace KoboldUi.Element.View
{
    public abstract class AUiView : MonoBehaviour, IUiView
    {
        public virtual IUiAction Open() => OnOpen();

        public virtual IUiAction ReturnFocus() => OnReturnFocus();

        public virtual IUiAction RemoveFocus() => OnRemoveFocus();

        public virtual IUiAction Close() => OnClose();

        public abstract void CloseInstantly();

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        protected virtual IUiAction OnOpen()
        {
            return new EmptyAction();
        }

        protected virtual IUiAction OnReturnFocus()
        {
            return new EmptyAction();
        }

        protected virtual IUiAction OnRemoveFocus()
        {
            return new EmptyAction();
        }

        protected virtual IUiAction OnClose()
        {
            return new EmptyAction();
        }
    }
}
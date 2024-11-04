using UnityEngine;

namespace KoboldUi.Element.View
{
    public abstract class AUiView : MonoBehaviour, IUiView
    {
        public virtual void Open()
        {
            OnOpen();
        }
        
        public virtual void ReturnFocus()
        {
            OnReturnFocus();
        }
        
        public virtual void RemoveFocus()
        {
            OnRemoveFocus();
        }

        public virtual void Close()
        {
            OnClose();
        }

        public abstract void CloseInstantly();

        protected virtual void OnOpen()
        {
        }
        
        protected virtual void OnReturnFocus()
        {
        }
        
        protected virtual void OnRemoveFocus()
        {
        }
        
        protected virtual void OnClose()
        {
        }
    }
}
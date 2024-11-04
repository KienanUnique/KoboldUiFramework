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

        protected virtual UniTask OnOpen()
        {
            return UniTask.NextFrame(); // TODO: refactor this
        }
        
        protected virtual UniTask OnReturnFocus()
        {
            return UniTask.NextFrame(); // TODO: refactor this
        }
        
        protected virtual UniTask OnRemoveFocus()
        {
            return UniTask.NextFrame(); // TODO: refactor this
        }
        
        protected virtual UniTask OnClose()
        {
            return UniTask.NextFrame(); // TODO: refactor this
        }
    }
}
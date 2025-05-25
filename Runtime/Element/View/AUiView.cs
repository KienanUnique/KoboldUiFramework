using KoboldUi.UiAction;
using KoboldUi.UiAction.Impl.Common;
using KoboldUi.UiAction.Pool;
using UnityEngine;

namespace KoboldUi.Element.View
{
    public abstract class AUiView : MonoBehaviour, IUiView
    {
        public virtual void Initialize()
        {
        }

        public virtual IUiAction Open(in IUiActionsPool pool) => OnOpen(pool);

        public virtual IUiAction ReturnFocus(in IUiActionsPool pool) => OnReturnFocus(pool);

        public virtual IUiAction RemoveFocus(in IUiActionsPool pool) => OnRemoveFocus(pool);

        public virtual IUiAction Close(in IUiActionsPool pool) => OnClose(pool);

        public abstract void CloseInstantly();

        protected virtual IUiAction OnOpen(in IUiActionsPool pool)
        {
            pool.GetAction(out EmptyAction emptyAction);
            return emptyAction;
        }

        protected virtual IUiAction OnReturnFocus(in IUiActionsPool pool)
        {
            pool.GetAction(out EmptyAction emptyAction);
            return emptyAction;
        }

        protected virtual IUiAction OnRemoveFocus(in IUiActionsPool pool)
        {
            pool.GetAction(out EmptyAction emptyAction);
            return emptyAction;
        }

        protected virtual IUiAction OnClose(in IUiActionsPool pool)
        {
            pool.GetAction(out EmptyAction emptyAction);
            return emptyAction;
        }
    }
}
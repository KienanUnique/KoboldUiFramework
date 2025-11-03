using KoboldUi.UiAction;
using KoboldUi.UiAction.Impl.Common;
using KoboldUi.UiAction.Pool;
using UnityEngine;

namespace KoboldUi.Element.View
{
    /// <summary>
    /// Base implementation of a view that exposes lifecycle hooks tied to UI actions.
    /// </summary>
    public abstract class AUiView : MonoBehaviour, IUiView
    {
        /// <inheritdoc />
        public virtual void Initialize()
        {
        }

        /// <inheritdoc />
        public virtual IUiAction Open(in IUiActionsPool pool)
        {
            return OnOpen(pool);
        }

        /// <inheritdoc />
        public virtual IUiAction ReturnFocus(in IUiActionsPool pool)
        {
            return OnReturnFocus(pool);
        }

        /// <inheritdoc />
        public virtual IUiAction RemoveFocus(in IUiActionsPool pool)
        {
            return OnRemoveFocus(pool);
        }

        /// <inheritdoc />
        public virtual IUiAction Close(in IUiActionsPool pool)
        {
            return OnClose(pool);
        }

        /// <inheritdoc />
        public abstract void CloseInstantly();

        /// <summary>
        /// Creates the action used when the view is opened.
        /// </summary>
        /// <param name="pool">Pool used to obtain reusable actions.</param>
        /// <returns>An action describing the open transition.</returns>
        protected virtual IUiAction OnOpen(in IUiActionsPool pool)
        {
            pool.GetAction(out EmptyAction emptyAction);
            return emptyAction;
        }

        /// <summary>
        /// Creates the action used when the view regains focus.
        /// </summary>
        /// <param name="pool">Pool used to obtain reusable actions.</param>
        /// <returns>An action describing the focus transition.</returns>
        protected virtual IUiAction OnReturnFocus(in IUiActionsPool pool)
        {
            pool.GetAction(out EmptyAction emptyAction);
            return emptyAction;
        }

        /// <summary>
        /// Creates the action used when the view loses focus.
        /// </summary>
        /// <param name="pool">Pool used to obtain reusable actions.</param>
        /// <returns>An action describing the defocus transition.</returns>
        protected virtual IUiAction OnRemoveFocus(in IUiActionsPool pool)
        {
            pool.GetAction(out EmptyAction emptyAction);
            return emptyAction;
        }

        /// <summary>
        /// Creates the action used when the view closes.
        /// </summary>
        /// <param name="pool">Pool used to obtain reusable actions.</param>
        /// <returns>An action describing the close transition.</returns>
        protected virtual IUiAction OnClose(in IUiActionsPool pool)
        {
            pool.GetAction(out EmptyAction emptyAction);
            return emptyAction;
        }
    }
}
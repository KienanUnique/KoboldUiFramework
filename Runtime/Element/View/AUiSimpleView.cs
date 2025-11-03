using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;

namespace KoboldUi.Element.View
{
    /// <summary>
    /// Simple view implementation that toggles the GameObject when opening or closing.
    /// </summary>
    public class AUiSimpleView : AUiView
    {
        /// <inheritdoc />
        public sealed override IUiAction Open(in IUiActionsPool pool)
        {
            gameObject.SetActive(true);
            return base.Open(pool);
        }

        /// <inheritdoc />
        public sealed override IUiAction ReturnFocus(in IUiActionsPool pool)
        {
            return base.ReturnFocus(pool);
        }

        /// <inheritdoc />
        public sealed override IUiAction RemoveFocus(in IUiActionsPool pool)
        {
            return base.RemoveFocus(pool);
        }

        /// <inheritdoc />
        public sealed override IUiAction Close(in IUiActionsPool pool)
        {
            gameObject.SetActive(false);
            return base.Close(pool);
        }

        /// <inheritdoc />
        public sealed override void CloseInstantly()
        {
            gameObject.SetActive(false);
        }
    }
}
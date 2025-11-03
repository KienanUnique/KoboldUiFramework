using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;

namespace KoboldUi.Element.View
{
    /// <summary>
    /// Describes the lifecycle contract for UI views controlled by the framework.
    /// </summary>
    public interface IUiView
    {
        /// <summary>
        /// Initializes the view after it has been constructed or injected.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Creates an action that opens the view.
        /// </summary>
        /// <param name="pool">Pool used to retrieve reusable actions.</param>
        /// <returns>The action representing the open transition.</returns>
        IUiAction Open(in IUiActionsPool pool);

        /// <summary>
        /// Creates an action that returns focus to the view.
        /// </summary>
        /// <param name="pool">Pool used to retrieve reusable actions.</param>
        /// <returns>The action representing the focus transition.</returns>
        IUiAction ReturnFocus(in IUiActionsPool pool);

        /// <summary>
        /// Creates an action that removes focus from the view.
        /// </summary>
        /// <param name="pool">Pool used to retrieve reusable actions.</param>
        /// <returns>The action representing the defocus transition.</returns>
        IUiAction RemoveFocus(in IUiActionsPool pool);

        /// <summary>
        /// Creates an action that closes the view.
        /// </summary>
        /// <param name="pool">Pool used to retrieve reusable actions.</param>
        /// <returns>The action representing the close transition.</returns>
        IUiAction Close(in IUiActionsPool pool);

        /// <summary>
        /// Immediately closes the view without animations.
        /// </summary>
        void CloseInstantly();
    }
}

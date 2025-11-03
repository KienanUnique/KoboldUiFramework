using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;

namespace KoboldUi.Element.Controller
{
    /// <summary>
    /// Defines how controllers drive view state transitions.
    /// </summary>
    public interface IUIController
    {
        /// <summary>
        /// Applies the requested window state and returns the action to perform the transition.
        /// </summary>
        /// <param name="state">Desired state for the window.</param>
        /// <param name="pool">Pool used to acquire reusable actions.</param>
        /// <returns>The action representing the transition.</returns>
        IUiAction SetState(EWindowState state, in IUiActionsPool pool);
    }
}

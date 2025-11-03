using KoboldUi.UiAction;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;

namespace KoboldUi.Windows
{
    /// <summary>
    /// Describes the contract that window implementations must follow.
    /// </summary>
    public interface IWindow
    {
        /// <summary>
        /// Gets a value indicating whether the window has been initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Gets a human-readable name for the window.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the window is treated as a popup.
        /// </summary>
        bool IsPopup { get; }

        /// <summary>
        /// Gets a value indicating whether the window can be skipped during back navigation.
        /// </summary>
        bool IsBackLogicIgnorable { get; }

        /// <summary>
        /// Creates an action that waits for the window to finish initialization.
        /// </summary>
        /// <param name="pool">Pool used to obtain the action instance.</param>
        IUiAction WaitInitialization(in IUiActionsPool pool);

        /// <summary>
        /// Creates an action that transitions the window to the specified state.
        /// </summary>
        /// <param name="state">Desired window state.</param>
        /// <param name="pool">Pool used to obtain the action instance.</param>
        IUiAction SetState(EWindowState state, in IUiActionsPool pool);

        /// <summary>
        /// Applies the sorting order for the window within the stack.
        /// </summary>
        /// <param name="order">Zero-based order index.</param>
        void ApplyOrder(int order);
    }
}

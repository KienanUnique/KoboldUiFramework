namespace KoboldUi.Utils
{
    /// <summary>
    /// Represents the possible states of a window within the stack.
    /// </summary>
    public enum EWindowState
    {
        /// <summary>
        /// The window is visible and has focus.
        /// </summary>
        Active,
        /// <summary>
        /// The window is visible but does not have focus.
        /// </summary>
        NonFocused,
        /// <summary>
        /// The window is closed and not visible.
        /// </summary>
        Closed
    }
}

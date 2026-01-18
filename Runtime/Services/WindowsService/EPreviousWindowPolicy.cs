namespace KoboldUi.Services.WindowsService
{
    /// <summary>
    /// Defines how the previously active window should be handled when opening a new one.
    /// </summary>
    public enum EPreviousWindowPolicy
    {
        /// <summary>
        /// Keeps the current behavior.
        /// </summary>
        Default,
        /// <summary>
        /// Closes the previous window and removes it from the navigation stack.
        /// </summary>
        CloseAndForget,
        /// <summary>
        /// Opens the new window first, then closes and removes the previous one.
        /// </summary>
        CloseAfterOpenAndForget
    }
}

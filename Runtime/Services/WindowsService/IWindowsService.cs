using System;
using KoboldUi.Windows;

namespace KoboldUi.Services.WindowsService
{
    /// <summary>
    /// Provides high-level APIs for opening and closing windows.
    /// </summary>
    public interface IWindowsService
    {
        /// <summary>
        /// Gets the window currently at the top of the stack.
        /// </summary>
        IWindow CurrentWindow { get; }

        /// <summary>
        /// Checks whether a window of the specified type is opened.
        /// </summary>
        /// <typeparam name="TWindow">Window type to look for.</typeparam>
        bool IsOpened<TWindow>() where TWindow : IWindow;

        /// <summary>
        /// Opens the specified window type.
        /// </summary>
        /// <typeparam name="TWindow">Window type to open.</typeparam>
        /// <param name="onComplete">Optional callback invoked after the transition completes.</param>
        /// <param name="previousWindowPolicy">Controls how the previously active window is handled.</param>
        void OpenWindow<TWindow>(Action onComplete = null, EPreviousWindowPolicy previousWindowPolicy = EPreviousWindowPolicy.Default)
            where TWindow : IWindow;

        /// <summary>
        /// Closes the current window.
        /// </summary>
        /// <param name="onComplete">Optional callback invoked after the transition completes.</param>
        /// <param name="useBackLogicIgnorableChecks">When true, respects back-logic ignore flags.</param>
        void CloseWindow(Action onComplete = null, bool useBackLogicIgnorableChecks = true);

        /// <summary>
        /// Closes windows until a window of the specified type becomes active.
        /// </summary>
        /// <typeparam name="TWindow">Target window type to remain on top.</typeparam>
        /// <param name="onComplete">Optional callback invoked after the transition completes.</param>
        /// <param name="useBackLogicIgnorableChecks">When true, respects back-logic ignore flags.</param>
        void CloseToWindow<TWindow>(Action onComplete = null, bool useBackLogicIgnorableChecks = true);
    }
}

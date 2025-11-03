using System.Collections.Generic;
using KoboldUi.Windows;

namespace KoboldUi.WindowsStack
{
    /// <summary>
    /// Provides access to the stack of currently opened windows.
    /// </summary>
    public interface IWindowsStackHolder
    {
        /// <summary>
        /// Gets the window at the top of the stack.
        /// </summary>
        IWindow CurrentWindow { get; }

        /// <summary>
        /// Gets a value indicating whether the stack has no windows.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets a read-only view of the internal stack.
        /// </summary>
        IReadOnlyCollection<IWindow> Stack { get; }

        /// <summary>
        /// Checks whether a window of the specified type is opened.
        /// </summary>
        /// <typeparam name="TWindow">Window type to look for.</typeparam>
        bool IsOpened<TWindow>() where TWindow : IWindow;

        /// <summary>
        /// Checks whether the provided window instance is opened.
        /// </summary>
        /// <param name="window">Window to look for.</param>
        bool IsOpened(IWindow window);

        /// <summary>
        /// Pushes a window onto the stack.
        /// </summary>
        /// <param name="window">Window to push.</param>
        void Push(IWindow window);

        /// <summary>
        /// Pops the top window from the stack.
        /// </summary>
        /// <returns>The removed window.</returns>
        IWindow Pop();

        /// <summary>
        /// Determines whether the specified window exists in the stack.
        /// </summary>
        /// <param name="windowToClose">Window to find.</param>
        bool Contains(IWindow windowToClose);
    }
}

using System.Collections.Generic;
using KoboldUi.Windows;

namespace KoboldUi.Utils
{
    /// <summary>
    /// Helper methods for maintaining window sibling order based on the stack.
    /// </summary>
    public static class WindowsOrdersManager
    {
        /// <summary>
        /// Assigns the correct order to a window when it appears.
        /// </summary>
        /// <param name="openedWindowsStack">Current stack of opened windows.</param>
        /// <param name="openedWindow">Window that just opened.</param>
        public static void HandleWindowAppear(IReadOnlyCollection<IWindow> openedWindowsStack, IWindow openedWindow)
        {
            var newOrder = openedWindowsStack.Count;
            openedWindow.ApplyOrder(newOrder);
        }

        /// <summary>
        /// Reserved for future logic when a window disappears.
        /// </summary>
        /// <param name="openedWindowsStack">Current stack of opened windows.</param>
        /// <param name="closedWindow">Window that has closed.</param>
        public static void HandleWindowDisappear(IReadOnlyCollection<IWindow> openedWindowsStack, IWindow closedWindow)
        {
        }

        /// <summary>
        /// Reassigns sibling indexes for all windows in the stack.
        /// </summary>
        /// <param name="openedWindowsStack">Current stack of opened windows.</param>
        public static void UpdateWindowsLayers(IReadOnlyCollection<IWindow> openedWindowsStack)
        {
            var orderIndex = 0;
            foreach (var window in openedWindowsStack)
            {
                window.ApplyOrder(orderIndex);
                orderIndex++;
            }
        }
    }
}
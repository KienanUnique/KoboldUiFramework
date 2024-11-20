using System.Collections.Generic;
using KoboldUi.Windows;

namespace KoboldUi.Utils
{
    public static class WindowsOrdersManager
    {
        public static void HandleWindowAppear(IReadOnlyCollection<IWindow> openedWindowsStack, IWindow openedWindow)
        {
            var newOrder = openedWindowsStack.Count;
            openedWindow.ApplyOrder(newOrder);
        }

        public static void HandleWindowDisappear(IReadOnlyCollection<IWindow> openedWindowsStack, IWindow closedWindow)
        {
        }

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
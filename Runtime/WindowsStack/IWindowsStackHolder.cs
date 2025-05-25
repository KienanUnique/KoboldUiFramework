using System.Collections.Generic;
using KoboldUi.Windows;

namespace KoboldUi.WindowsStack
{
    public interface IWindowsStackHolder
    {
        IWindow CurrentWindow { get; }
        bool IsEmpty { get; }
        IReadOnlyCollection<IWindow> Stack { get; }

        bool IsOpened<TWindow>() where TWindow : IWindow;
        bool IsOpened(IWindow window);
        void Push(IWindow window);
        IWindow Pop();
    }
}
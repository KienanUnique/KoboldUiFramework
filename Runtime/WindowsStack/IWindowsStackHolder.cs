using KoboldUi.Windows;

namespace KoboldUi.WindowsStack
{
    public interface IWindowsStackHolder
    {
        IWindow CurrentWindow { get; }
        bool IsOpened<TWindow>() where TWindow : IWindow;
        void HandleWindowOpen(IWindow window);
        void HandleWindowClose(IWindow window);
    }
}
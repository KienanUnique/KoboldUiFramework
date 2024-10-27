using KoboldUi.Windows;

namespace KoboldUi.Services.WindowsService
{
    public interface IWindowsService
    {
        IWindow CurrentWindow { get; }
        
        void OpenWindow<TWindow>() where TWindow : IWindow;
        bool TryBackWindow();
        bool TryBackToWindow<TWindow>();
        bool TryBackWindows(int countOfWindowsToClose);
        
        void CloseWindow();
        void CloseToWindow<TWindow>();
        void CloseWindows(int countOfWindowsToClose);
    }
}
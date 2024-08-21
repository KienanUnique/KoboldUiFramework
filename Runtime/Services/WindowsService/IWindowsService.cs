using KoboldUi.Windows;

namespace KoboldUi.Services.WindowsService
{
    public interface IWindowsService
    {
        IWindow CurrentWindow { get; }
        
        void OpenWindow<TWindow>() where TWindow : IWindow;
        void BackWindow();
        void ForceCloseWindow();
    }
}
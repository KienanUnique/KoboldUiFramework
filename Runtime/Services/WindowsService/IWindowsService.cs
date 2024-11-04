using System;
using KoboldUi.Windows;

namespace KoboldUi.Services.WindowsService
{
    public interface IWindowsService
    {
        IWindow CurrentWindow { get; }
        
        void OpenWindow<TWindow>(Action onComplete = default) where TWindow : IWindow;
        void TryBackWindow(Action<bool> onComplete = default);
        void TryBackToWindow<TWindow>(Action<bool> onComplete = default);
        void TryBackWindows(int countOfWindowsToClose, Action<bool> onComplete = default);
        
        void CloseWindow(Action onComplete = default);
        void CloseToWindow<TWindow>(Action onComplete = default);
        void CloseWindows(int countOfWindowsToClose, Action onComplete = default);
    }
}
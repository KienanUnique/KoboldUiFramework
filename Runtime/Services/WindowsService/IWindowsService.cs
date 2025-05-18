using System;
using KoboldUi.Windows;

namespace KoboldUi.Services.WindowsService
{
    public interface IWindowsService
    {
        IWindow CurrentWindow { get; }
        bool IsOpened<TWindow>() where TWindow : IWindow;

        void OpenWindow<TWindow>(Action onComplete = null) where TWindow : IWindow;

        void TryBackWindow(Action<bool> onComplete = null);
        void TryBackToWindow<TWindow>(Action<bool> onComplete = null);
        
        void CloseWindow(Action onComplete = null);
        void CloseToWindow<TWindow>(Action onComplete = null) where TWindow : IWindow;
    }
}
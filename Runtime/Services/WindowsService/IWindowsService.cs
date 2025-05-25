using System;
using KoboldUi.Windows;

namespace KoboldUi.Services.WindowsService
{
    public interface IWindowsService
    {
        IWindow CurrentWindow { get; }
        bool IsOpened<TWindow>() where TWindow : IWindow;

        void OpenWindow<TWindow>(Action onComplete = null) where TWindow : IWindow;

        void CloseWindow(Action onComplete = null, bool useBackLogicIgnorableChecks = true);
        void CloseToWindow<TWindow>(Action onComplete = null, bool useBackLogicIgnorableChecks = true);
    }
}
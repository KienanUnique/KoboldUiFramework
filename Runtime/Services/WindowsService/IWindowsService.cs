using System;
using KoboldUi.Utils;
using KoboldUi.Windows;

namespace KoboldUi.Services.WindowsService
{
    public interface IWindowsService
    {
        IWindow CurrentWindow { get; }

        void OpenWindow<TWindow>(Action onComplete = default,
            EAnimationPolitic previousWindowPolitic = EAnimationPolitic.Wait) where TWindow : IWindow;

        void TryBackWindow(Action<bool> onComplete = default,
            EAnimationPolitic previousWindowPolitic = EAnimationPolitic.Wait);

        void TryBackToWindow<TWindow>(Action<bool> onComplete = default,
            EAnimationPolitic previousWindowsPolitic = EAnimationPolitic.Wait);

        void TryBackWindows(int countOfWindowsToClose, Action<bool> onComplete = default,
            EAnimationPolitic previousWindowsPolitic = EAnimationPolitic.Wait);

        void CloseWindow(Action onComplete = default,
            EAnimationPolitic previousWindowPolitic = EAnimationPolitic.Wait);

        void CloseToWindow<TWindow>(Action onComplete = default,
            EAnimationPolitic previousWindowsPolitic = EAnimationPolitic.Wait) where TWindow : IWindow;

        void CloseWindows(int countOfWindowsToClose, Action onComplete = default,
            EAnimationPolitic previousWindowsPolitic = EAnimationPolitic.Wait);
    }
}
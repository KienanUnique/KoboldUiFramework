using System;
using KoboldUi.Utils;
using KoboldUi.Windows;

namespace KoboldUi.Services.WindowsService
{
    public interface IWindowsService
    {
        IWindow CurrentWindow { get; }
        bool IsOpened<TWindow>() where TWindow : IWindow;

        void OpenWindow<TWindow>(Action onComplete = default,
            EAnimationPolitic previousWindowPolitic = EAnimationPolitic.DoNotWait) where TWindow : IWindow;

        void TryBackWindow(Action<bool> onComplete = default,
            EAnimationPolitic previousWindowPolitic = EAnimationPolitic.DoNotWait);

        void TryBackToWindow<TWindow>(Action<bool> onComplete = default,
            EAnimationPolitic previousWindowsPolitic = EAnimationPolitic.DoNotWait);

        void CloseWindow(Action onComplete = default,
            EAnimationPolitic previousWindowPolitic = EAnimationPolitic.DoNotWait);

        void CloseToWindow<TWindow>(Action onComplete = default,
            EAnimationPolitic previousWindowsPolitic = EAnimationPolitic.DoNotWait) where TWindow : IWindow;
    }
}
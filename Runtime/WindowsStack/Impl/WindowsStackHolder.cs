using System.Collections.Generic;
using KoboldUi.Windows;

namespace KoboldUi.WindowsStack.Impl
{
    public class WindowsStackHolder : IWindowsStackHolder
    {
        private readonly Stack<IWindow> _windowsStack = new();

        public IWindow CurrentWindow => IsEmpty ? null : _windowsStack.Peek();
        public bool IsEmpty => _windowsStack.Count <= 0;
        public IReadOnlyCollection<IWindow> Stack => _windowsStack;

        public bool IsOpened<TWindow>() where TWindow : IWindow
        {
            return !IsEmpty && _windowsStack.Peek() is TWindow;
        }

        public bool IsOpened(IWindow window)
        {
            return !IsEmpty && _windowsStack.Peek() == window;
        }

        public void Push(IWindow window)
        {
            _windowsStack.Push(window);
        }

        public IWindow Pop()
        {
            return _windowsStack.Pop();
        }
    }
}
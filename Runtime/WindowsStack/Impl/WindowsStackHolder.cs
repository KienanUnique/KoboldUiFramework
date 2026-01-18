using System.Collections.Generic;
using KoboldUi.Windows;

namespace KoboldUi.WindowsStack.Impl
{
    /// <summary>
    /// Default implementation that tracks windows using a stack.
    /// </summary>
    public class WindowsStackHolder : IWindowsStackHolder
    {
        private readonly Stack<IWindow> _windowsStack = new();

        /// <inheritdoc />
        public IWindow CurrentWindow => IsEmpty ? null : _windowsStack.Peek();
        /// <inheritdoc />
        public bool IsEmpty => _windowsStack.Count <= 0;
        /// <inheritdoc />
        public IReadOnlyCollection<IWindow> Stack => _windowsStack;

        /// <inheritdoc />
        public bool IsOpened<TWindow>() where TWindow : IWindow
        {
            return !IsEmpty && _windowsStack.Peek() is TWindow;
        }

        /// <inheritdoc />
        public bool IsOpened(IWindow window)
        {
            return !IsEmpty && _windowsStack.Peek() == window;
        }

        /// <inheritdoc />
        public void Push(IWindow window)
        {
            _windowsStack.Push(window);
        }

        /// <inheritdoc />
        public IWindow Pop()
        {
            return _windowsStack.Pop();
        }

        /// <inheritdoc />
        public bool Remove(IWindow window)
        {
            if (window == null || _windowsStack.Count == 0)
                return false;

            var removed = false;
            var tempStack = new Stack<IWindow>(_windowsStack.Count);

            while (_windowsStack.Count > 0)
            {
                var current = _windowsStack.Pop();
                if (!removed && current == window)
                {
                    removed = true;
                    continue;
                }

                tempStack.Push(current);
            }

            while (tempStack.Count > 0)
                _windowsStack.Push(tempStack.Pop());

            return removed;
        }

        /// <inheritdoc />
        public bool Contains(IWindow windowToClose)
        {
            return _windowsStack.Contains(windowToClose);
        }
    }
}

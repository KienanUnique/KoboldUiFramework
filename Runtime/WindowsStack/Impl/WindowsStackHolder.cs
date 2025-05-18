using System.Collections.Generic;
using KoboldUi.Windows;

namespace KoboldUi.WindowsStack.Impl
{
    public class WindowsStackHolder : IWindowsStackHolder
    {
        private readonly Stack<IWindow> _windowsStack = new();
        
        public IWindow CurrentWindow => _windowsStack.Count > 0 ? _windowsStack.Peek() : null;
        
        public bool IsOpened<TWindow>() where TWindow : IWindow
        {
            return _windowsStack.Count > 0 && _windowsStack.Peek() is TWindow;
        }
        
        public void HandleWindowOpen(IWindow window)
        {
            
        }

        public void HandleWindowClose(IWindow window)
        {
            
        }
    }
}
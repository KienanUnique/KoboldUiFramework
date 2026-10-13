using System;
using System.Collections.Generic;
using KoboldUi.Windows;

namespace KoboldUi.Composition
{
    public sealed class WindowRegistry
    {
        private readonly Dictionary<Type, IWindow> _windows = new Dictionary<Type, IWindow>();

        public void Register(IWindow window)
        {
            if (window == null) throw new ArgumentNullException(nameof(window));
            var type = window.GetType();
            if (_windows.ContainsKey(type))
                throw new InvalidOperationException($"Window {type.FullName} is already registered.");
            _windows.Add(type, window);
        }

        public IWindow Get(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (_windows.TryGetValue(type, out var exact)) return exact;
            IWindow match = null;
            foreach (var entry in _windows)
            {
                if (!type.IsAssignableFrom(entry.Key)) continue;
                if (match != null)
                    throw new InvalidOperationException($"Several registered windows implement {type.FullName}.");
                match = entry.Value;
            }
            return match ?? throw new InvalidOperationException($"Window {type.FullName} is not registered.");
        }

        public TWindow Get<TWindow>() where TWindow : IWindow => (TWindow)Get(typeof(TWindow));

        public bool Contains(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (_windows.ContainsKey(type)) return true;
            foreach (var registered in _windows.Keys)
                if (type.IsAssignableFrom(registered)) return true;
            return false;
        }

        public void Clear() => _windows.Clear();
    }
}

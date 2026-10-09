using System;
using System.Collections.Generic;
using KoboldUi.Windows;
using UnityEngine;

namespace KoboldUi.Composition
{
    public enum WindowScope { Local, Project }

    [Serializable]
    public sealed class WindowCatalogEntry
    {
        [SerializeField] private AWindowBase prefab;
        [SerializeField] private WindowScope scope;
        public AWindowBase Prefab => prefab;
        public WindowScope Scope => scope;
        public WindowCatalogEntry(AWindowBase prefab, WindowScope scope) { this.prefab = prefab; this.scope = scope; }
    }

    [CreateAssetMenu(menuName = "KoboldUi/Window Catalog", fileName = "WindowCatalog")]
    public sealed class WindowCatalog : ScriptableObject
    {
        [SerializeField] private List<WindowCatalogEntry> windows = new List<WindowCatalogEntry>();
        public IReadOnlyList<WindowCatalogEntry> Windows => windows;
        public void Add(AWindowBase prefab, WindowScope scope)
        {
            if (prefab == null) throw new ArgumentNullException(nameof(prefab));
            foreach (var entry in windows)
                if (entry.Prefab != null && entry.Prefab.GetType() == prefab.GetType())
                    throw new InvalidOperationException($"Window {prefab.GetType().FullName} is already in the catalog.");
            windows.Add(new WindowCatalogEntry(prefab, scope));
        }
    }
}

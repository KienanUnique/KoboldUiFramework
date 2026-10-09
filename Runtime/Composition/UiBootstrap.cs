using System;
using System.Collections.Generic;
using KoboldUi.Services.WindowsService;
using KoboldUi.Services.WindowsService.Impl;
using KoboldUi.Windows;
using UnityEngine;

namespace KoboldUi.Composition
{
    public sealed class UiBootstrap : IDisposable
    {
        private readonly Canvas _localCanvas;
        private readonly Canvas _projectCanvas;
        private readonly WindowRegistry _localRegistry = new WindowRegistry();
        private readonly WindowRegistry _projectRegistry = new WindowRegistry();
        private readonly List<AWindowBase> _instances = new List<AWindowBase>();
        private bool _initialized;
        public ILocalWindowsService LocalWindows { get; private set; }
        public IProjectWindowsService ProjectWindows { get; private set; }

        public UiBootstrap(Canvas localCanvas, Canvas projectCanvas)
        {
            _localCanvas = localCanvas ?? throw new ArgumentNullException(nameof(localCanvas));
            _projectCanvas = projectCanvas ?? throw new ArgumentNullException(nameof(projectCanvas));
        }

        public void Initialize(WindowCatalog catalog, IUiResolver resolver) => Initialize(catalog, resolver, resolver);

        public void Initialize(WindowCatalog catalog, IUiResolver projectResolver, IUiResolver localResolver)
        {
            if (_initialized) throw new InvalidOperationException("UiBootstrap is already initialized.");
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            if (projectResolver == null) throw new ArgumentNullException(nameof(projectResolver));
            if (localResolver == null) throw new ArgumentNullException(nameof(localResolver));
            Validate(catalog);
            LocalWindows = new LocalWindowsService(_localRegistry);
            ProjectWindows = new ProjectWindowsService(_projectRegistry);
            var localActivator = new UiActivator(new ServiceResolver(localResolver, LocalWindows, ProjectWindows));
            var projectActivator = new UiActivator(new ServiceResolver(projectResolver, LocalWindows, ProjectWindows));
            var staging = new GameObject("KoboldUi window staging");
            staging.SetActive(false);
            try
            {
                foreach (var entry in catalog.Windows)
                {
                    var project = entry.Scope == WindowScope.Project;
                    var window = UnityEngine.Object.Instantiate(entry.Prefab, staging.transform);
                    _instances.Add(window);
                    (project ? _projectRegistry : _localRegistry).Register(window);
                    window.Configure(project ? projectActivator : localActivator);
                }
                for (var i = 0; i < _instances.Count; i++)
                {
                    var activator = catalog.Windows[i].Scope == WindowScope.Project ? projectActivator : localActivator;
                    activator.InjectGameObject(_instances[i].gameObject);
                }
                for (var i = 0; i < _instances.Count; i++)
                {
                    var canvas = catalog.Windows[i].Scope == WindowScope.Project ? _projectCanvas : _localCanvas;
                    _instances[i].transform.SetParent(canvas.transform, false);
                    _instances[i].Initialize();
                }
                _initialized = true;
            }
            catch { Dispose(); throw; }
            finally
            {
                if (Application.isPlaying) UnityEngine.Object.Destroy(staging);
                else UnityEngine.Object.DestroyImmediate(staging);
            }
        }

        public void Open<TWindow>() where TWindow : IWindow
        {
            if (!_initialized) throw new InvalidOperationException("UiBootstrap is not initialized.");
            if (_localRegistry.Contains(typeof(TWindow))) LocalWindows.OpenWindow<TWindow>();
            else { _projectRegistry.Get<TWindow>(); ProjectWindows.OpenWindow<TWindow>(); }
        }

        public TWindow Get<TWindow>() where TWindow : IWindow => _localRegistry.Contains(typeof(TWindow))
            ? _localRegistry.Get<TWindow>() : _projectRegistry.Get<TWindow>();

        public void Dispose()
        {
            (LocalWindows as IDisposable)?.Dispose();
            (ProjectWindows as IDisposable)?.Dispose();
            foreach (var window in _instances)
            {
                if (window == null) continue;
                if (Application.isPlaying) UnityEngine.Object.Destroy(window.gameObject);
                else UnityEngine.Object.DestroyImmediate(window.gameObject);
            }
            _instances.Clear();
            _localRegistry.Clear();
            _projectRegistry.Clear();
            _initialized = false;
        }

        private static void Validate(WindowCatalog catalog)
        {
            var types = new HashSet<Type>();
            foreach (var entry in catalog.Windows)
            {
                if (entry == null || entry.Prefab == null)
                    throw new InvalidOperationException("WindowCatalog contains an empty prefab entry.");
                if (!types.Add(entry.Prefab.GetType()))
                    throw new InvalidOperationException($"WindowCatalog contains duplicate {entry.Prefab.GetType().FullName}.");
            }
        }

        private sealed class ServiceResolver : IUiResolver
        {
            private readonly IUiResolver _inner;
            private readonly ILocalWindowsService _local;
            private readonly IProjectWindowsService _project;
            public ServiceResolver(IUiResolver inner, ILocalWindowsService local, IProjectWindowsService project)
            { _inner = inner; _local = local; _project = project; }
            public bool TryResolve(Type type, out object value)
            {
                if (type == typeof(ILocalWindowsService)) { value = _local; return true; }
                if (type == typeof(IProjectWindowsService)) { value = _project; return true; }
                return _inner.TryResolve(type, out value);
            }
        }
    }
}

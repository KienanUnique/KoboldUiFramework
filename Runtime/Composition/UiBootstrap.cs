using System;
using System.Collections.Generic;
using KoboldUi.Element.Animations;
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
        private readonly IUiCompositionAdapter _projectAdapter;
        private readonly IUiCompositionAdapter _localAdapter;
        private readonly UiAnimationDefaults _animationDefaults;
        private readonly WindowRegistry _localRegistry = new WindowRegistry();
        private readonly WindowRegistry _projectRegistry = new WindowRegistry();
        private readonly List<WindowRegistration> _registrations = new List<WindowRegistration>();
        private readonly List<AWindowBase> _instances = new List<AWindowBase>();
        private readonly ILocalWindowsService _localWindows;
        private readonly IProjectWindowsService _projectWindows;
        private bool _built;
        private bool _building;
        private bool _disposed;

        public ILocalWindowsService LocalWindows
        {
            get { EnsureBuilt(); return _localWindows; }
        }

        public IProjectWindowsService ProjectWindows
        {
            get { EnsureBuilt(); return _projectWindows; }
        }

        public UiBootstrap(Canvas localCanvas, Canvas projectCanvas,
            IUiCompositionAdapter adapter = null, UiAnimationDefaults animationDefaults = null)
            : this(localCanvas, projectCanvas, adapter, adapter, animationDefaults)
        {
        }

        public UiBootstrap(Canvas localCanvas, Canvas projectCanvas, IUiCompositionAdapter projectAdapter,
            IUiCompositionAdapter localAdapter, UiAnimationDefaults animationDefaults)
        {
            _localCanvas = localCanvas ?? throw new ArgumentNullException(nameof(localCanvas));
            _projectCanvas = projectCanvas ?? throw new ArgumentNullException(nameof(projectCanvas));
            _projectAdapter = projectAdapter;
            _localAdapter = localAdapter;
            _animationDefaults = animationDefaults;
            _localWindows = new LocalWindowsService(_localRegistry);
            _projectWindows = new ProjectWindowsService(_projectRegistry);
        }

        public UiBootstrap RegisterWindow<TWindow>(TWindow prefab, WindowScope scope = WindowScope.Local,
            Action<TWindow> configure = null) where TWindow : AWindowBase
        {
            ThrowIfDisposed();
            if (_built || _building)
                throw new InvalidOperationException("Register windows before accessing or opening the UI.");
            if (prefab == null) throw new ArgumentNullException(nameof(prefab));
            if (scope != WindowScope.Local && scope != WindowScope.Project)
                throw new ArgumentOutOfRangeException(nameof(scope));
            foreach (var registration in _registrations)
                if (registration.Prefab.GetType() == prefab.GetType())
                    throw new InvalidOperationException($"Window {prefab.GetType().FullName} is already registered.");
            _registrations.Add(new WindowRegistration(prefab, scope,
                configure == null ? (Action<AWindowBase>)null : window => configure((TWindow)window)));
            return this;
        }

        public void Open<TWindow>() where TWindow : IWindow
        {
            if (_building) throw new InvalidOperationException("Cannot open a window while UI windows are being initialized.");
            EnsureBuilt();
            if (_localRegistry.Contains(typeof(TWindow))) _localWindows.OpenWindow<TWindow>();
            else { _projectRegistry.Get<TWindow>(); _projectWindows.OpenWindow<TWindow>(); }
        }

        public TWindow Get<TWindow>() where TWindow : IWindow
        {
            EnsureBuilt();
            return _localRegistry.Contains(typeof(TWindow))
                ? _localRegistry.Get<TWindow>() : _projectRegistry.Get<TWindow>();
        }

        private void EnsureBuilt()
        {
            ThrowIfDisposed();
            if (_built || _building) return;
            if (_registrations.Count == 0)
                throw new InvalidOperationException("Register at least one window before using the UI.");

            _building = true;
            var localActivator = new UiActivator(_localAdapter, _animationDefaults, _localWindows, _projectWindows);
            var projectActivator = new UiActivator(_projectAdapter, _animationDefaults, _localWindows, _projectWindows);
            var staging = new GameObject("KoboldUi window staging");
            staging.SetActive(false);
            try
            {
                foreach (var registration in _registrations)
                {
                    var project = registration.Scope == WindowScope.Project;
                    var window = UnityEngine.Object.Instantiate(registration.Prefab, staging.transform);
                    _instances.Add(window);
                    (project ? _projectRegistry : _localRegistry).Register(window);
                    window.Configure(project ? projectActivator : localActivator);
                }
                for (var i = 0; i < _instances.Count; i++)
                {
                    var activator = _registrations[i].Scope == WindowScope.Project ? projectActivator : localActivator;
                    activator.PrepareGameObject(_instances[i].gameObject);
                }
                for (var i = 0; i < _instances.Count; i++)
                    _registrations[i].Configure?.Invoke(_instances[i]);
                for (var i = 0; i < _instances.Count; i++)
                {
                    var canvas = _registrations[i].Scope == WindowScope.Project ? _projectCanvas : _localCanvas;
                    _instances[i].transform.SetParent(canvas.transform, false);
                    _instances[i].Initialize();
                }
                _built = true;
            }
            catch { Dispose(); throw; }
            finally
            {
                _building = false;
                if (Application.isPlaying) UnityEngine.Object.Destroy(staging);
                else UnityEngine.Object.DestroyImmediate(staging);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            (_localWindows as IDisposable)?.Dispose();
            (_projectWindows as IDisposable)?.Dispose();
            foreach (var window in _instances)
            {
                if (window == null) continue;
                if (Application.isPlaying) UnityEngine.Object.Destroy(window.gameObject);
                else UnityEngine.Object.DestroyImmediate(window.gameObject);
            }
            _instances.Clear();
            _registrations.Clear();
            _localRegistry.Clear();
            _projectRegistry.Clear();
            _built = false;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(UiBootstrap));
        }

        private sealed class WindowRegistration
        {
            public readonly AWindowBase Prefab;
            public readonly WindowScope Scope;
            public readonly Action<AWindowBase> Configure;

            public WindowRegistration(AWindowBase prefab, WindowScope scope, Action<AWindowBase> configure)
            {
                Prefab = prefab;
                Scope = scope;
                Configure = configure;
            }
        }
    }
}

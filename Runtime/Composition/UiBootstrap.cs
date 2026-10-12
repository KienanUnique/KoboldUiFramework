using System;
using System.Collections.Generic;
using KoboldUi.Element.Animations;
using KoboldUi.Services.WindowsService;
using KoboldUi.Services.WindowsService.Impl;
using KoboldUi.Windows;
using UnityEngine;

namespace KoboldUi.Composition
{
    public enum UiInitializationMode { Automatic, Manual }

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
        private readonly List<Action> _pendingActions = new List<Action>();
        private readonly UiInitializationMode _initializationMode;
        private readonly ILocalWindowsService _localWindows;
        private readonly IProjectWindowsService _projectWindows;
        private bool _built;
        private bool _building;
        private bool _disposed;

        public ILocalWindowsService LocalWindows
        {
            get { ThrowIfDisposed(); return _localWindows; }
        }

        public IProjectWindowsService ProjectWindows
        {
            get { ThrowIfDisposed(); return _projectWindows; }
        }

        public UiInitializationMode InitializationMode => _initializationMode;
        public UiAnimationDefaults AnimationDefaults => _animationDefaults;

        public UiBootstrap(Canvas localCanvas, Canvas projectCanvas,
            IUiCompositionAdapter adapter = null, UiAnimationDefaults animationDefaults = null,
            UiInitializationMode initializationMode = UiInitializationMode.Automatic)
            : this(localCanvas, projectCanvas, adapter, adapter, animationDefaults, initializationMode)
        {
        }

        public UiBootstrap(Canvas localCanvas, Canvas projectCanvas, IUiCompositionAdapter projectAdapter,
            IUiCompositionAdapter localAdapter, UiAnimationDefaults animationDefaults,
            UiInitializationMode initializationMode = UiInitializationMode.Automatic)
        {
            _localCanvas = localCanvas ?? throw new ArgumentNullException(nameof(localCanvas));
            _projectCanvas = projectCanvas ?? throw new ArgumentNullException(nameof(projectCanvas));
            _projectAdapter = projectAdapter;
            _localAdapter = localAdapter;
            _animationDefaults = animationDefaults;
            if (initializationMode != UiInitializationMode.Automatic &&
                initializationMode != UiInitializationMode.Manual)
                throw new ArgumentOutOfRangeException(nameof(initializationMode));
            _initializationMode = initializationMode;
            _localWindows = new LocalWindowsService(_localRegistry, EnsureReadyForServiceOperation, QueueAction);
            _projectWindows = new ProjectWindowsService(_projectRegistry, EnsureReadyForServiceOperation, QueueAction);
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

        public void Initialize()
        {
            ThrowIfDisposed();
            if (_built) return;
            if (_building)
                throw new InvalidOperationException("Cannot initialize UI windows while they are being initialized.");
            Build();
        }

        private void EnsureBuilt()
        {
            ThrowIfDisposed();
            if (_built || _building) return;
            if (_initializationMode == UiInitializationMode.Manual)
                throw new InvalidOperationException("Call Initialize before using windows in Manual mode.");
            Build();
        }

        private bool EnsureReadyForServiceOperation()
        {
            ThrowIfDisposed();
            if (_built) return true;
            if (_building) return false;
            EnsureBuilt();
            return true;
        }

        private void QueueAction(Action action)
        {
            _pendingActions.Add(action);
        }

        private void Build()
        {
            if (_registrations.Count == 0)
                throw new InvalidOperationException("Register at least one window before using the UI.");

            _building = true;
            var hasLocalWindows = false;
            var hasProjectWindows = false;
            foreach (var registration in _registrations)
            {
                if (registration.Scope == WindowScope.Local) hasLocalWindows = true;
                else hasProjectWindows = true;
            }
            var localWindows = hasLocalWindows ? _localWindows : null;
            var projectWindows = hasProjectWindows ? _projectWindows : null;
            var localActivator = new UiActivator(_localAdapter, _animationDefaults, localWindows, projectWindows);
            var projectActivator = new UiActivator(_projectAdapter, _animationDefaults, localWindows, projectWindows);
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
            try
            {
                foreach (var action in _pendingActions) action();
            }
            finally { _pendingActions.Clear(); }
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
            _pendingActions.Clear();
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

    /// <summary>Collects UI registrations and optional animation overrides before creating the bootstrap.</summary>
    public sealed class UiBootstrapBuilder
    {
        private readonly Canvas _localCanvas;
        private readonly Canvas _projectCanvas;
        private readonly WindowScope _defaultScope;
        private readonly List<Action<UiBootstrap>> _registrations = new List<Action<UiBootstrap>>();
        private readonly Dictionary<Type, IAnimationParameters> _parameters =
            new Dictionary<Type, IAnimationParameters>();
        private readonly Action<UiBootstrap> _onBuilt;
        private IUiCompositionAdapter _adapter;
        private UiAnimationDefaults _projectDefaults;
        private UiInitializationMode _initializationMode = UiInitializationMode.Automatic;
        private bool _built;

        public UiBootstrapBuilder(Canvas localCanvas, Canvas projectCanvas,
            WindowScope defaultScope = WindowScope.Local)
            : this(localCanvas, projectCanvas, defaultScope, null)
        {
        }

        public UiBootstrapBuilder(Canvas localCanvas, Canvas projectCanvas, WindowScope defaultScope,
            Action<UiBootstrap> onBuilt)
        {
            _localCanvas = localCanvas ?? throw new ArgumentNullException(nameof(localCanvas));
            _projectCanvas = projectCanvas ?? throw new ArgumentNullException(nameof(projectCanvas));
            if (defaultScope != WindowScope.Local && defaultScope != WindowScope.Project)
                throw new ArgumentOutOfRangeException(nameof(defaultScope));
            _defaultScope = defaultScope;
            _onBuilt = onBuilt;
        }

        public UiBootstrapBuilder RegisterWindow<TWindow>(TWindow prefab,
            WindowScope? scope = null, Action<TWindow> configure = null) where TWindow : AWindowBase
        {
            ThrowIfBuilt();
            if (prefab == null) throw new ArgumentNullException(nameof(prefab));
            _registrations.Add(ui => ui.RegisterWindow(prefab, scope ?? _defaultScope, configure));
            return this;
        }

        public UiBootstrapBuilder AddDefaultParameters<T>(T parameter)
            where T : class, IAnimationParameters
        {
            ThrowIfBuilt();
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            var parameterType = typeof(T);
            if (_parameters.ContainsKey(parameterType))
                throw new InvalidOperationException($"Defaults already contain {parameterType.FullName}.");
            _parameters.Add(parameterType, parameter);
            return this;
        }

        public UiBootstrapBuilder WithProjectDefaults(UiAnimationDefaults projectDefaults)
        {
            ThrowIfBuilt();
            _projectDefaults = projectDefaults ?? throw new ArgumentNullException(nameof(projectDefaults));
            return this;
        }

        public UiBootstrapBuilder WithAdapter(IUiCompositionAdapter adapter)
        {
            ThrowIfBuilt();
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            return this;
        }

        public UiBootstrapBuilder WithInitializationMode(UiInitializationMode mode)
        {
            ThrowIfBuilt();
            if (mode != UiInitializationMode.Automatic && mode != UiInitializationMode.Manual)
                throw new ArgumentOutOfRangeException(nameof(mode));
            _initializationMode = mode;
            return this;
        }

        public UiBootstrap Build()
        {
            ThrowIfBuilt();
            _built = true;
            UiAnimationDefaults defaults = null;
            if (_projectDefaults != null || _parameters.Count != 0)
            {
                defaults = new UiAnimationDefaults(_projectDefaults);
                foreach (var parameter in _parameters) defaults.Add(parameter.Key, parameter.Value);
            }
            var ui = new UiBootstrap(_localCanvas, _projectCanvas, _adapter, defaults, _initializationMode);
            try
            {
                foreach (var registration in _registrations) registration(ui);
                _onBuilt?.Invoke(ui);
                return ui;
            }
            catch
            {
                ui.Dispose();
                throw;
            }
        }

        private void ThrowIfBuilt()
        {
            if (_built) throw new InvalidOperationException("UI builder has already been built.");
        }
    }
}

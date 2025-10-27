# Kobold UI Framework – UI plugin documentation

## Window services
Two services expose the window stack: `ILocalWindowsService` and `IProjectWindowsService`. Both implement `IWindowsService` and share the same behaviour.

### Properties
- `IWindow CurrentWindow { get; }` – returns the window currently located on the top of the stack or `null` when the stack is empty.

### Methods
- `bool IsOpened<TWindow>()` – checks whether the top of the stack already contains the requested window instance.
- `void OpenWindow<TWindow>(Action onComplete = null)` – resolves the window from the Zenject container, skips the call when the window is already on top, pushes the window into the stack and starts its activation sequence. The optional callback is appended to the execution queue and runs after the window reaches the `Active` state.
- `void CloseWindow(Action onComplete = null, bool useBackLogicIgnorableChecks = true)` – closes the current window. The action is cancelled when the stack is empty, when another window becomes current before execution, or when the current window has `IsBackLogicIgnorable == true` and the flag `useBackLogicIgnorableChecks` is left enabled. When the close succeeds the previous window is restored to the `Active` state. The optional callback is queued after the closing sequence.
- `void CloseToWindow<TWindow>(Action onComplete = null, bool useBackLogicIgnorableChecks = true)` – closes windows one by one until the requested window becomes current. If the target window is missing from the stack the action finishes without changes. The optional callback is queued after the transition completes.

All service calls are enqueued in `TaskRunner`. Actions execute sequentially; waiting is not required on the caller side. Additional callbacks supplied through `onComplete` are also enqueued.

## Window stack behaviour
Windows are stored in `WindowsStackHolder`.
- Opening a non-popup window (`IsPopup == false`) moves the previous window to the `Closed` state. Opening a popup leaves the previous window in the `NonFocused` state.
- Closing a window always triggers `WindowsOrdersManager.UpdateWindowsLayers` so the `Transform` order matches the stack order. The previous window receives the `Active` state.
- Stack order is also used to call `IWindow.ApplyOrder`, which adjusts the sibling index.

## Window lifecycle
- Every window inherits from `AWindow`.
- `AWindow` requires a `CanvasGroup`. The component toggles `interactable` according to the target `EWindowState`.
- Dependencies are injected through `InstallBindings(DiContainer)` and `Initialize()` is triggered right after all controllers are registered.
- `AddController<TController, TView>(TView view)` instantiates a controller via Zenject, injects dependencies into the window, stores the controller and closes it instantly. Controllers receive state changes together with the window.
- Optional `AnimatedEmptyView` entries are also added during initialization.
- Behaviour flags:
  - `IsPopup` controls whether previous windows stay visible (`NonFocused`).
  - `IsBackLogicIgnorable` blocks `CloseWindow` and `CloseToWindow` when the corresponding methods run with `useBackLogicIgnorableChecks = true`.
- `WaitInitialization(IUiActionsPool)` returns a pooled action that waits until `Initialize()` finishes. `OpenWindowAction` calls it before applying the `Active` state.

## Controllers and views
- `AUiController<TView>` listens to window state changes. Depending on `EWindowState` it calls `View.Open`, `View.ReturnFocus`, `View.RemoveFocus` or `View.Close`. Hooks `OnOpen`, `OnClose` and `OnFocusRemove` allow custom logic.
- `AUiView` defines the base view contract. `AUiSimpleView` toggles the GameObject active flag. `AUiAnimatedView` delegates to the configured `AUiAnimationBase` instances for appearance and disappearance.
- All operations return `IUiAction`. Actions are pooled by `IUiActionsPool` and executed through the shared task runner.

## Collections
`AUiCollection<TView>` defines the base behaviour for repeated UI elements.
- `AUiListCollection<TView>` keeps instantiated views in a list. `Create()` spawns a new view via `IInstantiator`, assigns it under the configured container and calls `Appear()`. `Clear()`, `Remove()` and `RemoveAt()` destroy views.
- `AUiPooledCollection<TView>` reuses views. `Create()` returns an existing pooled view when possible; `ReturnToPool(view)` hides the view, executes `Disappear()` and stores it for reuse. `Clear()` returns every active view to the pool.
- Views for collections inherit from `AUiCollectionView` and must implement `Appear()` and `Disappear()`.

## Installers and binding
- `LocalWindowsServiceInstaller` and `ProjectWindowsServiceInstaller` bind the corresponding window services as Zenject singletons.
- `DefaultAnimationsInstaller` binds default animation parameter ScriptableObjects.
- Windows are bound through the extension `DiContainerExtensions.BindWindowFromPrefab(Canvas canvas, T windowPrefab)`. The method instantiates the prefab as a child of the provided canvas, calls `InstallBindings`, queues the window for injection and binds its interfaces and self as a single instance.

## Usage examples
```csharp
// Opening a window from a service
_localWindowsService.OpenWindow<SettingsWindow>();

// Forcing a back navigation even when the current window ignores back logic
_projectWindowsService.CloseWindow(useBackLogicIgnorableChecks: false);

// Returning to an already opened window
_localWindowsService.CloseToWindow<MainMenuWindow>();
```


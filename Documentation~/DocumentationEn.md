# Kobold UI Framework

## Window services
- The framework exposes `ILocalWindowsService` and `IProjectWindowsService`; both implement `IWindowsService` and share the same API.
- Bind `LocalWindowsServiceInstaller` in the scene context and `ProjectWindowsServiceInstaller` in the project context to register the concrete implementations.

### Service API
- `IWindow CurrentWindow { get; }` — returns the top window in the stack or `null` if no windows are opened.
- `bool IsOpened<TWindow>()` — returns `true` only when the current window is of the requested type.
- `void OpenWindow<TWindow>(Action onComplete = null)` — resolves the window from the Zenject container, waits for its `IInitializable.Initialize` call if needed, updates the previous window state, and activates the requested window. If the requested instance is already on the top of the stack, no action is queued.
- `void CloseWindow(Action onComplete = null, bool useBackLogicIgnorableChecks = true)` — closes the current window. When `useBackLogicIgnorableChecks` is `true`, the call is ignored if the window reports `IsBackLogicIgnorable`.
- `void CloseToWindow<TWindow>(Action onComplete = null, bool useBackLogicIgnorableChecks = true)` — pops windows until the requested instance becomes the current window. The operation stops immediately when `useBackLogicIgnorableChecks` is `true` and a popped window has `IsBackLogicIgnorable` enabled. The call is ignored if the target window is not in the stack.

### Execution flow
- Each service instance owns a `TaskRunner` that dequeues `IUiAction` instances sequentially; window requests never overlap.
- Completion callbacks are enqueued as separate actions and run strictly after all prior UI actions in the queue.

### Stack behaviour
- Opening a non-popup window closes the previous window before activating the new one. Opening a popup window sets the previous window to the `NonFocused` state instead of closing it.
- Closing a window calls `SetState(EWindowState.Closed)` on the removed instance and then reopens the new top window with `EWindowState.Active`.
- `WindowsOrdersManager` assigns sibling indices so that the newest window stays on top and reorders the remaining instances after removals.

## Window implementation
- Every UI window inherits from `AWindow` (which itself derives from `AWindowBase`, `MonoBehaviour`, and `IInitializable`).
- `AWindow` requires a `CanvasGroup` component; it toggles `interactable` according to the active state.
- Serialized fields `_isPopup` and `_isBackLogicIgnorable` define the values returned by `IsPopup` and `IsBackLogicIgnorable` and control stack behaviour.
- Override `AddControllers()` to register window controllers by calling `AddController<TController, TView>(viewInstance)`. The helper instantiates the controller through Zenject, injects the host game object, initializes both controller and view, and closes the controller immediately.
- `AnimatedEmptyView` entries listed in the `_animatedEmptyViews` collection are attached automatically during `Initialize()`.
- The base initialization order is: resolve controllers, add empty elements, mark the window as initialized, wait for optional injections (`QueueForInject`), and then allow service actions to continue once `WaitInitializationAction` completes.

### Window states
- `Active` — the window is at the top of the stack; controllers receive `OnOpen()` and `CanvasGroup.interactable` is set to `true`.
- `NonFocused` — the window stays in the stack while a popup is active; controllers receive `OnFocusRemove()` and the canvas becomes non-interactable.
- `Closed` — the window leaves the stack or gets replaced by a non-popup; controllers receive `OnClose()` and the canvas is non-interactable.

## Controllers
- Controllers inherit from `AUiController<TView>` and implement `IInitializable`.
- `SetState(EWindowState state, in IUiActionsPool pool)` switches the view by invoking `Open`, `ReturnFocus`, `RemoveFocus`, or `Close` and updates the controller flags `IsOpened` and `IsInFocus`.
- Override `Initialize`, `OnOpen`, `OnClose`, and `OnFocusRemove` to attach business logic. `CloseInstantly()` delegates to `IUiView.CloseInstantly()` and is invoked during window initialization.

## Views
- Views implement `IUiView`; the base class `AUiView` supplies empty implementations that request pooled `EmptyAction` instances.
- `AUiSimpleView` toggles the GameObject on `Open`/`Close` and is suited for static layouts.
- `AUiAnimatedView` works with `AUiAnimationBase` components. `_needWaitAnimation` defines whether service actions wait for the DOTween animation to finish. `_useDefaultParameters` allows selecting injected default parameters or serialized overrides.
- Any custom view must implement `CloseInstantly()`; the base implementations either disable the GameObject or stop the animation.

## Animations
- `AUiAnimationBase` exposes `Appear`, `Disappear`, `AnimateFocusReturn`, `AnimateFocusRemoved`, and `DisappearInstantly` for view state changes.
- `AUiAnimation<TParams>` handles DOTween sequencing, optional waiting, and parameter selection. Default parameter assets are bound through `DefaultAnimationsInstaller`, which injects the framework-provided `FadeAnimationParameters`, `ScaleAnimationParameters`, and `SlideAnimationParameters` scriptable objects.

## Window binding
- Use `DiContainerExtensions.BindWindowFromPrefab(Canvas canvas, T windowPrefab)` to instantiate a window, call its `InstallBindings`, queue it for injection, and bind it as a singleton. Only the window is bound; controllers and views receive dependencies through injection when the window is initialized.

## Collections
- `AUiCollection<TView>` and its derivatives help manage repeated UI elements. Each collection stores a prefab reference and a container transform, and uses `IInstantiator` to spawn entries.
- `AUiListCollection<TView>` keeps created views in a `List<TView>`, supports indexed access, removal, and enumeration, and destroys views when clearing.
- `IUiCollectionView` implementations provide `Appear`, `Disappear`, and `Destroy`. `AUiSimpleCollectionView` toggles the GameObject and can be reused for straightforward cases.

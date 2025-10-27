# Windows service

## Interface summary
- `IWindowsService.CurrentWindow` returns the window on top of the stack or `null` when the stack is empty.
- `IWindowsService.IsOpened<TWindow>()` checks whether the current window matches the requested type.

### OpenWindow<TWindow>(Action onComplete = null)
- Resolves the bound window instance and ignores the request when the target window is already current.
- When the next window is a popup the previous window is switched to `EWindowState.NonFocused`; otherwise it is closed.
- Waits until `AWindowBase.Initialize` finishes before the new window appears.
- Adds the window to the stack, updates sibling indices and triggers the `EWindowState.Active` transition.
- Enqueues the optional callback after the open action so it runs in order.

### CloseWindow(Action onComplete = null, bool useBackLogicIgnorableChecks = true)
- Removes the current window from the stack and sets its state to `EWindowState.Closed`.
- With `useBackLogicIgnorableChecks = true` the request is skipped when the current window allows ignoring back navigation.
- After closing, schedules reopening of the previous window if one exists.
- Enqueues the optional callback right after the close action.

### CloseToWindow<TWindow>(Action onComplete = null, bool useBackLogicIgnorableChecks = true)
- Closes windows until the requested window becomes current; nothing happens if the target is not in the stack.
- Aborts when `useBackLogicIgnorableChecks` is enabled and an intermediate window is marked as ignorable.
- Reopens the resulting top window once the stack is trimmed.

### Execution model
- Every UI operation is queued in an internal `TaskRunner`, guaranteeing ordered execution.
- `LocalWindowsService` and `ProjectWindowsService` differ only by the context installer that registers them.

# Windows
- Derive windows from `AWindow`; each prefab must contain a `CanvasGroup`.
- `_isPopup` controls whether the previous window only loses focus (`true`) or closes (`false`).
- `_isBackLogicIgnorable` allows the window to opt out of back navigation when `useBackLogicIgnorableChecks` is enabled.
- Override `AddControllers` and call `AddController<TController, TView>(viewInstance)` to register each controller/view pair.
- `AddController` instantiates the controller via Zenject, injects the window GameObject, initializes the view and the controller, and closes the controller instantly.
- Animated placeholder views listed in `_animatedEmptyViews` are registered automatically during initialization.
- `SetState` forwards the requested state to all controllers and returns a single parallel action that wraps their responses.
- `ApplyOrder` updates the transform sibling index; `WindowsOrdersManager` uses it to keep the visual stacking order.
- `InstallBindings` caches the container reference and is called from `DiContainerExtensions.BindWindowFromPrefab`.

`AWindowBase` also exposes:
- `IsPopup`, `IsBackLogicIgnorable`, `Name`, and `IsInitialized` properties.
- `WaitInitialization`, which returns an action that completes once `Initialize` has run.

# Controllers
- Controllers inherit from `AUiController<TView>` and receive the view through Zenject injection.
- `SetState` transitions between `Active`, `NonFocused`, and `Closed`, triggering `OnOpen`, `OnFocusRemove`, or `OnClose` respectively after queuing the view action.
- Override `Initialize`, `OnOpen`, `OnClose`, and `OnFocusRemove` to implement behaviour.
- Use `CloseInstantly` when the view must be hidden without animations.

# Views
- Views implement `IUiView` and must be `MonoBehaviour` components.
- `AUiView` provides empty virtual handlers for each state and yields pooled `EmptyAction` instances by default.
- `AUiSimpleView` toggles the GameObject’s active state on open/close while reusing the base focus logic.
- `AUiAnimatedView` plays optional appear/disappear animations and falls back to `AUiView` when an animation is missing.
- `AnimatedEmptyView` is a ready-to-use animated placeholder derived from `AUiAnimatedView`.

# Animations
- Custom animations should inherit from `AUiAnimation<TParams>` or `AUiAnimationBase`.
- `_needWaitAnimation` forces the caller to wait for the tween to finish; otherwise an empty action is returned immediately.
- Animations can use injected default parameter assets or per-instance overrides; disable `_useDefaultParameters` to rely on the serialized parameters.
- Built-in animations cover fade, scale, and slide scenarios, each backed by its own DOTween tween implementation.
- Default parameter assets (`FadeAnimationParameters`, `ScaleAnimationParameters`, `SlideAnimationParameters`) expose duration and easing settings and are created as `ScriptableObject` instances.

# UI actions and pooling
- `IUiActionsPool` creates and recycles action instances for tweens, callbacks, initialization waits, window service commands, and parallel wrappers.
- `UiActionsPool.Initialize` creates object pools for every supported action type.
- Every action inherits from `AUiAction`, which runs its logic in `Start` and returns itself to the pool afterwards.
- `TaskRunner` dequeues and executes the actions sequentially; pending actions are disposed when the runner is shut down.

# Collections
- `AUiListCollection<TView>` instantiates new view instances for each item and destroys them when removed.
- `AUiPooledCollection<TView>` reuses view instances, returning them to an internal pool through `ReturnToPool`.
- Collection items implement `IUiCollectionView`; `AUiCollectionView` supplies `SetParent` and `Destroy`, while `AUiSimpleCollectionView` simply toggles the active state in `Appear`/`Disappear`.

# Installation
- Bind default animation parameters through `DefaultAnimationsInstaller`.
- Register `LocalWindowsServiceInstaller` and `ProjectWindowsServiceInstaller` in the corresponding Zenject contexts.
- Instantiate and bind window prefabs via `DiContainer.BindWindowFromPrefab(canvas, prefab)`; the helper attaches the prefab under the provided canvas, queues injection, and binds the instance as a singleton.

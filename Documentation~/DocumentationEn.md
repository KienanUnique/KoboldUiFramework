# Kobold UI Plugin Documentation

## Window services

### Interfaces
- `IWindowsService.CurrentWindow` exposes the window at the top of the stack or `null` when the stack is empty.
- `IWindowsService.IsOpened<TWindow>()` returns `true` if the current window is of the requested type.
- `IWindowsService.OpenWindow<TWindow>(Action onComplete = null)` resolves the window from the `DiContainer`, pushes it to the stack and opens it. The previous window becomes `NonFocused` when the next window is a popup and `Closed` otherwise. The optional callback is executed after every queued UI action finishes.
- `IWindowsService.CloseWindow(Action onComplete = null, bool useBackLogicIgnorableChecks = true)` closes the current window and returns to the previous one when the stack contains more than one window. When `useBackLogicIgnorableChecks` is `true`, windows with `IsBackLogicIgnorable` enabled block the close request.
- `IWindowsService.CloseToWindow<TWindow>(Action onComplete = null, bool useBackLogicIgnorableChecks = true)` closes windows until the requested instance reaches the top of the stack. The call stops early if the target is not present in the stack or if a back-logic-ignorable window is encountered while the flag is enabled.

### Execution pipeline
- `AWindowsService` owns the `WindowsStackHolder`, the `UiActionsPool`, and the `TaskRunner` that sequentially executes queued `IUiAction` instances with UniTask.
- Every service call creates one or more pooled actions (open/close/animation callbacks). Actions return themselves to the pool after completion; disposing a service disposes every pending action.
- `OpenWindowAction` waits for window initialisation (`WaitInitializationAction`) before showing the window and sets the sibling order through `WindowsOrdersManager`.
- `CloseWindowAction` and `CloseToWindowAction` close windows, update stack order and optionally reopen the previous window through `OpenPreviousWindowAction`.

### Implementations
- `LocalWindowsService` and `ProjectWindowsService` extend `AWindowsService`. They differ only by the context in which they are installed.

#### Example
```csharp
public sealed class InventoryButton : MonoBehaviour
{
    [Inject] private IWindowsService _windowsService;

    // Triggered by the UI button to open the inventory popup
    public void OnClick()
    {
        _windowsService.OpenWindow<InventoryWindow>(() =>
        {
            // Executed after every queued action (including animations) finishes
            Debug.Log("Inventory window ready for interaction");
        });
    }

    // Closes the active window while ignoring back-logic guards
    public void ForceClose()
    {
        _windowsService.CloseWindow(useBackLogicIgnorableChecks: false);
    }

    // Returns to the first instance of the main menu if it exists in the stack
    public void BackToMainMenu()
    {
        _windowsService.CloseToWindow<MainMenuWindow>();
    }
}
```

## Windows

### Contract
- `IWindow` exposes the lifecycle surface: `IsInitialized`, `Name`, `IsPopup`, `IsBackLogicIgnorable`, `WaitInitialization`, `SetState`, and `ApplyOrder`.
- `EWindowState` defines three legal states: `Active`, `NonFocused`, and `Closed`.

### Base classes
- `AWindowBase` is the common base for all windows. It implements `IInitializable`, waits for initialisation via a pooled action, and exposes `InstallBindings(DiContainer)` so the installer can bind a prefab instance.
- `AWindow` derives from `AWindowBase`. It requires a `CanvasGroup`, disables interaction while hidden or unfocused, and coordinates child controllers.
  - Override `AddControllers()` and call `AddController<TController, TView>(viewInstance)` for each `View` on the prefab. The method creates the controller through the injected `DiContainer`, injects the GameObject, initialises both the view and the controller, and forces the controller to close immediately so the window starts in the `Closed` state.
  - The serialized list of `AnimatedEmptyView` objects is processed during initialisation to register empty child elements automatically.
  - `ApplyOrder` assigns the transform sibling index that is calculated by `WindowsOrdersManager`.

#### Example
```csharp
public sealed class InventoryWindow : AWindow
{
    [SerializeField] private InventoryView _inventoryView;
    [SerializeField] private CanvasGroup _canvasGroup;

    // Called during prefab binding to register the main controller
    protected override void AddControllers()
    {
        AddController<InventoryController, InventoryView>(_inventoryView);
    }

    // Custom sibling order can be applied when needed
    public override void ApplyOrder(int order)
    {
        base.ApplyOrder(order);
        // Keep the inventory window above HUD overlays
        transform.SetSiblingIndex(order + 1);
    }
}
```

## Controllers
- Controllers must inherit from `AUiController<TView>`. The controller tracks `IsOpened` and `IsInFocus`, exposes `SetState(EWindowState, IUiActionsPool)`, and provides the overridable hooks `OnOpen`, `OnClose`, and `OnFocusRemove`.
- `CloseInstantly()` is available for instant teardown during initialisation or prefab setup.

#### Example
```csharp
public sealed class InventoryController : AUiController<InventoryView>
{
    [Inject] private IPlayerInventory _inventory;

    // Populate the view whenever the window opens
    protected override UniTask OnOpen()
    {
        View.RenderItems(_inventory.Items); // Render current items
        return UniTask.CompletedTask;
    }

    // Save transient state before the window closes
    protected override UniTask OnClose()
    {
        _inventory.RememberSelection(View.SelectedItemId);
        return UniTask.CompletedTask;
    }

    // Dampen UI effects when focus is lost but window remains visible
    protected override UniTask OnFocusRemove()
    {
        View.ToggleDimOverlay(true);
        return UniTask.CompletedTask;
    }
}
```

## Views
- `IUiView` defines `Initialize`, `Open`, `ReturnFocus`, `RemoveFocus`, `Close`, and `CloseInstantly` that all return pooled actions.
- `AUiView` implements the interface with empty animations (`EmptyAction`). Override the protected `On*` methods to plug custom behaviour.
- `AUiSimpleView` toggles the `GameObject` on open/close and delegates animation to the base implementation.
- `AUiAnimatedView` works with `AUiAnimationBase` references for open/close transitions. When no animation is assigned, it falls back to the base behaviour. `CloseInstantly` either runs the animation’s instant branch or disables the object. Auto-fill helpers are compiled only when `KOBOLD_ALCHEMY_SUPPORT` is defined.
- `AnimatedEmptyView` is a concrete animated view that can be used for placeholder elements.

#### Example
```csharp
public sealed class InventoryView : AUiAnimatedView
{
    [SerializeField] private ItemSlotWidget _slotPrefab;
    [SerializeField] private Transform _itemsParent;

    // Prepare visual state before the appear animation runs
    protected override void OnBeforeAppear()
    {
        gameObject.SetActive(true); // Ensure the root is visible
    }

    // Configure focus behaviour for child widgets
    protected override void OnReturnFocus()
    {
        HighlightSelection();
    }

    public void RenderItems(IReadOnlyList<ItemData> items)
    {
        // Instantiate slots using a pooled collection (see collections section)
    }

    public void ToggleDimOverlay(bool enabled)
    {
        // Enable a dim overlay when the window loses focus
    }
}
```

## Animations
- `AUiAnimationBase` describes the five animation hooks: `Appear`, `AnimateFocusReturn`, `AnimateFocusRemoved`, `Disappear`, and `DisappearInstantly`.
- `AUiAnimation<TParams>` provides DOTween-based `Appear` and `Disappear` implementations, optional waiting (`_needWaitAnimation`), access to default parameters injected into the component, and abstract methods `PrepareToAppear`, `AnimateAppear`, and `AnimateDisappear`.
- `IUiAnimationParameters` marks parameter assets. `AUiAnimationParameters` is a `ScriptableObject` base for parameter storage. Built-in implementations live under `Element/Animations/Parameters/Impl`.
- Default parameters can be registered through `DefaultAnimationsInstaller`, which binds fade, scale, and slide parameter instances as singletons.

#### Example
```csharp
[CreateAssetMenu(menuName = AssetMenuPath.Animations + nameof(InventoryOpenAnimation))]
public sealed class InventoryOpenAnimation : AUiAnimation<InventoryAnimationParams>
{
    // Configure reusable tween parameters in the inspector
    protected override void PrepareToAppear(InventoryAnimationParams parameters)
    {
        transform.localScale = Vector3.zero; // Start collapsed
    }

    protected override UniTask AnimateAppear(InventoryAnimationParams parameters)
    {
        // Play DOTween scale tween and wait for completion
        return transform.DOScale(Vector3.one, parameters.Duration).ToUniTask();
    }

    protected override UniTask AnimateDisappear(InventoryAnimationParams parameters)
    {
        // Shrink out without waiting (needWaitAnimation controls awaiting behaviour)
        return transform.DOScale(Vector3.zero, parameters.Duration).ToUniTask();
    }
}

[Serializable]
public sealed class InventoryAnimationParams : AUiAnimationParameters
{
    public float Duration = 0.2f; // Default tween duration
}
```

## Collections
- `AUiCollection<TView>` manages instantiation through `IInstantiator`, stores a prefab and target container, and provides a protected `OnCreated` hook that sets the parent and plays the appear animation.
- `AUiListCollection<TView>` keeps a list of created views, exposes an indexer, and destroys items on removal.
- `AUiPooledCollection<TView>` keeps a pool of detached views. `ReturnToPool` hides and stores the view for later reuse. `Clear` returns every active view to the pool.
- `AUiCollectionView` defines `Appear`, `Disappear`, `SetParent`, and `Destroy`. `AUiSimpleCollectionView` only toggles the object on appear/disappear.
- Interfaces `IUiCollection<TView>`, `IUiListCollection<TView>`, `IUiPooledCollection<TView>`, and `IUiFactory<TView>` describe the available operations for iteration, pooling, and creation.

#### Example
```csharp
public sealed class InventorySlotsCollection : AUiPooledCollection<ItemSlotWidget>
{
    [Inject] private IPlayerInventory _inventory;

    // Called whenever a slot is created from the pool
    protected override void OnCreated(ItemSlotWidget view)
    {
        base.OnCreated(view);
        view.Initialize(OnSlotSelected); // Attach callbacks once
    }

    public void Render(IReadOnlyList<ItemData> items)
    {
        Clear(); // Return old slots to the pool

        for (var index = 0; index < items.Count; index++)
        {
            var slot = Get(); // Acquire a pooled widget
            slot.SetItem(items[index]);
            slot.SetParent(Target); // Attach to the configured container
        }
    }

    private void OnSlotSelected(ItemSlotWidget slot)
    {
        _inventory.Select(slot.ItemId); // Update gameplay state
    }
}
```

## Installers and binding
- Use `DiContainerExtensions.BindWindowFromPrefab(canvasInstance, windowPrefab)` to instantiate a window prefab under a `Canvas`, queue it for injection, and bind it as a singleton service.
- Project-level installers should bind shared resources (default animation parameters and `ProjectWindowsServiceInstaller`). Scene-level installers typically bind `LocalWindowsServiceInstaller` and local windows.
- `LocalWindowsServiceInstaller` and `ProjectWindowsServiceInstaller` bind their corresponding services as eager singletons.

#### Example
```csharp
public sealed class InventoryWindowsInstaller : MonoInstaller
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private InventoryWindow _inventoryWindowPrefab;

    public override void InstallBindings()
    {
        // Bind the project-level windows service
        Container.Install<ProjectWindowsServiceInstaller>();

        // Bind default animation parameters shared across windows
        Container.Install<DefaultAnimationsInstaller>();

        // Instantiate and bind the inventory window under the target canvas
        Container.BindWindowFromPrefab(_canvas, _inventoryWindowPrefab);
    }
}
```

## Utilities and helpers
- `WindowsOrdersManager` updates sibling indices when windows appear or disappear. Override `ApplyOrder` in custom windows if the default transform ordering is not sufficient.
- `IAutoFillable` is an optional marker compiled with `KOBOLD_ALCHEMY_SUPPORT` to expose editor-only `AutoFill` buttons.
- `AssetMenuPath` centralises `CreateAssetMenu` prefixes for installers and animation parameters.

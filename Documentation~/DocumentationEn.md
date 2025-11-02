# Kobold UI Plugin Documentation

## Overview
The Kobold UI plugin supplies a composable window system built on Zenject, UniRx, and DOTween. Each UI screen is represented by a window that coordinates controllers and views, while services manage the window stack and route lifecycle events. This document describes the runtime contracts that ship with the package and references the Simple Sample implementation to show the intended usage.

Window content is broken into three layers:

- **Services** orchestrate the stack of windows and expose APIs to open, close, or replace screens.
- **Windows** configure which controllers operate inside a canvas and how animations are driven.
- **Controllers and views** wire user interactions and render data while reacting to lifecycle callbacks.

The following sections document the responsibilities of each layer and point to the Simple Sample code that exercises the APIs.

## Window services
`IWindowsService` keeps the global state for every window. The plugin exposes two scoped implementations:

- `ILocalWindowsService` operates on scene-local canvases, typically for menus.
- `IProjectWindowsService` handles global overlays that persist across scenes.

Every request is queued to guarantee sequential execution. Opening or closing a window schedules a transition to run after the previous action completes. Services also surface observables that reflect whether the stack is empty and which window is currently focused.

In the Simple Sample, the `Bootstrap` service waits for scene loading and then opens the main menu once the project is ready:

```csharp
public class Bootstrap : IInitializable
{
    private readonly ILocalWindowsService _localWindowsService;
    private readonly IScenesService _scenesService; // ...

    public void Initialize()
    {
        _scenesService.IsLoadingCompleted
            .Subscribe(_ => OpenMainMenu()); // ...
    }

    private void OpenMainMenu()
    {
        _localWindowsService.OpenWindow<MainMenuWindow>(); // ...
    }
}
```

Controllers call the same service to close the active window or to navigate back to a specific one:

```csharp
public class SettingsChangeConfirmationController : AUiController<SettingsChangeConfirmationView>
{
    private readonly ILocalWindowsService _localWindowsService;
    // ...

    public override void Initialize()
    {
        View.yesButton
            .OnClickAsObservable()
            .Subscribe(_ => _localWindowsService.CloseToWindow<MainMenuWindow>()); // ...
        // ...
    }
}
```

## Window lifecycle
Every window implements `IWindow` and inherits from `AWindow`. The base class injects a `CanvasGroup`, tracks the internal state (`Active`, `NonFocused`, `Closed`), and manages transitions between them. `OnOpen`, `OnClose`, and `OnFocusRemoved` hooks are dispatched to controllers so they can start or stop animations, reset data, or unsubscribe from services when the window leaves the stack.

A window registers its controllers inside `AddControllers()` so the framework can instantiate them with their views:

```csharp
public class MainMenuWindow : AWindow
{
    [SerializeField] private MainMenuView mainMenuView;
    [SerializeField] private TitleView titleView;

    protected override void AddControllers()
    {
        AddController<MainMenuController, MainMenuView>(mainMenuView); // Buttons controller for scene navigation
        AddController<TitleController, TitleView>(titleView); // Animated title controller
    }
}
```

## Controllers
Controllers inherit from `AUiController<TView>`. They receive constructor-injected services, then override `Initialize()` to subscribe to UniRx observables exposed by the view. Controllers can also override lifecycle callbacks to reset state when a window reopens.

The menu controller shows how to forward button clicks to the window service:

```csharp
public class MainMenuController : AUiController<MainMenuView>
{
    private readonly ILocalWindowsService _localWindowsService;
    // ...

    public override void Initialize()
    {
        View.startButton
            .OnClickAsObservable()
            .Subscribe(_ => _localWindowsService.OpenWindow<LevelSelectorWindow>());
        // ... handle other buttons
    }
}
```

More advanced controllers react to service data and adjust their views accordingly:

```csharp
public class LoadingIndicatorController : AUiController<LoadingIndicatorView>
{
    private readonly IScenesService _levelsService; // ...

    public override void Initialize()
    {
        _levelsService.LoadingProgress
            .Subscribe(progress => View.loadingProgressText.text = Format(progress))
            .AddTo(View); // ...
    }

    private string Format(float progress) => $"{progress * 100f:0}%"; // ...
}
```

Stateful controllers can track user input and react when a window closes or when values change:

```csharp
public class SettingsController : AUiController<SettingsView>
{
    private readonly ReactiveProperty<bool> _wasSomethingChanged = new();
    // ... includes storage service and ILocalWindowsService

    public override void Initialize()
    {
        View.applyButton.OnClickAsObservable().Subscribe(_ => Apply()).AddTo(View);
        View.closeButton.OnClickAsObservable().Subscribe(_ => HandleClose()).AddTo(View);
        _wasSomethingChanged.Subscribe(OnSomethingChanged).AddTo(View); // ...
    }

    private void HandleClose()
    {
        if (_wasSomethingChanged.Value)
            _localWindowsService.OpenWindow<SettingsChangeConfirmationWindow>();
        else
            _localWindowsService.CloseWindow();
    }

    private void Apply() => _settingsStorageService.ApplyUnsavedSettings(); // ...
    // ...
}
```

## Views
Views inherit from `AUiView` variations that define how the GameObject reacts to lifecycle events. `AUiAnimatedView` keeps the hierarchy alive during animation playback, `AUiSimpleView` toggles the root GameObject, and collection views inherit from `AUiSimpleCollectionView` to participate in pooled lists.

A settings view exposes raw Unity UI components that controllers manipulate:

```csharp
public class SettingsView : AUiAnimatedView
{
    public Slider soundVolume; // ...
    public Button closeButton;
}
```

Collection item views hold references for layout toggles and selection feedback:

```csharp
public class LevelItemView : AUiSimpleCollectionView
{
    [SerializeField] private Button button; // ...
    public IObservable<Unit> OnClick => button.OnClickAsObservable();
    public LevelData Data { get; private set; }
    // ...
}
```

## Animations
UI animations extend `AUiAnimationBase` or are orchestrated directly inside controllers. The Simple Sample uses DOTween to animate the title whenever the main menu opens and cleans up when the window closes.

```csharp
public class TitleController : AUiController<TitleView>
{
    protected override void OnOpen()
    {
        View.container
            .DOPunchScale(View.scalePunch, View.duration)
            .SetLoops(-1)
            .SetLink(View.gameObject); // ...
    }
    // ...
}
```

## Collections
Collection controllers derive from `AUiCollection<TView>` implementations to spawn pooled items. They clear the collection before repopulating it and manage selection state manually.

```csharp
public class LevelSelectorController : AUiController<LevelSelectorView>
{
    private LevelItemView _selectedItem; // ...
    private readonly ILevelProgressionService _levelProgressionService; // ...

    public override void Initialize()
    {
        foreach (var levelData in _levelProgressionService.Progression)
        {
            var item = View.levelItemsCollection.Create();
            item.SetLevelData(levelData);
            item.OnClick.Subscribe(_ => OnItemClicked(item));
        }
    }

    private void OnItemClicked(LevelItemView item)
    {
        if (!item.Data.IsUnlocked)
            return;

        _selectedItem?.SetSelectionState(false); // ...
        item.SetSelectionState(true);
        _selectedItem = item;
    }
}
```

![UI collection example](Images/ui_collection.jpeg)

## Installers and binding
Installers wrap window prefabs and canvases so Zenject can create them at runtime. Use `DiContainerExtensions.BindWindowFromPrefab` to register each window with the correct canvas. Scene installers typically configure local UI, while project installers register global overlays.

```csharp
[CreateAssetMenu(...)]
public class MainMenuUiInstaller : ScriptableObjectInstaller
{
    [SerializeField] private Canvas canvas; // ...
    [SerializeField] private MainMenuWindow mainMenuWindow;

    public override void InstallBindings()
    {
        var canvasInstance = Instantiate(canvas); // ...
        Container.BindWindowFromPrefab(canvasInstance, mainMenuWindow);
        Container.BindInterfacesTo<Bootstrap>().AsSingle();
    }
}
```

Project-level installers keep shared canvases alive between scene loads:

```csharp
[CreateAssetMenu(...)]
public class ProjectUiInstaller : ScriptableObjectInstaller
{
    [SerializeField] private Canvas canvas; // ...
    [SerializeField] private LoadingWindow loadingWindow;

    public override void InstallBindings()
    {
        var canvasInstance = Instantiate(canvas); // ...
        DontDestroyOnLoad(canvasInstance);
        Container.BindWindowFromPrefab(canvasInstance, loadingWindow);
    }
}
```

These examples demonstrate how the Simple Sample composes the plugin building blocks. Combine the services, windows, controllers, and installers shown above to extend the UI stack with your own windows while keeping the lifecycle consistent.

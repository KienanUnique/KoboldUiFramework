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
public class Bootstrap : IInitializable, IDisposable
{
    private readonly CompositeDisposable _compositeDisposable = new();
    private readonly ILocalWindowsService _localWindowsService;
    private readonly IScenesService _scenesService;
    // ... constructor assigns the injected services

    public void Initialize()
    {
        if (_scenesService.IsLoadingCompleted.Value)
            OpenMainMenu();
        else
            _scenesService.IsLoadingCompleted
                .Subscribe(OnLoadingComplete)
                .AddTo(_compositeDisposable); // Watches the loading flag once
    }

    private void OpenMainMenu()
    {
        _localWindowsService.OpenWindow<MainMenuWindow>(); // Pushes the first window into the local stack
        _compositeDisposable.Dispose();
    }
    // ...
}
```

Controllers call the same service to close the active window or to navigate back to a specific one:

```csharp
public class SettingsChangeConfirmationController : AUiController<SettingsChangeConfirmationView>
{
    private readonly ILocalWindowsService _localWindowsService;
    private readonly ISettingsStorageService _settingsStorageService;
    // ... dependency assignment

    public override void Initialize()
    {
        View.yesButton
            .OnClickAsObservable()
            .Subscribe(_ => OnYesButtonClicked())
            .AddTo(View);
        // ... wire the remaining buttons
    }

    private void OnYesButtonClicked()
    {
        _settingsStorageService.ApplyUnsavedSettings();
        _localWindowsService.CloseToWindow<MainMenuWindow>(); // Pops every overlay until the main menu becomes active
    }
    // ...
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
    // ... constructor stores the dependency

    public override void Initialize()
    {
        View.startButton
            .OnClickAsObservable()
            .Subscribe(_ => OnStartButtonClick())
            .AddTo(View);
        View.settingsButton
            .OnClickAsObservable()
            .Subscribe(_ => OnSettingsButtonClick())
            .AddTo(View);
        View.exitButton
            .OnClickAsObservable()
            .Subscribe(_ => OnExitButtonClick())
            .AddTo(View);
    }

    private void OnStartButtonClick() => _localWindowsService.OpenWindow<LevelSelectorWindow>();
    private void OnSettingsButtonClick() => _localWindowsService.OpenWindow<SettingsWindow>();
    private void OnExitButtonClick() => Application.Quit();
}
```

More advanced controllers react to service data and adjust their views accordingly:

```csharp
public class LoadingIndicatorController : AUiController<LoadingIndicatorView>
{
    private readonly IScenesService _levelsService;
    // ...

    public override void Initialize()
    {
        _levelsService.LoadingProgress
            .Subscribe(OnLoadingProgress)
            .AddTo(View); // Updates the text every frame while the window is open
    }

    private void OnLoadingProgress(float progress)
    {
        var progressPercentage = (int)(progress * 100f);
        progressPercentage = Mathf.Clamp(progressPercentage, 0, 100);
        View.loadingProgressText.text = $"{progressPercentage}%";
    }
}
```

Stateful controllers can track user input and react when a window closes or when values change:

```csharp
public class SettingsController : AUiController<SettingsView>
{
    private readonly ISettingsStorageService _settingsStorageService;
    private readonly ILocalWindowsService _localWindowsService;
    private readonly ReactiveProperty<bool> _wasSomethingChanged = new();
    // ...

    public override void Initialize()
    {
        View.applyButton
            .OnClickAsObservable()
            .Subscribe(_ => OnApplyButtonClick())
            .AddTo(View);
        View.closeButton
            .OnClickAsObservable()
            .Subscribe(_ => OnCloseButtonClick())
            .AddTo(View);
        _wasSomethingChanged.Subscribe(OnSomethingChanged).AddTo(View);
    }

    private void OnCloseButtonClick()
    {
        if (_wasSomethingChanged.Value)
        {
            var currentSettings = CreateSettingsData();
            _settingsStorageService.RememberUnsavedSettings(currentSettings);
            _localWindowsService.OpenWindow<SettingsChangeConfirmationWindow>();
        }
        else
        {
            _localWindowsService.CloseWindow();
        }
    }
    // ...
}
```

## Views
Views inherit from `AUiView` variations that define how the GameObject reacts to lifecycle events. `AUiAnimatedView` keeps the hierarchy alive during animation playback, `AUiSimpleView` toggles the root GameObject, and collection views inherit from `AUiSimpleCollectionView` to participate in pooled lists.

A settings view exposes raw Unity UI components that controllers manipulate:

```csharp
public class SettingsView : AUiAnimatedView
{
    [Header("Sounds")]
    public Slider soundVolume;
    public Slider musicVolume;
    // ... additional controls

    [Header("Buttons")]
    public Button applyButton;
    public Button cancelButton;
    public Button closeButton;
}
```

Collection item views hold references for layout toggles and selection feedback:

```csharp
public class LevelItemView : AUiSimpleCollectionView
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject lockedContainer;
    [SerializeField] private GameObject unlockedContainer;
    // ... visuals omitted for brevity
    [SerializeField] private Button button;

    public IObservable<Unit> OnClick => button.OnClickAsObservable();
    public LevelData Data { get; private set; }

    public void SetLevelData(LevelData levelData)
    {
        Data = levelData;
        nameText.text = levelData.Name; // Stores metadata for later selection checks
        // ... updates lock state and star icons
    }
}
```

## Animations
UI animations extend `AUiAnimationBase` or are orchestrated directly inside controllers. The Simple Sample uses DOTween to animate the title whenever the main menu opens and cleans up when the window closes.

```csharp
public class TitleController : AUiController<TitleView>
{
    private Tween _animationTween;
    // ...

    protected override void OnOpen()
    {
        _animationTween?.Kill();
        _animationTween = View.container
            .DOPunchScale(View.scalePunch, View.duration, View.vibrato, View.elasticity)
            .SetEase(View.ease)
            .SetLoops(-1, LoopType.Restart)
            .SetLink(View.gameObject); // Ensures the tween stops with the view
    }

    protected override void OnClose()
    {
        _animationTween?.Kill();
    }
}
```

## Collections
Collection controllers derive from `AUiCollection<TView>` implementations to spawn pooled items. They clear the collection before repopulating it and manage selection state manually.

```csharp
public class LevelSelectorController : AUiController<LevelSelectorView>
{
    private LevelItemView _selectedItem;
    // ... injected services

    public override void Initialize()
    {
        var collection = View.levelItemsCollection;
        collection.Clear();

        foreach (var levelData in _levelProgressionService.Progression)
        {
            var item = collection.Create();
            item.SetLevelData(levelData);
            item.SetSelectionState(false);
            item.OnClick.Subscribe(_ => OnItemClicked(item)).AddTo(View);
        }
    }

    private void OnItemClicked(LevelItemView item)
    {
        if (!item.Data.IsUnlocked)
            return;

        _selectedItem?.SetSelectionState(false);
        item.SetSelectionState(true);
        _selectedItem = item;
        View.loadButton.interactable = true;
    }
    // ...
}
```

## Installers and binding
Installers wrap window prefabs and canvases so Zenject can create them at runtime. Use `DiContainerExtensions.BindWindowFromPrefab` to register each window with the correct canvas. Scene installers typically configure local UI, while project installers register global overlays.

```csharp
[CreateAssetMenu(fileName = nameof(MainMenuUiInstaller), menuName = "Simple Sample/" + nameof(MainMenuUiInstaller), order = 0)]
public class MainMenuUiInstaller : ScriptableObjectInstaller
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private MainMenuWindow mainMenuWindow;
    [SerializeField] private SettingsWindow settingsWindow;
    // ... other window references

    public override void InstallBindings()
    {
        var canvasInstance = Instantiate(canvas);
        Container.BindWindowFromPrefab(canvasInstance, mainMenuWindow);
        Container.BindWindowFromPrefab(canvasInstance, settingsWindow);
        // ... bind additional windows

        Container.BindInterfacesTo<Bootstrap>().AsSingle().NonLazy();
    }
}
```

Project-level installers keep shared canvases alive between scene loads:

```csharp
[CreateAssetMenu(fileName = nameof(ProjectUiInstaller), menuName = "Simple Sample/" + nameof(ProjectUiInstaller), order = 0)]
public class ProjectUiInstaller : ScriptableObjectInstaller
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private LoadingWindow loadingWindow;
    // ...

    public override void InstallBindings()
    {
        var canvasInstance = Instantiate(canvas);
        DontDestroyOnLoad(canvasInstance);
        Container.BindWindowFromPrefab(canvasInstance, loadingWindow);
    }
}
```

These examples demonstrate how the Simple Sample composes the plugin building blocks. Combine the services, windows, controllers, and installers shown above to extend the UI stack with your own windows while keeping the lifecycle consistent.

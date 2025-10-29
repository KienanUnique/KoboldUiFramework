# Kobold UI Plugin Documentation

## Window services

The plugin exposes window management through `IWindowsService`, `ILocalWindowsService`, and `IProjectWindowsService`. The service
keeps a stack of `IWindow` instances, runs queued UI actions sequentially, and transitions the window state between `Active`,
`NonFocused`, and `Closed`.

The Simple Sample demonstrates how services are resolved from Zenject and how each call schedules an action:

#### Example: bootstrapping the local window stack
```csharp
public class Bootstrap : IInitializable, IDisposable
{
    private readonly CompositeDisposable _compositeDisposable = new();
    private readonly ILocalWindowsService _localWindowsService;
    private readonly IScenesService _scenesService;

    public void Initialize()
    {
        if (_scenesService.IsLoadingCompleted.Value)
            OpenMainMenu();
        else
            _scenesService.IsLoadingCompleted.Subscribe(OnLoadingComplete).AddTo(_compositeDisposable);
    }

    private void OpenMainMenu()
    {
        _localWindowsService.OpenWindow<MainMenuWindow>(); // Pushes the main menu window to the local stack
        _compositeDisposable.Dispose(); // Releases subscriptions once the first window is shown
    }
}
```

#### Example: combining open, close, and close-to calls
```csharp
public class SettingsController : AUiController<SettingsView>
{
    private readonly ISettingsStorageService _settingsStorageService;
    private readonly ILocalWindowsService _localWindowsService;
    private readonly ReactiveProperty<bool> _wasSomethingChanged = new();

    private void OnCloseButtonClick()
    {
        if (_wasSomethingChanged.Value)
        {
            var currentSettings = CreateSettingsData();
            _settingsStorageService.RememberUnsavedSettings(currentSettings);

            _localWindowsService.OpenWindow<SettingsChangeConfirmationWindow>(); // Opens a confirmation popup
        }
        else
        {
            _localWindowsService.CloseWindow(); // Pops the current settings window
        }
    }

    private void OnCancelButtonClick()
    {
        _settingsStorageService.ForgetUnsavedSettings();
        ResetSettings();

        _localWindowsService.CloseWindow(); // Returns to the previous window without saving
    }
}

public class SettingsChangeConfirmationController : AUiController<SettingsChangeConfirmationView>
{
    private void OnYesButtonClicked()
    {
        _settingsStorageService.ApplyUnsavedSettings();
        _localWindowsService.CloseToWindow<MainMenuWindow>(); // Closes popups until the main menu is on top
    }
}
```

## Windows

Windows implement `IWindow` and derive from `AWindow`. The base class injects a `CanvasGroup`, coordinates child controllers, and
tracks the window lifecycle. Override `AddControllers()` to register controllers that control individual view fragments.

#### Example: window composed from multiple views
```csharp
public class MainMenuWindow : AWindow
{
    [SerializeField] private MainMenuView mainMenuView;
    [SerializeField] private TitleView titleView;

    protected override void AddControllers()
    {
        AddController<MainMenuController, MainMenuView>(mainMenuView); // Registers the main menu buttons controller
        AddController<TitleController, TitleView>(titleView); // Registers the animated title controller
    }
}
```

## Controllers

Controllers inherit from `AUiController<TView>` and receive their dependencies through the constructor. `Initialize()` is called
after the view is injected and lets controllers subscribe to UI events. Override `OnOpen`, `OnClose`, or `OnFocusRemove` to handle
state transitions.

#### Example: wiring buttons to window navigation
```csharp
public class MainMenuController : AUiController<MainMenuView>
{
    private readonly ILocalWindowsService _localWindowsService;

    public override void Initialize()
    {
        View.startButton.OnClickAsObservable().Subscribe(_ => OnStartButtonClick()).AddTo(View);
        View.settingsButton.OnClickAsObservable().Subscribe(_ => OnSettingsButtonClick()).AddTo(View);
        View.exitButton.OnClickAsObservable().Subscribe(_ => OnExitButtonClick()).AddTo(View);
    }

    private void OnStartButtonClick() => _localWindowsService.OpenWindow<LevelSelectorWindow>(); // Shows the level selector
    private void OnSettingsButtonClick() => _localWindowsService.OpenWindow<SettingsWindow>(); // Opens the settings window
    private void OnExitButtonClick() => Application.Quit(); // Leaves the application in builds
}
```

#### Example: reacting to service data
```csharp
public class LoadingIndicatorController : AUiController<LoadingIndicatorView>
{
    private readonly IScenesService _levelsService;

    public override void Initialize()
    {
        _levelsService.LoadingProgress.Subscribe(OnLoadingProgress).AddTo(View); // Updates progress each frame
    }

    private void OnLoadingProgress(float progress)
    {
        var progressPercentage = (int) (progress * 100f);
        progressPercentage = Mathf.Clamp(progressPercentage, 0, 100);

        View.loadingProgressText.text = $"{progressPercentage}%"; // Displays loading percentage
    }
}
```

## Views

Views derive from `AUiView` subclasses to control how the hierarchy reacts to lifecycle events. `AUiAnimatedView` keeps the GameObject
enabled while playing appear/disappear animations, `AUiSimpleView` simply toggles the hierarchy, and collection views inherit from
`AUiSimpleCollectionView`.

#### Example: animated view exposing UI bindings
```csharp
public class SettingsView : AUiAnimatedView
{
    [Header("Sounds")]
    public Slider soundVolume;
    public Slider musicVolume;

    [Header("Difficulty")]
    public Toggle easyModeToggle;

    [Header("Buttons")]
    public Button applyButton;
    public Button cancelButton;
    public Button closeButton;
}
```

#### Example: collection item view
```csharp
public class LevelItemView : AUiSimpleCollectionView
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject lockedContainer;
    [SerializeField] private GameObject unlockedContainer;
    [SerializeField] private GameObject selectedBackground;
    [SerializeField] private GameObject unselectedBackground;
    [SerializeField] private StarGroup[] stars;
    [SerializeField] private Button button;

    public IObservable<Unit> OnClick => button.OnClickAsObservable(); // Emits when the user selects the item
    public LevelData Data { get; private set; }

    public void SetLevelData(LevelData levelData)
    {
        Data = levelData;

        nameText.text = levelData.Name; // Updates the text label

        lockedContainer.SetActive(!levelData.IsUnlocked);
        unlockedContainer.SetActive(levelData.IsUnlocked);

        for (var i = 0; i < stars.Length; i++)
        {
            var isAchieved = i < levelData.StarsCount;
            stars[i].SetState(isAchieved); // Toggles star icons based on earned progress
        }
    }
}
```

## Animations

Animation components derive from `AUiAnimationBase` or `AUiAnimation<TParams>`. The Simple Sample uses DOTween to animate the title
window while it is open. Controllers are free to start tweens in `OnOpen` and stop them in `OnClose`.

#### Example: looped title animation
```csharp
public class TitleController : AUiController<TitleView>
{
    private Tween _animationTween;

    protected override void OnOpen()
    {
        _animationTween?.Kill();

        _animationTween = View.container.DOPunchScale(View.scalePunch, View.duration, View.vibrato, View.elasticity)
            .SetEase(View.ease)
            .SetLoops(-1, LoopType.Restart)
            .SetLink(View.gameObject); // Disposes the tween automatically with the view
    }

    protected override void OnClose()
    {
        _animationTween?.Kill(); // Stops the tween when the window closes
    }
}
```

## Collections

Collections derive from `AUiCollection<TView>` implementations. `AUiListCollection<TView>` stores created items and destroys them on
`Clear()`. Controllers iterate over data, call `Create()`, and configure each item before it becomes part of the hierarchy.

#### Example: populating a list of level buttons
```csharp
public class LevelSelectorController : AUiController<LevelSelectorView>
{
    private readonly ILevelProgressionService _levelProgressionService;
    private LevelItemView _selectedItem;

    public override void Initialize()
    {
        var collection = View.levelItemsCollection;
        collection.Clear(); // Removes any previously created items

        var progression = _levelProgressionService.Progression;
        foreach (var levelData in progression)
        {
            var item = collection.Create(); // Creates a new view instance
            item.SetLevelData(levelData); // Populates the view with level metadata
            item.SetSelectionState(false);
            item.OnClick.Subscribe(_ => OnItemClicked(item)).AddTo(View); // Subscribes to selection events
        }
    }

    private void OnItemClicked(LevelItemView item)
    {
        if (!item.Data.IsUnlocked)
            return; // Locked levels cannot be selected

        if(_selectedItem != null)
            _selectedItem.SetSelectionState(false);

        item.SetSelectionState(true);
        _selectedItem = item;

        View.loadButton.interactable = true; // Enables loading once a valid level is chosen
    }
}
```

## Installers and binding

Use `DiContainerExtensions.BindWindowFromPrefab` to instantiate and bind window prefabs to a canvas. The sample splits bindings into a
project-wide installer for persistent windows and a scene installer for local UI.

#### Example: scene-level bindings
```csharp
[CreateAssetMenu(fileName = nameof(MainMenuUiInstaller), menuName = "Simple Sample/" + nameof(MainMenuUiInstaller), order = 0)]
public class MainMenuUiInstaller : ScriptableObjectInstaller
{
    [SerializeField] private Canvas canvas;

    [Header("Windows")]
    [SerializeField] private MainMenuWindow mainMenuWindow;
    [SerializeField] private SettingsWindow settingsWindow;
    [SerializeField] private SettingsChangeConfirmationWindow settingsChangeConfirmationWindow;
    [SerializeField] private LevelSelectorWindow levelSelectorWindow;

    public override void InstallBindings()
    {
        var canvasInstance = Instantiate(canvas);

        Container.BindWindowFromPrefab(canvasInstance, mainMenuWindow); // Registers the main menu window
        Container.BindWindowFromPrefab(canvasInstance, settingsWindow); // Registers the settings window
        Container.BindWindowFromPrefab(canvasInstance, settingsChangeConfirmationWindow); // Registers the confirmation popup
        Container.BindWindowFromPrefab(canvasInstance, levelSelectorWindow); // Registers the level selector window

        Container.BindInterfacesTo<Bootstrap>().AsSingle().NonLazy(); // Starts the loading bootstrapper immediately
    }
}
```

#### Example: project-level bindings
```csharp
[CreateAssetMenu(fileName = nameof(ProjectUiInstaller), menuName = "Simple Sample/" + nameof(ProjectUiInstaller), order = 0)]
public class ProjectUiInstaller : ScriptableObjectInstaller
{
    [SerializeField] private Canvas canvas;

    [Header("Windows")]
    [SerializeField] private LoadingWindow loadingWindow;

    public override void InstallBindings()
    {
        var canvasInstance = Instantiate(canvas);
        DontDestroyOnLoad(canvasInstance); // Keeps the loading canvas alive between scenes

        Container.BindWindowFromPrefab(canvasInstance, loadingWindow); // Registers the global loading window
    }
}
```

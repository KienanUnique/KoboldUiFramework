# WARNING
This documentation version is outdated!

# Services

The project provides two services for working with windows: `ILocalWindowsService` and `IProjectWindowsService`.

### OpenWindow
```c#
OpenWindow<TWindow>(Action onComplete = default, EAnimationPolitic previousWindowPolitic = EAnimationPolitic.Wait)
```
- Opens a window of the specified type.

Example:
```c#
_localWindowsService.OpenWindow<SettingsChangeConfirmationWindow>();
```

### TryBackWindow
```c#
TryBackWindow(Action<bool> onComplete = default, EAnimationPolitic previousWindowPolitic = EAnimationPolitic.Wait)
```
- Closes the current window and opens the previous one.
- If the window implements the `IBackLogicIgnorable` interface, closing will not occur.
- The `onComplete` callback receives a result: `false` if it was not possible to close the current window, or `true` if it was successful.

Example:
```c#
_localWindowsService.TryBackWindow();
```

### TryBackToWindow
```c#
TryBackToWindow<TWindow>(Action<bool> onComplete = default, EAnimationPolitic previousWindowsPolitic = EAnimationPolitic.Wait) 
```
- Closes windows until the specified window is opened.
- If the specified window was never opened previously, an error will be thrown!

Example:
```c#
_localWindowsService.TryBackToWindow<MainMenuWindow>();
```

### CloseWindow
```c#
CloseWindow(Action onComplete = default, EAnimationPolitic previousWindowPolitic = EAnimationPolitic.Wait)
```
- Closes the current window and opens the previous one.
- Does not depend on inheritance from `IBackLogicIgnorable`.

Example:
```c#
_projectWindowsService.CloseWindow();
```

### CloseToWindow
```c#
CloseToWindow<TWindow>(Action onComplete = default, EAnimationPolitic previousWindowsPolitic = EAnimationPolitic.Wait)
```
- Closes windows until the specified one is opened (similar to `TryBackToWindow`).
- Does not depend on inheritance from `IBackLogicIgnorable`.


## Animation Politic
You can also pass the `EAnimationPolitic` parameter to the window service methods.  
This parameter determines how the animation of the previous window is handled and has only two values:

* `Wait` – the animation for opening the new window will start **only after** the animation for closing or switching focus from the previous window finishes.
* `DoNotWait` – the animation for opening the new window will start **in parallel** with the animation for closing or switching focus from the previous window.


# Window
* A window class should inherit from `AWindow`.
* A window is a `MonoBehaviour` attached to a prefab. To add a UI element consisting of a Controller and View, override the `AddControllers` method and call the following method:
```c# 
    AddController<TController, TView>(viewObject)
```
---
* Windows can implement the following flag interfaces to change their behavior:
  * `IPopUp`: for **popup** windows. When opened, the previous window remains visible and just loses focus.
  * `IBackLogicIgnorable`: for windows that should **ignore** the `BackWindow` method.

Examples of window implementations:
```c#
    public class GameplayWindow : AWindow
    {
        [SerializeField] private ScoreCounterView scoreCounterView;
        [SerializeField] private HealthView healthView;
        [SerializeField] private TimerView timerView;

        protected override void AddControllers()
        {
            AddController<ScoreCounterController, ScoreCounterView>(scoreCounterView);
            AddController<HealthController, HealthView>(healthView);
            AddController<TimerController, TimerView>(timerView);
        }
    }
```

```c#
    public class SettingsWindow : AWindow, IPopUp
    {
        [SerializeField] private SettingsView settingsView;

        protected override void AddControllers()
        {
            AddController<SettingsController, SettingsView>(settingsView);
        }
    }
```



## States

A window can be in one of the following three states:

### Active
- The state when a window is open.
- Only the currently open window has this state.
- The controller's `OnOpen` method is called.

### NonFocused
- The state when a window is sent to the background due to the opening of an `IPopup` window.
- The controller's `OnFocusRemove` method is called.

### Closed
- The initial state of the window or the state after opening another regular window (not an `IPopup`!).
- The controller's `OnClose` method is called.

# View

Each `View` class must be a `MonoBehaviour` attached to the corresponding prefab.

A `View` class can inherit from one of the following base classes:

### AUiView:
- The base class with empty methods for tracking state.
- Use this when custom `View` logic is needed.

### AUiSimpleView:
- Inherits from `AUiView`.
- Implements simple enabling/disabling of the object when opening/closing.
- Suitable for rapid prototyping.

### AUiAnimatedView:
- Also inherits from `AUiView`.
- Uses animations when changing states.
- Recommended for most cases.

---
Example:
```c#
    public class ScoreCounterView : AUiAnimatedView
    {
        public TMP_Text score;
    }
```



# Animations

Animations are used to visually display changes in the `View` state.

Here are the main points to consider:
- Animations are implemented as `MonoBehaviour`.
- They are used only in `View` classes inherited from `AUiAnimatedView`.
- Each animation type has its own **unique** settings.
- To create a custom animation type, inherit from `AUiAnimation<TParams>`, where `TParams` must implement the `IUiAnimationParameters` interface and be **serializable**.


## Animation Parameters
- Animations can use both custom and default parameters.
- An animation parameter class is a **serializable** class inherited from `AUiAnimationParameters`, which stores all the necessary settings. `AUiAnimationParameters` in turn inherits from `ScriptableObject`.
- Default parameters are parameters set in one of the **contexts**.
- Setting default parameters is mandatory only if they are to be used (i.e., when the `UseDefaultParameters` boolean is set to `true`).
- If each animation uses its own parameters, setting default parameters **is not required**. There will be no errors since default parameters are injected with the `InjectOptional` attribute.


## Built-in Animations
- The plugin already provides three built-in animations: **Fade**, **Scale**, and **Slide**.
- These animations have an **installer**. However, when adding a new animation type, it is recommended to create your own custom installer for standard animations, using the built-in installer code as a reference.


# Controller
- All controllers must inherit from `AUiController<TView>`, where `TView` is a view inherited from `IUIView`.
- The controller has virtual methods `OnOpen`, `OnClose`, `OnFocusRemove`, which can be overridden as needed.
- It is recommended to perform initial controller setup in the `Initialize` method override.

Example of a controller:
```c#
    public class ScoreCounterController : AUiController<ScoreCounterView>
    {
        private readonly IScoreService _scoreService;

        public ScoreCounterController(IScoreService scoreService)
        {
            _scoreService = scoreService;
        }

        public override void Initialize()
        {
            _scoreService.CurrentScore.Subscribe(ShowScore).AddTo(View);
            ShowScore(_scoreService.CurrentScore.Value);
        }

        private void ShowScore(int newScore)
        {
            View.score.text = $"{newScore} / {_scoreService.NeedScore}";
        }
    }
```



### Important! Only windows are bound!
### Controllers and Views are not bound, only dependencies are injected into them!


# Installers
The following contexts should have the corresponding dependencies installed:
- **Project context:**
  - Default animation settings. If there are no custom animations, use `DefaultAnimationsInstaller*`.
  - `ProjectWindowsServiceInstaller`.
  - Project layer windows.
- **Scene context:**
  - `LocalWindowsServiceInstaller`.
  - Local layer windows.

`DefaultAnimationsInstaller` is the installer for the built-in animations. If you have added your own animations, create your own installer and bind your animations there:
```c#
  Container.BindInstance(myAnimationParameters).AsSingle();
```


* To install a window, do the following:
```c#
  Container.BindWindowFromPrefab(canvasInstance, windowPrefab);
```
Where:
- `canvasInstance` is the canvas created in the scene, where the window should be placed.
- `windowPrefab` is the prefab of the window to be spawned and bound.
---

Example of a Local windows installer:

```c#
[CreateAssetMenu(menuName = "Installers/GameUiInstaller", fileName = "GameUiInstaller")]
public class GameUiInstaller : ScriptableObjectInstaller<GameUiInstaller>
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameplayWindow gameplayWindow;
    [SerializeField] private LoseWindow loseWindow;
    
    public override void InstallBindings()
    {
        var canvasInstance = Instantiate(canvas);
        Container.BindWindowFromPrefab(canvasInstance, gameplayWindow);
        Container.BindWindowFromPrefab(canvasInstance, loseWindow);
    }
}
```

## Collections

<img src="Images/ui_collection.jpeg"/>

To display multiple identical UI elements, you can use collections:

### Collection example:

```c#
public class LevelItemsCollection : AUiListCollection<LevelItemView>
{
}
```

### Collection item example:

```c#
public class LevelItemView : A

UiSimpleView
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject lockedContainer;
    [SerializeField] private Button button;

    public IObservable<Unit> OnClick => button.OnClickAsObservable();
    public LevelData Data { get; private set; }

    public void SetLevelData(LevelData levelData)
    {
        Data = levelData;
        nameText.text = levelData.Name;
        lockedContainer.SetActive(!levelData.IsUnlocked);
    }
}
```

### Filling the collection example:

```c#
var collection = View.levelItemsCollection;
collection.Clear();
var progression = _levelProgressionService.Progression;

foreach (var levelData in progression)
{
    var item = collection.Create();
    item.SetLevelData(levelData);
    item.OnClick.Subscribe(_ => OnItemClicked(item)).AddTo(View);
}
```

---
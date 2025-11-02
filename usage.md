# Usage

This short guide shows how the Simple Sample wires Kobold UI windows, controllers, views, and collections. Each snippet comes directly from the sample and focuses on the relevant calls.

## Project setup

```csharp
// Samples~/SimpleSample/Scripts/Installers/ProjectServicesInstaller.cs
public override void InstallBindings()
{
    Container.BindInterfacesTo<ScenesService>().AsSingle();
    Container.BindInterfacesTo<SettingsStorageService>().AsSingle();
    Container.BindInterfacesTo<LevelProgressionService>().AsSingle();
}
```

```csharp
// Samples~/SimpleSample/Scripts/Installers/ProjectUiInstaller.cs
public override void InstallBindings()
{
    var canvasInstance = Instantiate(canvas); // Create the shared UI canvas.
    DontDestroyOnLoad(canvasInstance);

    Container.BindWindowFromPrefab(canvasInstance, loadingWindow);
}
```

## Registering windows

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Installers/MainMenuUiInstaller.cs
public override void InstallBindings()
{
    var canvasInstance = Instantiate(canvas);

    Container.BindWindowFromPrefab(canvasInstance, mainMenuWindow);
    Container.BindWindowFromPrefab(canvasInstance, settingsWindow);
    Container.BindWindowFromPrefab(canvasInstance, settingsChangeConfirmationWindow);
    Container.BindWindowFromPrefab(canvasInstance, levelSelectorWindow);

    Container.BindInterfacesTo<Bootstrap>().AsSingle().NonLazy(); //...
}
```

## Opening windows from services

```csharp
// Samples~/SimpleSample/Scripts/Services/Bootstrap/Bootstrap.cs
private void OpenMainMenu()
{
    _localWindowsService.OpenWindow<MainMenuWindow>();
    _compositeDisposable.Dispose();
}
```

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Ui/MainMenu/Menu/MainMenuController.cs
private void OnStartButtonClick() => _localWindowsService.OpenWindow<LevelSelectorWindow>();
private void OnSettingsButtonClick() => _localWindowsService.OpenWindow<SettingsWindow>();
private void OnExitButtonClick() => Application.Quit();
```

## Defining windows and controllers

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Ui/MainMenu/MainMenuWindow.cs
protected override void AddControllers()
{
    AddController<MainMenuController, MainMenuView>(mainMenuView); // Menu buttons.
    AddController<TitleController, TitleView>(titleView);         // Animated title.
}
```

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Ui/LevelSelector/LevelSelectorWindow.cs
protected override void AddControllers()
{
    AddController<LevelSelectorController, LevelSelectorView>(levelSelectorView);
}
```

## Working with views

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Ui/MainMenu/Menu/MainMenuView.cs
public class MainMenuView : AUiAnimatedView
{
    public Button startButton;
    public Button settingsButton;
    public Button exitButton;
}
```

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Ui/Settings/Settings/SettingsView.cs
public class SettingsView : AUiAnimatedView
{
    public Slider soundVolume; //...
    public Slider musicVolume;
    public Toggle easyModeToggle;
    public Button applyButton;
    public Button cancelButton;
    public Button closeButton;
}
```

## Controller lifecycle usage

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Ui/Settings/Settings/SettingsController.cs
public override void Initialize()
{
    View.musicVolume.OnValueChangedAsObservable().Subscribe(_ => RememberThatSomethingChanged()).AddTo(View);
    View.soundVolume.OnValueChangedAsObservable().Subscribe(_ => RememberThatSomethingChanged()).AddTo(View);
    //...
    View.closeButton.OnClickAsObservable().Subscribe(_ => OnCloseButtonClick()).AddTo(View);

    _settingsStorageService.UnsavedSettingsForgotten.Subscribe(_ => ResetSettings()).AddTo(View);
    _wasSomethingChanged.Subscribe(OnSomethingChanged).AddTo(View);
}

protected override void OnOpen() => ResetSettings();
```

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Ui/LevelSelector/Selector/LevelSelectorController.cs
public override void Initialize()
{
    foreach (var cancelButton in View.cancelButtons)
        cancelButton.OnClickAsObservable().Subscribe(_ => OnCancelButtonPressed()).AddTo(View);

    View.loadButton.OnClickAsObservable().Subscribe(_ => OnLoadButtonPressed()).AddTo(View);

    var collection = View.levelItemsCollection;
    collection.Clear();
    //...
}
```

## Collections

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Ui/LevelSelector/Selector/LevelItemsCollection.cs
public class LevelItemsCollection : AUiListCollection<LevelItemView>
{
}
```

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Ui/LevelSelector/Selector/LevelSelectorController.cs
var collection = View.levelItemsCollection;
collection.Clear();

var progression = _levelProgressionService.Progression;
foreach (var levelData in progression)
{
    var item = collection.Create();
    item.SetLevelData(levelData);
    item.SetSelectionState(false);
    item.OnClick.Subscribe(_ => OnItemClicked(item)).AddTo(View);
}
```

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Ui/LevelSelector/Selector/LevelItemView.cs
public void SetSelectionState(bool isSelected)
{
    selectedBackground.SetActive(isSelected);
    unselectedBackground.SetActive(!isSelected);
}
```

## Animations

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Ui/MainMenu/Title/TitleController.cs
protected override void OnOpen()
{
    _animationTween?.Kill();

    _animationTween = View.container.DOPunchScale(View.scalePunch, View.duration, View.vibrato, View.elasticity)
        .SetEase(View.ease)
        .SetLoops(-1, LoopType.Restart)
        .SetLink(View.gameObject);
}
```

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Ui/MainMenu/Title/TitleView.cs
public class TitleView : AUiAnimatedView
{
    public Vector3 scalePunch;
    public float duration;
    public int vibrato;
    public float elasticity;
    public Ease ease;
    public RectTransform container;
}
```

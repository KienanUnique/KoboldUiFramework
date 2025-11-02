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
    var canvasInstance = Instantiate(canvas); // ...
    DontDestroyOnLoad(canvasInstance);
    Container.BindWindowFromPrefab(canvasInstance, loadingWindow);
}
```

## Registering windows

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Installers/MainMenuUiInstaller.cs
public override void InstallBindings()
{
    var canvasInstance = Instantiate(canvas); // ...
    Container.BindWindowFromPrefab(canvasInstance, mainMenuWindow);
    Container.BindWindowFromPrefab(canvasInstance, settingsWindow); // ...
    Container.BindInterfacesTo<Bootstrap>().AsSingle();
}
```

## Opening windows from services

```csharp
// Samples~/SimpleSample/Scripts/Services/Bootstrap/Bootstrap.cs
private void OpenMainMenu()
{
    _localWindowsService.OpenWindow<MainMenuWindow>(); // ...
}
```

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Ui/MainMenu/Menu/MainMenuController.cs
private void OnStartButtonClick() => _localWindowsService.OpenWindow<LevelSelectorWindow>();
// ... other buttons
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
    // ...
}
```

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Ui/Settings/Settings/SettingsView.cs
public class SettingsView : AUiAnimatedView
{
    public Slider soundVolume; // ...
    public Button closeButton;
}
```

## Controller lifecycle usage

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Ui/Settings/Settings/SettingsController.cs
public override void Initialize()
{
    View.closeButton
        .OnClickAsObservable()
        .Subscribe(_ => OnCloseButtonClick()); // ...
}

protected override void OnOpen() => ResetSettings(); // ...
```

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Ui/LevelSelector/Selector/LevelSelectorController.cs
public override void Initialize()
{
    foreach (var cancelButton in View.cancelButtons)
        cancelButton.OnClickAsObservable().Subscribe(_ => OnCancelButtonPressed()); // ...
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
foreach (var levelData in _levelProgressionService.Progression)
{
    var item = View.levelItemsCollection.Create();
    item.SetLevelData(levelData);
    item.OnClick.Subscribe(_ => OnItemClicked(item)); // ...
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

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Ui/MainMenu/Title/TitleController.cs
protected override void OnOpen()
{
    View.container
        .DOPunchScale(View.scalePunch, View.duration)
        .SetLoops(-1)
        .SetLink(View.gameObject); // ...
}
```

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Ui/MainMenu/Title/TitleView.cs
public class TitleView : AUiAnimatedView
{
    public RectTransform container; // ...
}
```

## Animations

Simple Sample views that require animations inherit from `AUiAnimatedView` and keep their animation components serialized on the same GameObject. Drop the `AUiAnimationBase` MonoBehaviour scripts into the view component fields to wire open and close animations.

```csharp
// Samples~/SimpleSample/Scripts/MainMenuScreen/Ui/Settings/Settings/SettingsView.cs
public class SettingsView : AUiAnimatedView
{
    public Slider soundVolume; // ...
    public Button closeButton;
}
```

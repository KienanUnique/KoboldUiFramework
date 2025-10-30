# Документация плагина Kobold UI

## Общее описание
Плагин Kobold UI предоставляет композиционную систему окон поверх Zenject, UniRx и DOTween. Каждый экран интерфейса представлен окном, которое управляет контроллерами и вью, а сервисы управляют стеком окон и транслируют события жизненного цикла. Этот документ описывает контракт рантайма, поставляемый с пакетом, и ссылается на реализацию Simple Sample, чтобы показать ожидаемый способ использования.

Структура интерфейса делится на три слоя:

- **Сервисы** управляют стеком окон и предоставляют API для открытия, закрытия и замены экранов.
- **Окна** определяют, какие контроллеры работают внутри Canvas и как запускаются анимации.
- **Контроллеры и вью** обрабатывают пользовательские действия и отображают данные, реагируя на обратные вызовы жизненного цикла.

В следующих разделах описаны обязанности каждого слоя и приведены выдержки из Simple Sample, демонстрирующие использование API.

## Сервисы окон
`IWindowsService` хранит глобальное состояние всех окон. Плагин предоставляет две специализированные реализации:

- `ILocalWindowsService` работает с Canvas текущей сцены, что подходит для меню.
- `IProjectWindowsService` управляет глобальными окнами, которые переживают смену сцен.

Каждый запрос помещается в очередь, чтобы действия выполнялись последовательно. Открытие или закрытие окна планирует переход, который выполнится после завершения предыдущего действия. Сервисы также предоставляют наблюдаемые значения, показывающие, пуст ли стек и какое окно находится в фокусе.

В Simple Sample сервис `Bootstrap` ожидает завершения загрузки сцены и открывает главное меню, когда проект готов:

```csharp
public class Bootstrap : IInitializable, IDisposable
{
    private readonly CompositeDisposable _compositeDisposable = new();
    private readonly ILocalWindowsService _localWindowsService;
    private readonly IScenesService _scenesService;
    // ... конструктор сохраняет внедрённые сервисы

    public void Initialize()
    {
        if (_scenesService.IsLoadingCompleted.Value)
            OpenMainMenu();
        else
            _scenesService.IsLoadingCompleted
                .Subscribe(OnLoadingComplete)
                .AddTo(_compositeDisposable); // Один раз отслеживаем флаг загрузки
    }

    private void OpenMainMenu()
    {
        _localWindowsService.OpenWindow<MainMenuWindow>(); // Добавляем главное меню в локальный стек
        _compositeDisposable.Dispose();
    }
    // ...
}
```

Контроллеры используют тот же сервис, чтобы закрывать активное окно или вернуться к конкретному окну:

```csharp
public class SettingsChangeConfirmationController : AUiController<SettingsChangeConfirmationView>
{
    private readonly ILocalWindowsService _localWindowsService;
    private readonly ISettingsStorageService _settingsStorageService;
    // ... сохраняем зависимости

    public override void Initialize()
    {
        View.yesButton
            .OnClickAsObservable()
            .Subscribe(_ => OnYesButtonClicked())
            .AddTo(View);
        // ... подписываем остальные кнопки
    }

    private void OnYesButtonClicked()
    {
        _settingsStorageService.ApplyUnsavedSettings();
        _localWindowsService.CloseToWindow<MainMenuWindow>(); // Удаляем всплывающие окна до возвращения к главному меню
    }
    // ...
}
```

## Жизненный цикл окна
Каждое окно реализует `IWindow` и наследуется от `AWindow`. Базовый класс получает `CanvasGroup`, отслеживает внутренние состояния (`Active`, `NonFocused`, `Closed`) и управляет переходами между ними. Колбэки `OnOpen`, `OnClose` и `OnFocusRemoved` передаются контроллерам, чтобы они могли запускать или останавливать анимации, сбрасывать данные или отписываться от сервисов, когда окно покидает стек.

Окно регистрирует свои контроллеры в `AddControllers()`, чтобы фреймворк мог создать их вместе с привязанными вью:

```csharp
public class MainMenuWindow : AWindow
{
    [SerializeField] private MainMenuView mainMenuView;
    [SerializeField] private TitleView titleView;

    protected override void AddControllers()
    {
        AddController<MainMenuController, MainMenuView>(mainMenuView); // Контроллер кнопок навигации
        AddController<TitleController, TitleView>(titleView); // Контроллер анимированного заголовка
    }
}
```

## Контроллеры
Контроллеры наследуются от `AUiController<TView>`. Они получают сервисы через конструктор и переопределяют `Initialize()`, чтобы подписаться на наблюдаемые значения, предоставляемые вью. При необходимости контроллеры переопределяют методы жизненного цикла и сбрасывают состояние, когда окно открывается повторно.

Контроллер главного меню показывает, как пробрасывать клики в сервис окон:

```csharp
public class MainMenuController : AUiController<MainMenuView>
{
    private readonly ILocalWindowsService _localWindowsService;
    // ... конструктор сохраняет зависимость

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

Другие контроллеры реагируют на данные сервисов и обновляют вью:

```csharp
public class LoadingIndicatorController : AUiController<LoadingIndicatorView>
{
    private readonly IScenesService _levelsService;
    // ...

    public override void Initialize()
    {
        _levelsService.LoadingProgress
            .Subscribe(OnLoadingProgress)
            .AddTo(View); // Обновляем текст каждый кадр, пока окно активно
    }

    private void OnLoadingProgress(float progress)
    {
        var progressPercentage = (int)(progress * 100f);
        progressPercentage = Mathf.Clamp(progressPercentage, 0, 100);
        View.loadingProgressText.text = $"{progressPercentage}%";
    }
}
```

Состояние контроллера настроек показывает работу с пользовательским вводом и подтверждением закрытия:

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

## Вью
Вью наследуются от вариаций `AUiView`, которые определяют реакцию GameObject на события жизненного цикла. `AUiAnimatedView` оставляет иерархию активной во время анимаций, `AUiSimpleView` включает и отключает корневой объект, а коллекционные вью наследуются от `AUiSimpleCollectionView`, чтобы работать в составе пулов.

Вью настроек содержит компоненты Unity UI, которыми управляет контроллер:

```csharp
public class SettingsView : AUiAnimatedView
{
    [Header("Sounds")]
    public Slider soundVolume;
    public Slider musicVolume;
    // ... остальные элементы управления

    [Header("Buttons")]
    public Button applyButton;
    public Button cancelButton;
    public Button closeButton;
}
```

Элемент коллекции хранит ссылки на элементы оформления и состояния выбора:

```csharp
public class LevelItemView : AUiSimpleCollectionView
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject lockedContainer;
    [SerializeField] private GameObject unlockedContainer;
    // ... дополнительные визуальные элементы
    [SerializeField] private Button button;

    public IObservable<Unit> OnClick => button.OnClickAsObservable();
    public LevelData Data { get; private set; }

    public void SetLevelData(LevelData levelData)
    {
        Data = levelData;
        nameText.text = levelData.Name; // Сохраняем метаданные для проверки выбора
        // ... обновляем состояние замка и звёзд
    }
}
```

## Анимации
Анимации интерфейса наследуются от `AUiAnimationBase` или управляются напрямую внутри контроллеров. В Simple Sample DOTween анимирует заголовок при открытии главного меню и останавливает анимацию при закрытии окна.

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
            .SetLink(View.gameObject); // Гарантируем остановку твина вместе с вью
    }

    protected override void OnClose()
    {
        _animationTween?.Kill();
    }
}
```

## Коллекции
Коллекционные контроллеры наследуются от реализаций `AUiCollection<TView>`, создают элементы из пула и управляют их жизненным циклом. Перед заполнением список очищается, а состояние выбора обновляется вручную.

```csharp
public class LevelSelectorController : AUiController<LevelSelectorView>
{
    private LevelItemView _selectedItem;
    // ... внедрённые сервисы

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

## Инсталлеры и биндинги
Инсталлеры оборачивают префабы окон и Canvas, чтобы Zenject мог создать их в рантайме. Используйте `DiContainerExtensions.BindWindowFromPrefab`, чтобы зарегистрировать окно на нужном Canvas. Сценовые инсталлеры обычно настраивают локальный интерфейс, а проектные — глобальные окна.

```csharp
[CreateAssetMenu(fileName = nameof(MainMenuUiInstaller), menuName = "Simple Sample/" + nameof(MainMenuUiInstaller), order = 0)]
public class MainMenuUiInstaller : ScriptableObjectInstaller
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private MainMenuWindow mainMenuWindow;
    [SerializeField] private SettingsWindow settingsWindow;
    // ... остальные окна

    public override void InstallBindings()
    {
        var canvasInstance = Instantiate(canvas);
        Container.BindWindowFromPrefab(canvasInstance, mainMenuWindow);
        Container.BindWindowFromPrefab(canvasInstance, settingsWindow);
        // ... связываем дополнительные окна

        Container.BindInterfacesTo<Bootstrap>().AsSingle().NonLazy();
    }
}
```

Проектный инсталлер сохраняет общие Canvas между сменами сцен:

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

Эти примеры показывают, как Simple Sample объединяет строительные блоки плагина. Комбинируйте сервисы, окна, контроллеры и инсталлеры, чтобы расширять стек интерфейса собственными окнами, сохраняя единый жизненный цикл.

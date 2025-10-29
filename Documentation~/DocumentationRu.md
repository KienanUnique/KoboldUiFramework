# Документация плагина Kobold UI

## Сервис окон

Плагин управляет окнами через `IWindowsService`, `ILocalWindowsService` и `IProjectWindowsService`. Сервис хранит стек реализаций
`IWindow`, последовательно выполняет поставленные UI-действия и переключает состояние окна между `Active`, `NonFocused` и `Closed`.

В Simple Sample показано, как сервисы разрешаются из Zenject и как каждое обращение ставит действие в очередь:

#### Пример: запуск локального стека окон
```csharp
public class Bootstrap : IInitializable, IDisposable
{
    private readonly CompositeDisposable _compositeDisposable = new();
    private readonly ILocalWindowsService _localWindowsService;
    private readonly IScenesService _scenesService;
    // ...

    public void Initialize()
    {
        if (_scenesService.IsLoadingCompleted.Value)
            OpenMainMenu();
        else
            _scenesService.IsLoadingCompleted
                .Subscribe(OnLoadingComplete)
                .AddTo(_compositeDisposable); // Подписываемся до завершения загрузки
    }

    private void OnLoadingComplete(bool isComplete)
    {
        if (!isComplete)
            return;

        OpenMainMenu();
    }

    private void OpenMainMenu()
    {
        _localWindowsService.OpenWindow<MainMenuWindow>(); // Открываем главное меню в локальном стеке окон
        _compositeDisposable.Dispose(); // Освобождаем подписки после появления первого окна
    }
    // ...
}
```

#### Пример: открытие, закрытие и возврат к окну
```csharp
public class SettingsController : AUiController<SettingsView>
{
    private readonly ISettingsStorageService _settingsStorageService;
    private readonly ILocalWindowsService _localWindowsService;
    private readonly ReactiveProperty<bool> _wasSomethingChanged = new();
    // ... остальная инициализация и вспомогательные методы

    private void OnCloseButtonClick()
    {
        if (_wasSomethingChanged.Value)
        {
            var currentSettings = CreateSettingsData();
            _settingsStorageService.RememberUnsavedSettings(currentSettings);

            _localWindowsService.OpenWindow<SettingsChangeConfirmationWindow>(); // Показываем окно подтверждения
        }
        else
        {
            _localWindowsService.CloseWindow(); // Закрываем настройки и возвращаем предыдущее окно
        }
    }

    private void OnCancelButtonClick()
    {
        _settingsStorageService.ForgetUnsavedSettings();
        ResetSettings();

        _localWindowsService.CloseWindow(); // Возвращаемся назад без сохранения
    }
}

public class SettingsChangeConfirmationController : AUiController<SettingsChangeConfirmationView>
{
    // Зависимости передаются через конструктор...

    private void OnYesButtonClicked()
    {
        _settingsStorageService.ApplyUnsavedSettings();
        _localWindowsService.CloseToWindow<MainMenuWindow>(); // Закрываем цепочку окон до главного меню
    }
}
```

## Окна

Окна реализуют `IWindow` и наследуются от `AWindow`. Базовый класс получает `CanvasGroup`, управляет дочерними контроллерами и
отслеживает жизненный цикл. Переопределите `AddControllers()`, чтобы зарегистрировать контроллеры для отдельных участков окна.

#### Пример: окно из нескольких представлений
```csharp
public class MainMenuWindow : AWindow
{
    [SerializeField] private MainMenuView mainMenuView;
    [SerializeField] private TitleView titleView;
    // ...

    protected override void AddControllers()
    {
        AddController<MainMenuController, MainMenuView>(mainMenuView); // Контроллер кнопок главного меню
        AddController<TitleController, TitleView>(titleView); // Контроллер анимированного заголовка
    }
}
```

## Контроллеры

Контроллеры наследуются от `AUiController<TView>` и получают зависимости через конструктор. Метод `Initialize()` вызывается после
инъекции представления и позволяет подписаться на события UI. Переопределяйте `OnOpen`, `OnClose` или `OnFocusRemove`, чтобы
реагировать на смену состояния.

#### Пример: навигация по кнопкам
```csharp
public class MainMenuController : AUiController<MainMenuView>
{
    private readonly ILocalWindowsService _localWindowsService;
    // ...

    public override void Initialize()
    {
        View.startButton.OnClickAsObservable().Subscribe(_ => OnStartButtonClick()).AddTo(View);
        View.settingsButton.OnClickAsObservable().Subscribe(_ => OnSettingsButtonClick()).AddTo(View);
        View.exitButton.OnClickAsObservable().Subscribe(_ => OnExitButtonClick()).AddTo(View);
    }

    private void OnStartButtonClick() => _localWindowsService.OpenWindow<LevelSelectorWindow>(); // Переход к выбору уровня
    private void OnSettingsButtonClick() => _localWindowsService.OpenWindow<SettingsWindow>(); // Открываем настройки
    private void OnExitButtonClick() => Application.Quit(); // Завершаем приложение в билде
}
```

#### Пример: реакция на данные сервиса
```csharp
public class LoadingIndicatorController : AUiController<LoadingIndicatorView>
{
    private readonly IScenesService _levelsService;
    // ...

    public override void Initialize()
    {
        _levelsService.LoadingProgress.Subscribe(OnLoadingProgress).AddTo(View); // Обновляем прогресс при каждом кадре
    }

    private void OnLoadingProgress(float progress)
    {
        var progressPercentage = (int) (progress * 100f);
        progressPercentage = Mathf.Clamp(progressPercentage, 0, 100);

        View.loadingProgressText.text = $"{progressPercentage}%"; // Отображаем процент загрузки
    }
}
```

## Представления

Представления наследуются от производных `AUiView`. `AUiAnimatedView` оставляет объект активным на время анимаций появления/скрытия,
`AUiSimpleView` просто включает или выключает GameObject, а элементы коллекций наследуются от `AUiSimpleCollectionView`.

#### Пример: анимируемое представление с ссылками на UI
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
    // ... параметры анимации задаются в инспекторе
}
```

#### Пример: элемент коллекции уровней
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
    // ... остальные члены (например, SetSelectionState)

    public IObservable<Unit> OnClick => button.OnClickAsObservable(); // Событие выбора элемента
    public LevelData Data { get; private set; }

    public void SetLevelData(LevelData levelData)
    {
        Data = levelData;

        nameText.text = levelData.Name; // Обновляем подпись

        lockedContainer.SetActive(!levelData.IsUnlocked);
        unlockedContainer.SetActive(levelData.IsUnlocked);

        for (var i = 0; i < stars.Length; i++)
        {
            var isAchieved = i < levelData.StarsCount;
            stars[i].SetState(isAchieved); // Включаем иконки звёзд в зависимости от прогресса
        }
    }
    // ... остальные методы (например, SetSelectionState)
}
```

## Анимации

Компоненты анимаций наследуются от `AUiAnimationBase` или `AUiAnimation<TParams>`. В Simple Sample заголовок главного меню анимируется
через DOTween: контроллер запускает tween в `OnOpen` и останавливает его в `OnClose`.

#### Пример: циклическая анимация заголовка
```csharp
public class TitleController : AUiController<TitleView>
{
    private Tween _animationTween;
    // ...

    protected override void OnOpen()
    {
        _animationTween?.Kill();

        _animationTween = View.container.DOPunchScale(View.scalePunch, View.duration, View.vibrato, View.elasticity)
            .SetEase(View.ease)
            .SetLoops(-1, LoopType.Restart)
            .SetLink(View.gameObject); // Привязываем tween к объекту, чтобы он удалился вместе с ним
    }

    protected override void OnClose()
    {
        _animationTween?.Kill(); // Останавливаем анимацию при закрытии окна
    }
}
```

## Коллекции

Коллекции наследуются от реализаций `AUiCollection<TView>`. `AUiListCollection<TView>` хранит созданные элементы и удаляет их при
вызове `Clear()`. Контроллеры проходят по данным, вызывают `Create()`, настраивают представление и подключают события.

#### Пример: заполнение списка уровней
```csharp
public class LevelSelectorController : AUiController<LevelSelectorView>
{
    private readonly ILevelProgressionService _levelProgressionService;
    private LevelItemView _selectedItem;
    // ...

    public override void Initialize()
    {
        var collection = View.levelItemsCollection;
        collection.Clear(); // Удаляем элементы, созданные ранее

        foreach (var levelData in _levelProgressionService.Progression)
        {
            var item = collection.Create(); // Создаём новое представление уровня
            item.SetLevelData(levelData); // Передаём описание уровня
            item.SetSelectionState(false);
            item.OnClick.Subscribe(_ => OnItemClicked(item)).AddTo(View); // Подписываемся на выбор уровня
        }
    }

    private void OnItemClicked(LevelItemView item)
    {
        if (!item.Data.IsUnlocked)
            return; // Заблокированные уровни нельзя выбрать

        _selectedItem?.SetSelectionState(false);

        item.SetSelectionState(true);
        _selectedItem = item;

        View.loadButton.interactable = true; // Включаем кнопку загрузки после выбора доступного уровня
    }
    // ...
}
```

## Инсталляторы и привязка

Метод `DiContainerExtensions.BindWindowFromPrefab` создаёт экземпляр префаба окна под указанным канвасом и привязывает его в контейнере.
В примере проект делится на установщик глобальных окон и сценовый установщик локального UI.

#### Пример: привязки на уровне сцены
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
    // ...

    public override void InstallBindings()
    {
        var canvasInstance = Instantiate(canvas);

        Container.BindWindowFromPrefab(canvasInstance, mainMenuWindow); // Регистрируем главное меню
        Container.BindWindowFromPrefab(canvasInstance, settingsWindow); // Регистрируем окно настроек
        Container.BindWindowFromPrefab(canvasInstance, settingsChangeConfirmationWindow); // Регистрируем окно подтверждения
        Container.BindWindowFromPrefab(canvasInstance, levelSelectorWindow); // Регистрируем окно выбора уровня
        // ... регистрируйте дополнительные окна по той же схеме

        Container.BindInterfacesTo<Bootstrap>().AsSingle().NonLazy(); // Немедленно запускаем загрузочный сервис
    }
}
```

#### Пример: глобальные привязки проекта
```csharp
[CreateAssetMenu(fileName = nameof(ProjectUiInstaller), menuName = "Simple Sample/" + nameof(ProjectUiInstaller), order = 0)]
public class ProjectUiInstaller : ScriptableObjectInstaller
{
    [SerializeField] private Canvas canvas;

    [Header("Windows")]
    [SerializeField] private LoadingWindow loadingWindow;
    // ...

    public override void InstallBindings()
    {
        var canvasInstance = Instantiate(canvas);
        DontDestroyOnLoad(canvasInstance); // Канвас не уничтожается между сценами

        Container.BindWindowFromPrefab(canvasInstance, loadingWindow); // Регистрируем глобальное окно загрузки
        // ... при необходимости регистрируйте дополнительные окна
    }
}
```

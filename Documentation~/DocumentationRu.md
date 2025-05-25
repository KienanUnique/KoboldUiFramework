# WARNING
This documentation version is outdated!

# Services

В проекте предусмотрены два сервиса для работы с окнами: `ILocalWindowsService` и `IProjectWindowsService`


### OpenWindow
```c#
OpenWindow<TWindow>(Action onComplete = default, EAnimationPolitic previousWindowPolitic = EAnimationPolitic.Wait)
```
- Открывает окно указанного типа.

Пример:
```c#
_localWindowsService.OpenWindow<SettingsChangeConfirmationWindow>();
```

### TryBackWindow  
```c#
TryBackWindow(Action<bool> onComplete = default, EAnimationPolitic previousWindowPolitic = EAnimationPolitic.Wait)
```
- Закрывает текущее окно и открывает предыдущее 
- Если окно реализует интерфейс `IBackLogicIgnorable`, закрытие не произойдёт. 
- В `onComplete` передается результат: `false`, если не получилось закрыть текущее окно, или `true`, если получилось.

Пример:
```c#
_localWindowsService.TryBackWindow();
```

### TryBackToWindow  
```c#
TryBackToWindow<TWindow>(Action<bool> onComplete = default, EAnimationPolitic previousWindowsPolitic = EAnimationPolitic.Wait) 
```
- Закрывает окна до тех пор, пока не будет открыто указанное окно. 
- Если заданное окно не было открыто ранее — выпадет ошибка!

Пример:
```c#
_localWindowsService.TryBackToWindow<MainMenuWindow>();
```

### CloseWindow
```c#
CloseWindow(Action onComplete = default, EAnimationPolitic previousWindowPolitic = EAnimationPolitic.Wait)
```
- Закрывает текущее окно и открывает предыдущее
- Не зависит от унаследования от `IBackLogicIgnorable`.

Пример:
```c#
_projectWindowsService.CloseWindow();
```

### CloseToWindow  
```c#
CloseToWindow<TWindow>(Action onComplete = default, EAnimationPolitic previousWindowsPolitic = EAnimationPolitic.Wait)
```
- Закрывает окна до указанного (по аналогии с `TryBackToWindow`)
- Не зависит от унаследования от `IBackLogicIgnorable`.


## Animation Politic
В методы сервиса окон также можно передать параметр `EAnimationPolitic`
Этот параметр определяет обработку анимации предыдущего окна и имеет только два значения:

* `Wait` - анимация открытия нового окна запустится **только после окончания** анимации закрытия/сменой фокуса прошлого окна
* `DoNotWait` - анимация открытия нового окна запустится **параллельно** с анимацией закрытия/сменой фокуса прошлого окна


# Window
* Класс окна должен наследоваться от AWindow. 
* Окно является `MonoBehaviour`, размещенным на префабе. Чтобы добавить UI элемент, состоящий из Controller и View, переопределите метод `AddControllers` и вызовите метод:
```c# 
    AddController<TController, TView>(viewObject)
```
---
* Окна могут наследоваться от следующих интерфейсов-флагов, меняющих поведение окна:
    * `IPopUp`: для **всплывающих** окон. При открытии прошлое окно остается видимым и просто теряет фокус
    * `IBackLogicIgnorable`: для окон, которые должны **игнорировать метод `BackWindow`**

Примеры реализации окон:
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

Окна могут находиться только в трех состояниях:
### Active
- Состояние, когда окно открывается
- Только текущее открытое окно имеет это состояние
- У контроллера вызывается метод `OnOpen`

### NonFocused
- Состояние, когда окно переходит в фоновый режим из-за открытия `IPopup` окна
- У контроллера вызывается метод `OnFocusRemove`

### Closed
- Изначальное состояние окна или же состояние,полученное после открытия следующего обычного окна (не `IPopup`!)
- У контроллера вызывается метод `OnClose`

# View

Каждый класс View должен быть `MonoBehaviour`, прикрепленным к соответствующему Prefab.

Класс View может наследоваться от следующих базовых классов:

### AUiView:
- Базовый класс с пустыми методами для отслеживания состояния.
- Используйте, когда требуется реализовать кастомную логику View.

### AUiSimpleView:
- Наследуется от AUiView.
- Реализует простое включение/выключение объекта при открытии/закрытии.
- Подходит для быстрого прототипирования.

### AUiAnimatedView:
- Также наследуется от AUiView.
- Использует анимации при изменении состояний.
- Рекомендуется использовать в большинстве случаев.

---
Пример:
```c#
    public class ScoreCounterView : AUiAnimatedView
    {
        public TMP_Text score;
    }
```



# Animations

Анимации в служат для визуального отображения изменений состояния View

Вот основные моменты, которые стоит учитывать:
- Анимации реализованы как `MonoBehaviour`.
- Используются только во View, унаследованных от `AUiAnimatedView`.
- Каждый тип анимации имеет свои **уникальные** настройки.
- Чтобы создать собственный тип анимации, необходимо наследоваться от `AUiAnimation<TParams>`, где `TParams` должен реализовывать интерфейс `IUiAnimationParameters` и быть **сериализуемым**.


## Параметры анимации
- Анимации могут использовать как кастомные, так и стандартные параметры
- Класс параметров анимации — это **сериализуемый** класс, унаследованный от `AUiAnimationParameters`, который хранит все необходимые настройки. `AUiAnimationParameters` в свою очередь наследуется от `ScriptableObject`
- Стандартные параметры — это параметры, установленные в одном из **контекстов**
- Установка стандартных параметров обязательна только если планируется их использование (т.е. при выставлении bool `UseDefaultParameters` в true)
- Если для каждой анимации используются свои параметры, установка стандартных параметров **не требуется**. Ошибок не возникнет, так как стандартные параметры находятся в поле атрибутом `[InjectOptional]`


## Встроенные анимации
- В плагине уже доступны три готовые анимации: **Fade**, **Scale** и **Slide**.
- Для этих анимаций предусмотрен **инсталлер**. Однако при добавлении нового типа анимации рекомендуется создать свой собственный SO инсталлер для стандартных анимаций, используя код встроенного инсталлера в качестве примера.


# Controller
- Все контроллеры должны наследоваться от `AUiController<TView>`, где `TView` — это представление, унаследованное от `IUIView`.
- В контроллере имеются виртуальные методы `OnOpen`, `OnClose`, `OnFocusRemove`, которые можно переопределить при необходимости.
- Начальную настройку контроллера рекомендуется производить в переопределении метода `Initialize`

Пример контроллера:
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



### Внимание! Биндятся только сами окна!
### Контроллеры и Вьюшки никак не биндятся, в них только внедряются зависимости!


# Installers
В следующих контекстах должны быть установлены соответствующие зависимости:
- **Project контекст:**
  - Настройки анимаций по умолчанию. Если нет кастомных анимаций, то это DefaultAnimationsInstaller*
  - ProjectWindowsServiceInstaller
  - Окна Project слоя
- **Scene контекст:**
  - LocalWindowsServiceInstaller
  - Окна для Local слоя

DefaultAnimationsInstaller — это инсталлер для встроенных в плагин анимаций. Если вы добавили свои анимации, создайте собственный инсталлер и забиндите в нем ваши анимации:
```c#
  Container.BindInstance(myAnimationParameters).AsSingle();
```


* Заинсталить окно можно следующим образом
```c#
  Container.BindWindowFromPrefab(canvasInstance, windowPrefab);
```
Где:
- canvasInstance - это созданный канвас на сцене, в который нужно поместить окно
- windowPrefab - это префаб окна, которое нужно заспавнить и забиндить
---

Пример инсталлера Local окон:

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

Для отображения множества однотипных элементов UI можно использовать коллекции:

### Пример коллекции:

```c#
public class LevelItemsCollection : AUiListCollection<LevelItemView>
{
}
```

### Пример элемента коллекции:

```c#
public class LevelItemView : AUiSimpleView
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

### Пример заполнения коллекции:

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
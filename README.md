# Services

В проекте предусмотрены два сервиса для работы с окнами: ILocalWindowsService и IProjectWindowsService. Оба сервиса предлагают следующие методы:

- **OpenWindow<TWindow>(EWindowLayer windowLayer = EWindowLayer.Local)**: Открывает указанное окно. Параметр windowLayer определяет слой окна, по умолчанию используется Local.
```c#
  _service.OpenWindow<MainMenuWindow>();
```

- **BackWindow**: Закрывает текущее окно. Если окно реализует интерфейс IBackLogicIgnorable, то закрытие не произойдет.
```c#
  _service.BackWindow();
```

- **ForceCloseWindow**: Принудительно закрывает текущее окно, независимо от его наследования. Используйте этот метод только в крайних случаях.
```c#
  _service.ForceCloseWindow();
```

# Window

- Класс окна должен наследоваться от AWindow. Окно является MonoBehaviour, размещенным на префабе. Чтобы добавить UI элемент, состоящий из Controller и View, переопределите метод AddControllers и вызовите метод:
```AddController<TController, TView>(viewObject)**.```

- Окна могут наследоваться от следующих интерфейсов-флагов, меняющих поведение окна:
  - **IPopUp**: для всплывающих окон. При открытии прошлое окно остается видимым и просто теряет фокус
  - **IBackLogicIgnorable**: для окон, которые должны игнорировать метод BackWindow. Советую использовать этот интерфейс на "конечные" окна, дальше которых пользователь не должен уйти. Например, на какой-нибудь MainMenuWindow


Пример реализации окна:

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
# View

Каждый класс View должен быть MonoBehaviour, прикрепленным к соответствующему Prefab. Класс View может наследоваться от следующих базовых классов:

- **AUiView**:
    - Базовый класс с пустыми методами для отслеживания состояния.
    - Используйте, когда требуется реализовать кастомную логику View.

- **AUiSimpleView**:
    - Наследуется от AUiView.
    - Реализует простое включение/выключение объекта при открытии/закрытии.
    - Подходит для быстрого прототипирования.

- **AUiAnimatedView**:
    - Также наследуется от AUiView.
    - Использует анимации при изменении состояний.
    - Рекомендуется использовать в большинстве случаев.

Пример View:
```c#
    public class ScoreCounterView : AUiAnimatedView
    {
        public TMP_Text score;
    }
```

# Animations

Анимации в служат для визуального отображения изменений состояния View. Вот основные моменты, которые стоит учитывать:

- Анимации реализованы как MonoBehaviour.
- Используются только во View, унаследованных от AUiAnimatedView.
- Каждый тип анимации имеет свои уникальные настройки.
- Чтобы создать собственный тип анимации, необходимо наследоваться от AUiAnimation< TParams>, где TParams должен реализовывать интерфейс IUiAnimationParameters и быть сериализуемым.

## Параметры анимации

- Анимации могут использовать как кастомные, так и стандартные параметры.
- Класс параметров анимации — это сериализуемый класс, унаследованный от AUiAnimationParameters, который хранит все необходимые настройки. AUiAnimationParameters в свою очередь наследуется от ScriptableObject.
- Стандартные параметры — это параметры, установленные в одном из контекстов.
- Установка стандартных параметров обязательна только если планируется их использование (т.е. при выставлении bool UseDefaultParameters в true).
- Если для каждой анимации используются свои параметры, установка стандартных параметров не требуется. Ошибок не возникнет, так как стандартные параметры инжектятся с атрибутом InjectOptional.

## Встроенные анимации

- В плагине уже доступны три готовые анимации: Fade, Scale и Slide.
- Для этих анимаций предусмотрен инсталлер. Однако при добавлении нового типа анимации рекомендуется создать свой собственный SO инсталлер для стандартных анимаций, используя код встроенного инсталлера в качестве основы.


## Внимание! Контроллеры и Вьюшки никак не биндятся! Их нельзя получить как зависимость!

# Controller

* Все контроллеры должны наследоваться от AUiController<TView>, где TView — это представление, унаследованное от IUIView.
* В контроллере имеются виртуальные методы **OnOpen, OnClose, OnFocusRemove**, которые можно переопределить при необходимости.

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

# Installers

* В следующих контекстах должны быть установлены соответствующие зависимости:
  * **Project контекст:**
    * Настройки анимаций по умолчанию. Если нет кастомных анимаций, то это DefaultAnimationsInstaller*
    * ProjectWindowsServiceInstaller
    * Окна Project слоя
  * **Scene контекст:**
    * LocalWindowsServiceInstaller
    * Окна для Local слоя
* DefaultAnimationsInstaller — это инсталлер для встроенных в плагина анимаций. Если вы добавили свои анимации, создайте собственный инсталлер и свяжите в нем ваши анимации:
```c#
  Container.BindInstance(myAnimationParameters).AsSingle();
 ```

Пример инсталлера Local окон:
```c#
    [CreateAssetMenu(menuName = MenuPathBase.Installers + nameof(GameUiInstaller), fileName = nameof(GameUiInstaller))]
    public class GameUiInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameplayWindow gameplayWindow;
        [SerializeField] private PauseWindow pauseWindow;
        [SerializeField] private LoseWindow loseWindow;
        [SerializeField] private WinWindow winWindow;

        public override void InstallBindings()
        {
            var canvasInstance = Instantiate(canvas);

            Container.BindWindowFromPrefab(canvasInstance, gameplayWindow);
            Container.BindWindowFromPrefab(canvasInstance, loseWindow);
            Container.BindWindowFromPrefab(canvasInstance, winWindow);
            Container.BindWindowFromPrefab(canvasInstance, pauseWindow);
        }
    }
```

# Other

* Доступна поддержка плагина Alchemy. Этот пакет является опциональным и предназначен для более удобной сериализации.
  * Ссылка на Alchemy: [Alchemy GitHub](https://github.com/AnnulusGames/Alchemy)
  * Установка:
    * Установите через git package: https://github.com/AnnulusGames/Alchemy.git?path=/Alchemy/Assets/Alchemy
    * Добавьте define в проектные настройки: KOBOLD_ALCHEMY_SUPPORT
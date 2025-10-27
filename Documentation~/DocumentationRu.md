# Документация плагина Kobold UI

## Службы окон

### Интерфейсы
- `IWindowsService.CurrentWindow` возвращает верхнее окно стека или `null`, если стек пуст.
- `IWindowsService.IsOpened<TWindow>()` возвращает `true`, если текущим окном является экземпляр указанного типа.
- `IWindowsService.OpenWindow<TWindow>(Action onComplete = null)` запрашивает окно из `DiContainer`, помещает его в стек и открывает. Предыдущее окно получает состояние `NonFocused`, если новое окно является попапом, и `Closed` в противном случае. Необязательный колбэк выполняется после завершения всех поставленных в очередь UI-действий.
- `IWindowsService.CloseWindow(Action onComplete = null, bool useBackLogicIgnorableChecks = true)` закрывает текущее окно и возвращает предыдущее, если стек содержит больше одного окна. При `useBackLogicIgnorableChecks = true` окна с включённым `IsBackLogicIgnorable` блокируют закрытие.
- `IWindowsService.CloseToWindow<TWindow>(Action onComplete = null, bool useBackLogicIgnorableChecks = true)` закрывает окна до тех пор, пока целевое не окажется на вершине стека. Выполнение прерывается, если окно отсутствует в стеке или встречено окно, игнорирующее логику возврата при включённой проверке.

### Конвейер выполнения
- `AWindowsService` владеет `WindowsStackHolder`, `UiActionsPool` и `TaskRunner`, который последовательно выполняет поставленные `IUiAction` через UniTask.
- Каждый вызов службы формирует один или несколько действий из пула (открытие/закрытие/колбэки анимаций). После завершения действия возвращаются в пул; освобождение службы завершает все ожидающие действия.
- `OpenWindowAction` ждёт инициализации окна (`WaitInitializationAction`) перед показом и задаёт порядок отображения через `WindowsOrdersManager`.
- `CloseWindowAction` и `CloseToWindowAction` закрывают окна, обновляют порядок в стеке и при необходимости повторно открывают предыдущее окно через `OpenPreviousWindowAction`.

### Реализации
- `LocalWindowsService` и `ProjectWindowsService` наследуют `AWindowsService` и отличаются только контекстом установки.

## Окна

### Контракт
- `IWindow` предоставляет свойства `IsInitialized`, `Name`, `IsPopup`, `IsBackLogicIgnorable`, методы `WaitInitialization`, `SetState` и `ApplyOrder`.
- `EWindowState` задаёт три допустимых состояния: `Active`, `NonFocused`, `Closed`.

### Базовые классы
- `AWindowBase` — общий базовый класс для окон. Реализует `IInitializable`, позволяет ждать инициализацию через пула действий и предоставляет `InstallBindings(DiContainer)` для привязки экземпляра префаба.
- `AWindow` наследует `AWindowBase`, требует наличия `CanvasGroup`, отключает взаимодействие в скрытом и нефокусном состоянии и управляет дочерними контроллерами.
  - Переопределите `AddControllers()` и вызовите `AddController<TController, TView>(viewInstance)` для каждого `View` на префабе. Метод создаёт контроллер через внедрённый `DiContainer`, выполняет инъекцию зависимостей, инициализирует представление и контроллер и сразу закрывает контроллер, чтобы окно стартовало в состоянии `Closed`.
  - Сериализованный список `AnimatedEmptyView` обрабатывается при инициализации и добавляет пустые элементы автоматически.
  - `ApplyOrder` назначает индекс порядка среди дочерних трансформов по расчётам `WindowsOrdersManager`.

## Контроллеры
- Контроллеры должны наследовать `AUiController<TView>`. Класс отслеживает `IsOpened` и `IsInFocus`, предоставляет `SetState(EWindowState, IUiActionsPool)` и виртуальные методы `OnOpen`, `OnClose`, `OnFocusRemove`.
- Метод `CloseInstantly()` доступен для мгновенного закрытия во время инициализации или подготовки префаба.

## Представления
- `IUiView` определяет `Initialize`, `Open`, `ReturnFocus`, `RemoveFocus`, `Close` и `CloseInstantly`, каждый метод возвращает действие из пула.
- `AUiView` реализует интерфейс с пустыми анимациями (`EmptyAction`). Переопределяйте защищённые методы `On*` для дополнительной логики.
- `AUiSimpleView` включает объект при открытии и выключает при закрытии, остальные действия делегирует базовому классу.
- `AUiAnimatedView` использует ссылки на `AUiAnimationBase` для анимации открытия и закрытия. При отсутствии анимации применяется поведение базового класса. `CloseInstantly` либо вызывает мгновенное исчезновение анимации, либо отключает объект. Автозаполнение доступно только при определённом `KOBOLD_ALCHEMY_SUPPORT`.
- `AnimatedEmptyView` — готовое анимированное представление-заглушка.

## Анимации
- `AUiAnimationBase` описывает пять точек расширения: `Appear`, `AnimateFocusReturn`, `AnimateFocusRemoved`, `Disappear`, `DisappearInstantly`.
- `AUiAnimation<TParams>` реализует `Appear` и `Disappear` на основе DOTween, поддерживает ожидание завершения (`_needWaitAnimation`), использует внедряемые параметры по умолчанию и требует реализации `PrepareToAppear`, `AnimateAppear`, `AnimateDisappear`.
- `IUiAnimationParameters` помечает классы параметров. `AUiAnimationParameters` — базовый `ScriptableObject` для хранения настроек. Готовые параметры находятся в `Element/Animations/Parameters/Impl`.
- Параметры по умолчанию регистрируются через `DefaultAnimationsInstaller`, который биндиет экземпляры fade, scale и slide как синглтоны.

## Коллекции
- `AUiCollection<TView>` отвечает за создание элементов через `IInstantiator`, хранит префаб и контейнер и предоставляет защищённый `OnCreated`, устанавливающий родителя и вызывающий `Appear`.
- `AUiListCollection<TView>` ведёт список созданных представлений, предоставляет индексатор и удаляет элементы с уничтожением объектов.
- `AUiPooledCollection<TView>` хранит пул отсоединённых представлений. `ReturnToPool` скрывает элемент и возвращает его в пул, `Clear` возвращает все активные элементы.
- `AUiCollectionView` определяет `Appear`, `Disappear`, `SetParent`, `Destroy`. `AUiSimpleCollectionView` только включает и выключает объект.
- Интерфейсы `IUiCollection<TView>`, `IUiListCollection<TView>`, `IUiPooledCollection<TView>` и `IUiFactory<TView>` описывают операции по перебору, пулу и созданию.

## Инсталлеры и привязка
- Используйте `DiContainerExtensions.BindWindowFromPrefab(canvasInstance, windowPrefab)` для создания окна под `Canvas`, постановки его в очередь на инъекцию и привязки как синглтона.
- В проектном контексте следует биндить общие ресурсы (параметры анимаций по умолчанию и `ProjectWindowsServiceInstaller`). В сценах обычно устанавливаются `LocalWindowsServiceInstaller` и локальные окна.
- `LocalWindowsServiceInstaller` и `ProjectWindowsServiceInstaller` регистрируют свои службы как несущие синглтоны.

## Утилиты и помощники
- `WindowsOrdersManager` обновляет порядок следования окон при появлении и закрытии. При необходимости переопределяйте `ApplyOrder` для собственной логики сортировки.
- `IAutoFillable` — необязательный маркер, доступный при `KOBOLD_ALCHEMY_SUPPORT`, добавляющий кнопки `AutoFill` в редакторе.
- `AssetMenuPath` содержит префиксы `CreateAssetMenu` для инсталлеров и параметров анимаций.

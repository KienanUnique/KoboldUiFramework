# Kobold UI Framework

## Сервисы окон
- Фреймворк предоставляет `ILocalWindowsService` и `IProjectWindowsService`; оба реализуют `IWindowsService` и имеют общий API.
- В сценовом контексте установите `LocalWindowsServiceInstaller`, в проектном контексте — `ProjectWindowsServiceInstaller`, чтобы зарегистрировать реализации.

### API сервиса
- `IWindow CurrentWindow { get; }` — текущий элемент стека или `null`, если окна закрыты.
- `bool IsOpened<TWindow>()` — возвращает `true`, только когда верх стека принадлежит указанному типу.
- `void OpenWindow<TWindow>(Action onComplete = null)` — получает окно из контейнера Zenject, при необходимости ожидает вызов `IInitializable.Initialize`, переводит предыдущее окно в нужное состояние и активирует новое. Если запрошенное окно уже находится на вершине стека, действие не ставится в очередь.
- `void CloseWindow(Action onComplete = null, bool useBackLogicIgnorableChecks = true)` — закрывает текущее окно. При `useBackLogicIgnorableChecks == true` вызов игнорируется, если окно возвращает `IsBackLogicIgnorable`.
- `void CloseToWindow<TWindow>(Action onComplete = null, bool useBackLogicIgnorableChecks = true)` — удаляет окна до тех пор, пока вершиной стека не станет нужный экземпляр. При `useBackLogicIgnorableChecks == true` операция останавливается сразу, как только удаляемое окно помечено `IsBackLogicIgnorable`. Если целевого окна нет в стеке, действие игнорируется.

### Выполнение
- Каждый сервис содержит собственный `TaskRunner`, который последовательно обрабатывает `IUiAction`; запросы на окна не перекрываются.
- Коллбэки `onComplete` ставятся в очередь отдельным действием и исполняются строго после всех предыдущих UI-действий.

### Поведение стека
- При открытии обычного окна предыдущее окно закрывается. При открытии попапа предыдущее окно переводится в состояние `NonFocused` и остается в стеке.
- При закрытии окна вызывается `SetState(EWindowState.Closed)` для удаляемого экземпляра, после чего новое верхнее окно активируется через `EWindowState.Active`.
- `WindowsOrdersManager` выстраивает `SiblingIndex`, чтобы новое окно оказывалось поверх остальных и чтобы порядок обновлялся после удаления.

## Реализация окна
- Любое окно наследуется от `AWindow` (он расширяет `AWindowBase`, `MonoBehaviour` и `IInitializable`).
- `AWindow` требует компонент `CanvasGroup` и переключает `interactable` в зависимости от состояния.
- Сериализованные поля `_isPopup` и `_isBackLogicIgnorable` задают значения `IsPopup` и `IsBackLogicIgnorable` и влияют на работу стека.
- Переопределите `AddControllers()`, чтобы регистрировать контроллеры через `AddController<TController, TView>(viewInstance)`. Хелпер создает контроллер через Zenject, выполняет инжект GameObject, инициализирует контроллер и вью, после чего сразу закрывает контроллер.
- Экземпляры `AnimatedEmptyView`, перечисленные в `_animatedEmptyViews`, подключаются автоматически внутри `Initialize()`.
- Базовая инициализация включает добавление контроллеров, добавление пустых элементов, установку флага `IsInitialized`, ожидание отложенных инжектов (`QueueForInject`) и продолжение работы сервисов после завершения `WaitInitializationAction`.

### Состояния окна
- `Active` — окно находится на вершине стека; вызывается `OnOpen()`, `CanvasGroup.interactable` переключается в `true`.
- `NonFocused` — окно остается в стеке, но уступает фокус попапу; вызывается `OnFocusRemove()`, канва становится неинтерактивной.
- `Closed` — окно удаляется из стека или вытесняется обычным окном; вызывается `OnClose()`, канва неинтерактивна.

## Контроллеры
- Контроллеры наследуются от `AUiController<TView>` и реализуют `IInitializable`.
- `SetState(EWindowState state, in IUiActionsPool pool)` переключает вью через `Open`, `ReturnFocus`, `RemoveFocus` или `Close` и обновляет флаги `IsOpened` и `IsInFocus`.
- Переопределяйте `Initialize`, `OnOpen`, `OnClose` и `OnFocusRemove`, чтобы подключить бизнес-логику. `CloseInstantly()` вызывает `IUiView.CloseInstantly()` и используется во время инициализации окна.

## Вью
- Любая вью реализует `IUiView`; базовый класс `AUiView` предоставляет пустые реализации и использует пул `EmptyAction`.
- `AUiSimpleView` включает/выключает объект в `Open`/`Close` и подходит для статических элементов.
- `AUiAnimatedView` работает с компонентами `AUiAnimationBase`. Флаг `_needWaitAnimation` определяет, нужно ли дожидаться завершения анимации DOTween, `_useDefaultParameters` переключает использование внедренных параметров по умолчанию или сериализованных настроек.
- Любая кастомная вью должна реализовать `CloseInstantly()`; базовые реализации либо отключают объект, либо останавливают анимацию.

## Анимации
- `AUiAnimationBase` описывает методы `Appear`, `Disappear`, `AnimateFocusReturn`, `AnimateFocusRemoved` и `DisappearInstantly` для смены состояний вью.
- `AUiAnimation<TParams>` управляет запуском DOTween, ожиданием и выбором параметров. Инсталлер `DefaultAnimationsInstaller` связывает стандартные `FadeAnimationParameters`, `ScaleAnimationParameters` и `SlideAnimationParameters` (ScriptableObject).

## Привязка окон
- Используйте `DiContainerExtensions.BindWindowFromPrefab(Canvas canvas, T windowPrefab)`, чтобы создать экземпляр окна, вызвать его `InstallBindings`, поставить в очередь на инжект и забиндить как синглтон. Биндингу подвергается только окно; контроллеры и вью получают зависимости во время инициализации окна.

## Коллекции
- `AUiCollection<TView>` и ее наследники управляют повторяющимися UI-элементами. Коллекция хранит префаб, контейнер и создает элементы через `IInstantiator`.
- `AUiListCollection<TView>` держит созданные вью в `List<TView>`, поддерживает индексированный доступ, удаление и перечисление, а при очистке уничтожает элементы.
- Реализации `IUiCollectionView` предоставляют `Appear`, `Disappear` и `Destroy`. `AUiSimpleCollectionView` переключает активность GameObject и подходит для простых случаев.

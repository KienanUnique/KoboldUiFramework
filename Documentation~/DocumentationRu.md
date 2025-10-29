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

#### Пример
```csharp
public sealed class InventoryButton : MonoBehaviour
{
    [Inject] private IWindowsService _windowsService;

    // Вызывается кнопкой интерфейса для открытия окна инвентаря
    public void OnClick()
    {
        _windowsService.OpenWindow<InventoryWindow>(() =>
        {
            // Выполняется после завершения очереди действий и анимаций
            Debug.Log("Окно инвентаря готово к работе");
        });
    }

    // Закрывает активное окно, игнорируя защиту обратной логики
    public void ForceClose()
    {
        _windowsService.CloseWindow(useBackLogicIgnorableChecks: false);
    }

    // Возвращает стек к первому найденному окну главного меню
    public void BackToMainMenu()
    {
        _windowsService.CloseToWindow<MainMenuWindow>();
    }
}
```

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

#### Пример
```csharp
public sealed class InventoryWindow : AWindow
{
    [SerializeField] private InventoryView _inventoryView;
    [SerializeField] private CanvasGroup _canvasGroup;

    // Регистрируем контроллер при привязке префаба
    protected override void AddControllers()
    {
        AddController<InventoryController, InventoryView>(_inventoryView);
    }

    // При необходимости корректируем порядок в иерархии
    public override void ApplyOrder(int order)
    {
        base.ApplyOrder(order);
        // Держим окно инвентаря выше HUD
        transform.SetSiblingIndex(order + 1);
    }
}
```

## Контроллеры
- Контроллеры должны наследовать `AUiController<TView>`. Класс отслеживает `IsOpened` и `IsInFocus`, предоставляет `SetState(EWindowState, IUiActionsPool)` и виртуальные методы `OnOpen`, `OnClose`, `OnFocusRemove`.
- Метод `CloseInstantly()` доступен для мгновенного закрытия во время инициализации или подготовки префаба.

#### Пример
```csharp
public sealed class InventoryController : AUiController<InventoryView>
{
    [Inject] private IPlayerInventory _inventory;

    // Обновляем данные представления при открытии окна
    protected override UniTask OnOpen()
    {
        View.RenderItems(_inventory.Items); // Отрисовываем текущие предметы
        return UniTask.CompletedTask;
    }

    // Сохраняем состояние перед закрытием окна
    protected override UniTask OnClose()
    {
        _inventory.RememberSelection(View.SelectedItemId);
        return UniTask.CompletedTask;
    }

    // При потере фокуса ослабляем визуальные эффекты, окно остаётся открытым
    protected override UniTask OnFocusRemove()
    {
        View.ToggleDimOverlay(true);
        return UniTask.CompletedTask;
    }
}
```

## Представления
- `IUiView` определяет `Initialize`, `Open`, `ReturnFocus`, `RemoveFocus`, `Close` и `CloseInstantly`, каждый метод возвращает действие из пула.
- `AUiView` реализует интерфейс с пустыми анимациями (`EmptyAction`). Переопределяйте защищённые методы `On*` для дополнительной логики.
- `AUiSimpleView` включает объект при открытии и выключает при закрытии, остальные действия делегирует базовому классу.
- `AUiAnimatedView` использует ссылки на `AUiAnimationBase` для анимации открытия и закрытия. При отсутствии анимации применяется поведение базового класса. `CloseInstantly` либо вызывает мгновенное исчезновение анимации, либо отключает объект. Автозаполнение доступно только при определённом `KOBOLD_ALCHEMY_SUPPORT`.
- `AnimatedEmptyView` — готовое анимированное представление-заглушка.

#### Пример
```csharp
public sealed class InventoryView : AUiAnimatedView
{
    [SerializeField] private ItemSlotWidget _slotPrefab;
    [SerializeField] private Transform _itemsParent;

    // Готовим визуальное состояние перед анимацией появления
    protected override void OnBeforeAppear()
    {
        gameObject.SetActive(true); // Включаем корневой объект
    }

    // Реагируем на возврат фокуса, чтобы подсветить выбор
    protected override void OnReturnFocus()
    {
        HighlightSelection();
    }

    public void RenderItems(IReadOnlyList<ItemData> items)
    {
        // Создаём слоты через пул коллекции (см. раздел про коллекции)
    }

    public void ToggleDimOverlay(bool enabled)
    {
        // Включаем затемнение, когда окно теряет фокус
    }
}
```

## Анимации
- `AUiAnimationBase` описывает пять точек расширения: `Appear`, `AnimateFocusReturn`, `AnimateFocusRemoved`, `Disappear`, `DisappearInstantly`.
- `AUiAnimation<TParams>` реализует `Appear` и `Disappear` на основе DOTween, поддерживает ожидание завершения (`_needWaitAnimation`), использует внедряемые параметры по умолчанию и требует реализации `PrepareToAppear`, `AnimateAppear`, `AnimateDisappear`.
- `IUiAnimationParameters` помечает классы параметров. `AUiAnimationParameters` — базовый `ScriptableObject` для хранения настроек. Готовые параметры находятся в `Element/Animations/Parameters/Impl`.
- Параметры по умолчанию регистрируются через `DefaultAnimationsInstaller`, который биндиет экземпляры fade, scale и slide как синглтоны.

#### Пример
```csharp
[CreateAssetMenu(menuName = AssetMenuPath.Animations + nameof(InventoryOpenAnimation))]
public sealed class InventoryOpenAnimation : AUiAnimation<InventoryAnimationParams>
{
    // Настраиваем стартовое состояние перед запуском анимации
    protected override void PrepareToAppear(InventoryAnimationParams parameters)
    {
        transform.localScale = Vector3.zero; // Начинаем со свернутого состояния
    }

    protected override UniTask AnimateAppear(InventoryAnimationParams parameters)
    {
        // Запускаем DOTween и ждём завершения
        return transform.DOScale(Vector3.one, parameters.Duration).ToUniTask();
    }

    protected override UniTask AnimateDisappear(InventoryAnimationParams parameters)
    {
        // Сжимаем окно, ожидание контролируется полем needWaitAnimation
        return transform.DOScale(Vector3.zero, parameters.Duration).ToUniTask();
    }
}

[Serializable]
public sealed class InventoryAnimationParams : AUiAnimationParameters
{
    public float Duration = 0.2f; // Значение по умолчанию для твина
}
```

## Коллекции
- `AUiCollection<TView>` отвечает за создание элементов через `IInstantiator`, хранит префаб и контейнер и предоставляет защищённый `OnCreated`, устанавливающий родителя и вызывающий `Appear`.
- `AUiListCollection<TView>` ведёт список созданных представлений, предоставляет индексатор и удаляет элементы с уничтожением объектов.
- `AUiPooledCollection<TView>` хранит пул отсоединённых представлений. `ReturnToPool` скрывает элемент и возвращает его в пул, `Clear` возвращает все активные элементы.
- `AUiCollectionView` определяет `Appear`, `Disappear`, `SetParent`, `Destroy`. `AUiSimpleCollectionView` только включает и выключает объект.
- Интерфейсы `IUiCollection<TView>`, `IUiListCollection<TView>`, `IUiPooledCollection<TView>` и `IUiFactory<TView>` описывают операции по перебору, пулу и созданию.

#### Пример
```csharp
public sealed class InventorySlotsCollection : AUiPooledCollection<ItemSlotWidget>
{
    [Inject] private IPlayerInventory _inventory;

    // Вызывается при создании нового элемента из пула
    protected override void OnCreated(ItemSlotWidget view)
    {
        base.OnCreated(view);
        view.Initialize(OnSlotSelected); // Подписываем обработчик один раз
    }

    public void Render(IReadOnlyList<ItemData> items)
    {
        Clear(); // Возвращаем предыдущие элементы в пул

        for (var index = 0; index < items.Count; index++)
        {
            var slot = Get(); // Берём виджет из пула
            slot.SetItem(items[index]);
            slot.SetParent(Target); // Закрепляем под настроенным контейнером
        }
    }

    private void OnSlotSelected(ItemSlotWidget slot)
    {
        _inventory.Select(slot.ItemId); // Обновляем игровое состояние
    }
}
```

## Инсталлеры и привязка
- Используйте `DiContainerExtensions.BindWindowFromPrefab(canvasInstance, windowPrefab)` для создания окна под `Canvas`, постановки его в очередь на инъекцию и привязки как синглтона.
- В проектном контексте следует биндить общие ресурсы (параметры анимаций по умолчанию и `ProjectWindowsServiceInstaller`). В сценах обычно устанавливаются `LocalWindowsServiceInstaller` и локальные окна.
- `LocalWindowsServiceInstaller` и `ProjectWindowsServiceInstaller` регистрируют свои службы как несущие синглтоны.

#### Пример
```csharp
public sealed class InventoryWindowsInstaller : MonoInstaller
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private InventoryWindow _inventoryWindowPrefab;

    public override void InstallBindings()
    {
        // Подключаем сервис окон уровня проекта
        Container.Install<ProjectWindowsServiceInstaller>();

        // Регистрируем параметры анимаций по умолчанию
        Container.Install<DefaultAnimationsInstaller>();

        // Создаём и биндим окно инвентаря под указанный Canvas
        Container.BindWindowFromPrefab(_canvas, _inventoryWindowPrefab);
    }
}
```

## Утилиты и помощники
- `WindowsOrdersManager` обновляет порядок следования окон при появлении и закрытии. При необходимости переопределяйте `ApplyOrder` для собственной логики сортировки.
- `IAutoFillable` — необязательный маркер, доступный при `KOBOLD_ALCHEMY_SUPPORT`, добавляющий кнопки `AutoFill` в редакторе.
- `AssetMenuPath` содержит префиксы `CreateAssetMenu` для инсталлеров и параметров анимаций.

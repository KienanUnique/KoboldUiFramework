# Kobold UI Framework – документация UI-плагина

## Сервисы окон
Со стеком окон работают два сервиса: `ILocalWindowsService` и `IProjectWindowsService`. Оба реализуют `IWindowsService` и имеют одинаковое поведение.

### Свойства
- `IWindow CurrentWindow { get; }` – текущее окно на вершине стека или `null`, если стек пуст.

### Методы
- `bool IsOpened<TWindow>()` – проверяет, находится ли указанное окно на вершине стека.
- `void OpenWindow<TWindow>(Action onComplete = null)` – получает окно из контейнера Zenject, не выполняет действие, если окно уже находится наверху, помещает окно в стек и запускает последовательность активации. Необязательный колбэк добавляется в очередь и вызывается после перевода окна в состояние `Active`.
- `void CloseWindow(Action onComplete = null, bool useBackLogicIgnorableChecks = true)` – закрывает текущее окно. Действие отменяется, если стек пуст, если текущее окно успело поменяться, либо если у текущего окна `IsBackLogicIgnorable == true` и флаг `useBackLogicIgnorableChecks` оставлен включённым. При успешном закрытии предыдущее окно переводится в состояние `Active`. Колбэк ставится в очередь после завершения закрытия.
- `void CloseToWindow<TWindow>(Action onComplete = null, bool useBackLogicIgnorableChecks = true)` – по одному закрывает окна, пока указанное окно не станет текущим. Если нужного окна нет в стеке, действие завершается без изменений. Колбэк выполняется после завершения перехода.

Все вызовы сервиса попадают в очередь `TaskRunner`. Действия выполняются последовательно, ожидание на стороне вызывающего кода не требуется. Колбэки `onComplete` также ставятся в очередь.

## Поведение стека окон
Окна хранятся в `WindowsStackHolder`.
- При открытии окна с `IsPopup == false` предыдущее окно переводится в состояние `Closed`. При открытии окна-попапа предыдущее окно получает состояние `NonFocused`.
- Закрытие окна вызывает `WindowsOrdersManager.UpdateWindowsLayers`, чтобы порядок `Transform` совпадал с порядком стека. Предыдущее окно получает состояние `Active`.
- Порядок в стеке используется при вызове `IWindow.ApplyOrder`, который задаёт индекс потомка.

## Жизненный цикл окна
- Каждое окно наследуется от `AWindow`.
- `AWindow` требует наличие `CanvasGroup` и переключает свойство `interactable` в зависимости от целевого `EWindowState`.
- Зависимости внедряются через `InstallBindings(DiContainer)`, а `Initialize()` выполняется сразу после регистрации всех контроллеров.
- `AddController<TController, TView>(TView view)` создаёт контроллер через Zenject, проводит инъекции в окно, сохраняет контроллер и мгновенно закрывает его. Контроллеры получают изменения состояний вместе с окном.
- Дополнительные элементы `AnimatedEmptyView` добавляются в процессе инициализации.
- Флаги поведения:
  - `IsPopup` управляет тем, остаётся ли предыдущее окно видимым (`NonFocused`).
  - `IsBackLogicIgnorable` блокирует `CloseWindow` и `CloseToWindow`, если методы вызываются с `useBackLogicIgnorableChecks = true`.
- `WaitInitialization(IUiActionsPool)` возвращает действие ожидания завершения `Initialize()`. Его вызывает `OpenWindowAction` перед установкой состояния `Active`.

## Контроллеры и представления
- `AUiController<TView>` реагирует на смену состояний окна. В зависимости от `EWindowState` он вызывает у представления методы `Open`, `ReturnFocus`, `RemoveFocus` или `Close`. Переопределяемые методы `OnOpen`, `OnClose` и `OnFocusRemove` позволяют добавить логику.
- `AUiView` описывает базовый контракт представления. `AUiSimpleView` управляет активностью объекта. `AUiAnimatedView` использует настроенные экземпляры `AUiAnimationBase` для появления и исчезновения.
- Все операции возвращают `IUiAction`. Действия кэшируются в `IUiActionsPool` и выполняются общим планировщиком задач.

## Коллекции
`AUiCollection<TView>` задаёт базовое поведение повторяющихся UI-элементов.
- `AUiListCollection<TView>` хранит созданные представления в списке. `Create()` создаёт новый элемент через `IInstantiator`, назначает его контейнеру и вызывает `Appear()`. `Clear()`, `Remove()` и `RemoveAt()` уничтожают элементы.
- `AUiPooledCollection<TView>` переиспользует представления. `Create()` возвращает элемент из пула, если он есть; `ReturnToPool(view)` скрывает элемент, вызывает `Disappear()` и добавляет его обратно в пул. `Clear()` возвращает в пул все активные элементы.
- Представления коллекций наследуются от `AUiCollectionView` и реализуют методы `Appear()` и `Disappear()`.

## Инсталлеры и привязка
- `LocalWindowsServiceInstaller` и `ProjectWindowsServiceInstaller` привязывают соответствующие сервисы окон как одиночки Zenject.
- `DefaultAnimationsInstaller` регистрирует стандартные ScriptableObject-параметры анимаций.
- Окна привязываются через расширение `DiContainerExtensions.BindWindowFromPrefab(Canvas canvas, T windowPrefab)`. Метод создаёт экземпляр префаба дочерним объектом указанного канваса, вызывает `InstallBindings`, ставит окно в очередь на инъекцию и биндингом `BindInterfacesAndSelfTo<T>().FromInstance(window).AsSingle()` регистрирует окно и его интерфейсы.

## Примеры
```csharp
// Открыть окно через сервис
_localWindowsService.OpenWindow<SettingsWindow>();

// Принудительно закрыть окно, игнорируя защиту от back
_projectWindowsService.CloseWindow(useBackLogicIgnorableChecks: false);

// Вернуться к уже открытому окну
_localWindowsService.CloseToWindow<MainMenuWindow>();
```


# DI-independent KoboldUi migration

The core runtime no longer references Zenject or VContainer. The same UI classes work
with a manual resolver or an adapter. This changes the public setup API.

1. Create a `WindowCatalog` asset and add each window prefab with `Local` or `Project` scope.
2. Create `UiBootstrap(localCanvas, projectCanvas)` and call `Initialize(catalog, resolver)`.
   For separate scene dependencies, pass project and local resolvers separately.
3. Open windows with `ui.Open<TWindow>()` or `ui.LocalWindows.OpenWindow<TWindow>()`.
   The service's `OpenWindow<T>()` and `CloseToWindow<T>()` signatures remain available.
4. Replace Zenject `[Inject]` on UI components with `[UiInject]`; use
   `[UiInject(Optional = true)]` for optional services. Controller dependencies are
   constructor parameters; the view can be a constructor parameter or the inherited
   `View` member.
5. Register default animation parameter assets under their concrete types.

Bootstrap registers every window before injecting components and initializing controllers.
Collection items are injected once when instantiated, including items later reused by a pool.

## Without DI

```csharp
var project = new ManualUiResolver()
    .AddInstance<ISettingsStore>(settingsStore)
    .AddInstance(fadeAnimationParameters);
var local = project.CreateScope().AddInstance<ILevelState>(levelState);
ui.Initialize(catalog, project, local);
```

`AddFactory<T>(() => CreateService())` creates a fresh value per resolution. A scope
can override a parent registration; duplicates within one scope throw.

## Zenject

Install Zenject and enable `KOBOLD_ZENJECT_SUPPORT`. The `KoboldUi.Zenject` assembly
provides `ZenjectUiResolver`, migrated installers and the legacy
`BindWindowFromPrefab` extension. New code can use:

```csharp
ui.Initialize(catalog, new ZenjectUiResolver(projectContainer),
    new ZenjectUiResolver(sceneContainer));
```

The legacy extension registers its window immediately and injects and initializes it
during Zenject's `IInitializable` phase after bindings are complete. It also supports
existing Zenject `[Inject]` fields on window components and controllers. Collection
item prefabs created later must migrate their injected members to `[UiInject]`.

## VContainer

Install VContainer and enable `KOBOLD_VCONTAINER_SUPPORT`. The `KoboldUi.VContainer`
assembly provides `VContainerUiResolver`:

```csharp
ui.Initialize(catalog, new VContainerUiResolver(projectScope),
    new VContainerUiResolver(sceneScope));
```

The two scopes are `VContainer.IObjectResolver` instances. UI classes and the catalog
are identical across all three presets. An adapter assembly is excluded when its
define is absent.

## IL2CPP and managed stripping

The activator uses reflection for controller constructors and `[UiInject]` members.
For application classes used only through reflection, mark constructors and injected
private members with `UnityEngine.Scripting.Preserve`, or add a `link.xml` in the
application project:

```xml
<linker>
  <assembly fullname="Assembly-CSharp">
    <type fullname="Game.Ui.SettingsController" preserve="all" />
    <type fullname="Game.Ui.SettingsWindow" preserve="all" />
  </assembly>
</linker>
```

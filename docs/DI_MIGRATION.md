# KoboldUi composition migration

KoboldUi runs without a DI container. `IUiResolver`, `ManualUiResolver`, and the
framework-specific `[UiInject]` attribute have been removed. The core does not
inspect constructors or members for injection metadata.

## Without DI

Register the window prefabs, then open the UI:

```csharp
var ui = new UiBootstrap(localCanvas, projectCanvas);
ui.RegisterWindow(settingsPrefab);
ui.Open<SettingsWindow>();
```

Controllers with a public parameterless constructor need no registration:

```csharp
protected override void AddControllers()
{
    AddController<SettingsController, SettingsView>(settingsView);
}
```

Use the second `AddController` overload when a controller needs custom constructor arguments:

```csharp
protected override void AddControllers()
{
    AddController(settingsView, view => new SettingsController(view, settings));
}
```

For dependencies that must be passed to a window before `Initialize`, use the
typed `RegisterWindow` callback. It runs after all windows are registered and
before any window is initialized:

```csharp
ui.RegisterWindow(settingsPrefab, configure: window =>
    window.Configure(settings, ui.LocalWindows));
```

For shared animation parameters, create a `UiAnimationDefaults` asset, add
parameter assets, and pass it to the `UiBootstrap` constructor.
Custom per-prefab parameters remain serialized on the animation component.

## With Zenject or VContainer

Enable `KOBOLD_ZENJECT_SUPPORT` or `KOBOLD_VCONTAINER_SUPPORT` and create the
matching adapter:

```csharp
var ui = new UiBootstrap(localCanvas, projectCanvas, new ZenjectUiCompositionAdapter(container));
ui.RegisterWindow(settingsPrefab);
```

For VContainer, use the corresponding adapter instead:

```csharp
var ui = new UiBootstrap(localCanvas, projectCanvas, new VContainerUiCompositionAdapter(scope));
ui.RegisterWindow(settingsPrefab);
```

Pass project and local adapters separately when their containers differ:

```csharp
var ui = new UiBootstrap(localCanvas, projectCanvas, projectAdapter, localAdapter, null);
ui.RegisterWindow(settingsPrefab);
```

The same `AddController<TController, TView>(view)` call delegates controller
creation to the adapter when one is configured. The view and available KoboldUi
window services are passed as explicit runtime arguments. A controller without
DI can instead use a public parameterless constructor.

`IUiCompositionAdapter.Inject` applies the container's native member injection
to existing windows, views, and other components. `Create<T>(arguments)` creates
controllers with explicit runtime arguments, such as their view, plus container
dependencies. KoboldUi calls each method once per new object. Container-specific
attributes such as Zenject or VContainer `[Inject]` remain supported inside
those projects. There is no KoboldUi injection attribute.

KoboldUi window services are available as `ui.LocalWindows` and
`ui.ProjectWindows`. Pass them through controller factories when using no DI.
The adapter receives them as runtime constructor arguments; they are not
registered with an external container.

The application owns the Zenject or VContainer scope. Disposing `UiBootstrap`
does not dispose it. If IL2CPP stripping affects objects created by a DI adapter,
follow that container's preservation or code-generation instructions.

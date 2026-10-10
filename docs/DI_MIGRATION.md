# KoboldUi composition migration

KoboldUi runs without a DI container. `IUiResolver`, `ManualUiResolver`, and the
framework-specific `[UiInject]` attribute have been removed. The core does not
inspect constructors or members for injection metadata.

## Without DI

Create a `WindowCatalog`, then initialize and open the UI:

```csharp
var ui = new UiBootstrap(localCanvas, projectCanvas);
ui.Initialize(catalog);
ui.Open<SettingsWindow>();
```

Create controllers through factories in each `AWindow`:

```csharp
protected override void AddControllers()
{
    AddController(settingsView, view => new SettingsController(view, settings));
}
```

For dependencies that must be passed to a window before `Initialize`, use the
`configureWindow` callback. It runs after all windows are registered and before
any window is initialized:

```csharp
ui.Initialize(catalog, configureWindow: window =>
{
    if (window is SettingsWindow settingsWindow)
        settingsWindow.Configure(settings, ui.LocalWindows);
});
```

For shared animation parameters, create a `UiAnimationDefaults` asset, add
parameter assets, and pass it to `ui.Initialize(catalog, animationDefaults)`.
Custom per-prefab parameters remain serialized on the animation component.

## With Zenject or VContainer

Enable `KOBOLD_ZENJECT_SUPPORT` or `KOBOLD_VCONTAINER_SUPPORT` and create the
matching adapter:

```csharp
ui.Initialize(catalog, new ZenjectUiCompositionAdapter(container));
// or
ui.Initialize(catalog, new VContainerUiCompositionAdapter(scope));
```

Pass project and local adapters separately when their containers differ:

```csharp
ui.Initialize(catalog, projectAdapter, localAdapter);
```

`IUiCompositionAdapter.Inject` applies the container's native member injection
to existing windows, views, and other components. `Create<T>(arguments)` creates
controllers with explicit runtime arguments, such as their view, plus container
dependencies. KoboldUi calls each method once per new object. Container-specific
attributes such as Zenject or VContainer `[Inject]` remain supported inside
those projects. There is no KoboldUi injection attribute.

KoboldUi window services are available as `ui.LocalWindows` and
`ui.ProjectWindows`. Pass them explicitly through `configureWindow`, controller
factories, or `AddInjectedController` arguments. They are no longer silently
registered with an external container.

The application owns the Zenject or VContainer scope. Disposing `UiBootstrap`
does not dispose it. If IL2CPP stripping affects objects created by a DI adapter,
follow that container's preservation or code-generation instructions.

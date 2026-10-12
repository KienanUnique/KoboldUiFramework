# VContainer preset

Install the `jp.hadashikick.vcontainer` package and add
`KOBOLD_VCONTAINER_SUPPORT` to the project's scripting define symbols. The
`KoboldUi.VContainer` assembly is enabled only when that symbol is present.

Call `RegisterKoboldUi` in a `LifetimeScope.Configure` method, then register the window prefabs:

```csharp
var ui = builder.RegisterKoboldUi(localCanvas, WindowScope.Local);
ui.RegisterWindow(settingsPrefab);
```

The matching window service is available from the container. The preset attaches the resolver and initializes windows after the container builds, so dependencies registered later in `Configure` are available. Use `registerWindows: ui => ui.RegisterWindow(prefab)` if registration needs the built container. Pass an explicit `UiAnimationDefaults` asset through `animationDefaults: defaults` when needed; the preset does not resolve it from VContainer. A local asset can reference the project asset and override selected parameter types. `UiInitializationMode.Manual` requires an explicit `ui.Initialize()` before using windows.

For custom composition, `VContainerUiCompositionAdapter` still accepts an existing `IObjectResolver`. It creates controllers with explicit runtime arguments and injects Unity components through `IObjectResolver.Inject`. The application owns the VContainer scope.

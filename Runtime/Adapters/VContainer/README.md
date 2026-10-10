# VContainer preset

Install the `jp.hadashikick.vcontainer` package and add
`KOBOLD_VCONTAINER_SUPPORT` to the project's scripting define symbols. The
`KoboldUi.VContainer` assembly is enabled only when that symbol is present.

Pass the active VContainer `IObjectResolver` (typically a `LifetimeScope.Container`)
to `VContainerUiCompositionAdapter`, then pass the adapter to the `UiBootstrap` constructor:

```csharp
IUiCompositionAdapter adapter = new VContainerUiCompositionAdapter(lifetimeScope.Container);
var ui = new UiBootstrap(localCanvas, projectCanvas, adapter);
ui.RegisterWindow(settingsPrefab);
```

Register dependencies with VContainer before creating UI objects. The adapter
creates controllers with VContainer's injector and explicit constructor arguments,
and injects existing Unity components through `IObjectResolver.Inject`. It does
not dispose the VContainer scope; the application owns its lifetime.

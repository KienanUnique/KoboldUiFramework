# Zenject preset

Add `KOBOLD_ZENJECT_SUPPORT` to the project's scripting define symbols and install Zenject. The `KoboldUi.Zenject` assembly provides `BindKoboldUi` and `ZenjectUiCompositionAdapter`.

Call `BindKoboldUi` in a Zenject installer, then register the window prefabs. The matching window service is bound immediately, so other services can receive it through a normal constructor. Window instances are initialized when Zenject calls `IInitializable.Initialize`.

```csharp
var ui = Container.BindKoboldUi(canvas, WindowScope.Project);
ui.RegisterWindow(loadingWindow, WindowScope.Project);
```

Use `WindowScope.Local` in a scene installer. Pass a serialized `UiAnimationDefaults` asset through `animationDefaults: defaults` when needed; the preset does not resolve it from Zenject. A local asset can reference the project asset and override selected parameter types. If registration itself needs bindings created by later installers, pass `registerWindows: ui => ui.RegisterWindow(prefab)`; that callback runs during Zenject initialization. `UiInitializationMode.Manual` disables automatic window initialization, and the application must call `ui.Initialize()` before using windows. The adapter uses Zenject's constructor and member injection; KoboldUi defines no injection attribute.

<div align="center" valign="middle" style="page-break-after: always;">
<br>
    <h1>Kobold UI</h1>
    <img src="README-assets~/logo.png" width = "256" />
    <h2>uGUI framework for Unity</h2>
    <br>
</div>

# Dependencies
- **Unitask** – a package for more efficient and convenient async/await handling in Unity
  * [https://github.com/Cysharp/UniTask](https://github.com/Cysharp/UniTask)
- **Dotween** – a plugin for tween animations
  * [https://dotween.demigiant.com](https://dotween.demigiant.com/getstarted.php)
- **Alchemy** (optional) – extensions for the inspector
  * [https://github.com/AnnulusGames/Alchemy](https://github.com/AnnulusGames/Alchemy)
- **Odin Inspector** (optional) – alternative inspector extensions
  * [https://odininspector.com](https://odininspector.com)


# Installation
1. Install UniTask and DOTween. A DI container is optional.
2. Generate the `.asmdef` for **Dotween**:
`Tools -> Demigiant -> DOTween Utility Panel -> Create .asmdef`
3. Add the define to:`Edit -> Project Settings -> Player -> Scripting Define Symbols`
```
UNITASK_DOTWEEN_SUPPORT
```
4. If you installed **Alchemy**, also add the define:
```
KOBOLD_ALCHEMY_SUPPORT
```
5. If you installed **Odin Inspector**, add the define:
```
KOBOLD_ODIN_SUPPORT
```
6. Install the plugin via Git: 
```
https://github.com/KienanUnique/KoboldUiFramework.git
```

# Configure windows

Register window prefabs directly. Windows use the local scope by default; pass
`WindowScope.Project` for application-wide windows.

```csharp
using KoboldUi.Composition;
var ui = new UiBootstrap(localCanvas, projectCanvas);
ui.RegisterWindow(settingsPrefab);
ui.Open<SettingsWindow>();
```

`ui.LocalWindows` and `ui.ProjectWindows` can be passed to services immediately after creating `UiBootstrap`; obtaining either service does not create windows. Without DI, the first window operation initializes registered windows automatically. Call `ui.Initialize()` to do this explicitly. Pass `UiInitializationMode.Manual` to require explicit initialization.

For controllers with a public parameterless constructor, use
`AddController<SettingsController, SettingsView>(view)` in the window. To pass
custom dependencies without DI, use the factory overload:
`AddController(view, v => new SettingsController(v, settingsStore))`.
Existing windows and views can receive dependencies through explicit `Configure`
methods called before initialization.
Shared animation settings live in a `UiAnimationDefaults` asset, passed to
`new UiBootstrap(localCanvas, projectCanvas, animationDefaults: animationDefaults)`.
The argument is optional and does not depend on DI. A local defaults asset can
reference a project defaults asset in its `Project Defaults` field: its own
parameters override matching project parameter types, while missing types
fall back to the project asset.

Optional Zenject and VContainer presets use separate assemblies and the `KOBOLD_ZENJECT_SUPPORT` or `KOBOLD_VCONTAINER_SUPPORT` define. In installers, call `Container.BindKoboldUi(canvas, WindowScope.Local)` or `builder.RegisterKoboldUi(canvas, WindowScope.Local)`, then `ui.RegisterWindow(prefab)`. See [DI migration](docs/DI_MIGRATION.md).


# Useful Links
- [Documentation (En)](https://github.com/KienanUnique/KoboldUiFramework/wiki/Documentation-EN)
- [Documentation (Ru)](https://github.com/KienanUnique/KoboldUiFramework/wiki/Documentation-RU)
- [Start Guide (En)](https://github.com/KienanUnique/KoboldUiFramework/wiki/Start-Guide-EN)
- [Start Guide (Ru)](https://github.com/KienanUnique/KoboldUiFramework/wiki/Start-Guide-RU)
- [Sample](https://github.com/KienanUnique/KoboldUiSample)

<div align="center" valign="middle" style="page-break-after: always;">
<br>
    <img src="README-assets~/demo.gif" width = "780" />
<br>
</div>

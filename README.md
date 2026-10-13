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
var ui = new UiBootstrapBuilder(localCanvas, projectCanvas)
    .RegisterWindow(settingsPrefab)
    .Build();
ui.Open<SettingsWindow>();
```

`ui.LocalWindows` and `ui.ProjectWindows` are available after `Build()`; obtaining either service does not create windows. Without DI, the first window operation initializes registered windows automatically. Call `ui.Initialize()` to do this explicitly. Use `WithInitializationMode(UiInitializationMode.Manual)` to require explicit initialization.

For controllers with a public parameterless constructor, use
`AddController<SettingsController, SettingsView>(view)` in the window. To pass
custom dependencies without DI, use the factory overload:
`AddController(view, v => new SettingsController(v, settingsStore))`.
Existing windows and views can receive dependencies through explicit `Configure`
methods called before initialization.
Fade, scale, and slide animations use built-in values without assets. To override
one type for a UI scope, call `.AddDefaultParameters(fadeParameters)` before
`.Build()`. Concrete parameter ScriptableObjects remain available for this and
for individual prefab settings. A local builder can inherit project overrides
with `.WithProjectDefaults(projectUi.AnimationDefaults)`.

Optional Zenject and VContainer presets use separate assemblies and the `KOBOLD_ZENJECT_SUPPORT` or `KOBOLD_VCONTAINER_SUPPORT` define. In installers, call `Container.BindKoboldUi(canvas, WindowScope.Local).RegisterWindow(prefab).Build()` or the equivalent `builder.RegisterKoboldUi(...)`. See [DI migration](docs/DI_MIGRATION.md).


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

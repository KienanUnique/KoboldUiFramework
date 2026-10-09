# Zenject preset

Add `KOBOLD_ZENJECT_SUPPORT` to the project's scripting define symbols and install Zenject. The `KoboldUi.Zenject` assembly then provides `ZenjectUiResolver`, the existing Zenject installers, and `BindWindowFromPrefab`.

Create `new ZenjectUiResolver(container)` when wiring `UiBootstrap` directly. The local and project windows service installers register the resolver and window registry in their respective containers.

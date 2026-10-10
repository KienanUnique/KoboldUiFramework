# Zenject preset

Add `KOBOLD_ZENJECT_SUPPORT` to the project's scripting define symbols and install Zenject. The `KoboldUi.Zenject` assembly provides `ZenjectUiCompositionAdapter`, the Zenject installers, and `BindWindowFromPrefab`.

Pass `new ZenjectUiCompositionAdapter(container)` to the `UiBootstrap` constructor, then call `RegisterWindow` for each prefab. The adapter uses Zenject's own constructor and member injection. KoboldUi does not define an injection attribute. Local and project windows can use separate containers.

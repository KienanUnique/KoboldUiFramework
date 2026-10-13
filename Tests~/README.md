# KoboldUi regression tests

Unity ignores the `Tests~` folder when importing the package. To run these tests,
use a disposable checkout of the package as an embedded or local package and
rename `Tests~` to `Tests` before opening the Unity project. The test assembly
definitions then become available to the Test Runner. Keep the published package
folder named `Tests~` so consumer projects do not import the tests.

The `Editor` assembly covers navigation, window services, catalog/bootstrap ordering, explicit controller factories, optional composition adapters, collections, and animation defaults. Optional `KoboldUi.Tests.Zenject` and `KoboldUi.Tests.VContainer` assemblies test each adapter when its define and package are installed. The `PlayMode` assembly checks prefab creation, button observables and fade transitions.

Run from the Unity project root with Unity 2022.3.62f2:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\2022.3.62f2\Editor\Unity.exe' -batchmode -nographics -projectPath . -runTests -testPlatform EditMode -assemblyNames KoboldUi.Tests.Editor -testResults Logs/KoboldUiEditModeResults.xml -logFile Logs/KoboldUiEditMode.log
```

Check the XML `test-run` attributes `result`, `total`, and `failed`. Do not add `-quit` to this invocation: the Unity Test Framework exits after writing the results.

Run the Play Mode smoke test separately:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\2022.3.62f2\Editor\Unity.exe' -batchmode -projectPath . -runTests -testPlatform PlayMode -assemblyNames KoboldUi.Tests.PlayMode -testResults Logs/KoboldUiPlayModeResults.xml -logFile Logs/KoboldUiPlayMode.log
```

The Play Mode command omits `-nographics` because this project's test runner needs an initialized graphics device when it enters Play Mode.

The core test assemblies contain no DI reference. Adapter tests are enabled separately by `KOBOLD_ZENJECT_SUPPORT` or `KOBOLD_VCONTAINER_SUPPORT`.

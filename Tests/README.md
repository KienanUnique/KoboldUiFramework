# KoboldUi regression tests

The `Editor` assembly characterizes KoboldUi 1.1.9 behavior before the DI migration. It covers window navigation policies, stack ordering, the public windows service, prefab binding, controller and view initialization, collection item injection, and default animation parameters. The `PlayMode` assembly checks prefab creation, dependency injection, controller initialization, a real UI Button observable, and deterministic fade transitions in the Unity lifecycle.

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

The test assembly intentionally references Zenject while characterizing the current implementation. After the DI migration, move the Zenject-specific cases to the optional adapter tests and keep the navigation and lifecycle tests in the core test assembly.

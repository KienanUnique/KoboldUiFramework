# Controller Overloads Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Keep exactly two `AWindow.AddController` overloads: default creation and an explicit factory.

**Architecture:** The no-factory overload delegates to `UiActivator`, which uses the optional composition adapter or a public parameterless constructor. The factory overload keeps explicit construction. Built-in window services are passed as explicit runtime arguments to adapters, so DI controllers need no special window method.

**Tech Stack:** Unity 2022.3.62f2, C#, NUnit Unity Test Framework.

## Global Constraints

- No `UiInject` or framework member scanning.
- No mandatory DI dependency in the runtime assembly.
- Keep both DI adapters in their own asmdefs.
- Do not push this branch.

---

### Task 1: Default controller creation

**Files:** `Tests/Editor/CompositionTests.cs`, `Tests/Editor/WindowInitializationTests.cs`, `Runtime/Composition/UiActivator.cs`.

- [x] Add tests for parameterless creation without DI, view binding, and the missing-constructor error.
- [x] Run targeted tests and observe failure.
- [x] Implement the adapter-or-parameterless creation path.
- [x] Run targeted tests again.

### Task 2: Two public window overloads

**Files:** `Runtime/Windows/AWindow.cs`, `Assets/Examples/DiComparison/Scripts/DemoMainWindow.cs`, `Assets/Examples/DiComparison/Scripts/DemoSettingsWindow.cs`, package docs and tests.

- [x] Add a test that a no-DI window initializes through `AddController<TController,TView>(view)`.
- [x] Run targeted test and observe failure.
- [x] Remove `AddInjectedController`, update DI demo calls to `AddController`, and retain the factory overload.
- [x] Update examples and docs to show the two overloads.
- [x] Run EditMode and PlayMode tests, build C#, and check the diff.

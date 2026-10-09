# VContainer preset

Install the `jp.hadashikick.vcontainer` package and add
`KOBOLD_VCONTAINER_SUPPORT` to the project's scripting define symbols. The
`KoboldUi.VContainer` assembly is enabled only when that symbol is present.

Pass the active VContainer `IObjectResolver` (typically a `LifetimeScope.Container`)
to `VContainerUiResolver`, then pass the adapter wherever KoboldUi requests an
`IUiResolver`:

```csharp
IUiResolver resolver = new VContainerUiResolver(lifetimeScope.Container);
```

Register controllers and other dependencies with VContainer before using the
resolver. The adapter uses `TryResolve` and does not dispose the VContainer
scope; the application owns its lifetime.

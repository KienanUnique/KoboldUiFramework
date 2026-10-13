using System;
using KoboldUi.Services.WindowsService;
using UnityEngine;
using Zenject;

namespace KoboldUi.Composition.Zenject
{
    public static class ZenjectUiRegistrationExtensions
    {
        public static UiBootstrapBuilder BindKoboldUi(this DiContainer container, Canvas canvas, WindowScope scope,
            Action<UiBootstrap> registerWindows = null,
            UiInitializationMode initializationMode = UiInitializationMode.Automatic)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (canvas == null) throw new ArgumentNullException(nameof(canvas));
            if (scope != WindowScope.Local && scope != WindowScope.Project)
                throw new ArgumentOutOfRangeException(nameof(scope));

            return new UiBootstrapBuilder(canvas, canvas, scope, ui =>
                {
                    ui.WaitForComposition();
                    if (scope == WindowScope.Local)
                        container.Bind<ILocalWindowsService>().FromInstance(ui.LocalWindows).AsSingle();
                    else
                        container.Bind<IProjectWindowsService>().FromInstance(ui.ProjectWindows).AsSingle();
                    container.Bind<IDisposable>().FromInstance(ui).AsCached();
                    container.Bind<IInitializable>().FromInstance(new UiInitializer(ui, registerWindows)).AsCached();
                })
                .WithAdapter(new ZenjectUiCompositionAdapter(container))
                .WithInitializationMode(initializationMode);
        }

        private sealed class UiInitializer : IInitializable
        {
            private readonly UiBootstrap _ui;
            private readonly Action<UiBootstrap> _registerWindows;
            private bool _initialized;

            public UiInitializer(UiBootstrap ui, Action<UiBootstrap> registerWindows)
            {
                _ui = ui;
                _registerWindows = registerWindows;
            }

            public void Initialize()
            {
                if (_initialized) return;
                _initialized = true;
                _registerWindows?.Invoke(_ui);
                _ui.CompleteComposition();
            }
        }
    }
}

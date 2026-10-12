using System;
using KoboldUi.Composition;
using KoboldUi.Element.Animations;
using KoboldUi.Services.WindowsService;
using UnityEngine;
using VContainer;

namespace KoboldUi.Adapters.VContainer
{
    public static class VContainerUiRegistrationExtensions
    {
        public static UiBootstrap RegisterKoboldUi(this IContainerBuilder builder, Canvas canvas, WindowScope scope,
            Action<UiBootstrap> registerWindows = null, UiAnimationDefaults animationDefaults = null,
            UiInitializationMode initializationMode = UiInitializationMode.Automatic)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (canvas == null) throw new ArgumentNullException(nameof(canvas));
            if (scope != WindowScope.Local && scope != WindowScope.Project)
                throw new ArgumentOutOfRangeException(nameof(scope));

            var adapter = new VContainerUiCompositionAdapter();
            var ui = new UiBootstrap(canvas, canvas, adapter, animationDefaults, initializationMode);
            builder.RegisterInstance(ui);
            if (scope == WindowScope.Local)
                builder.RegisterInstance<ILocalWindowsService>(ui.LocalWindows);
            else
                builder.RegisterInstance<IProjectWindowsService>(ui.ProjectWindows);
            builder.RegisterBuildCallback(container =>
            {
                adapter.Attach(container);
                registerWindows?.Invoke(ui);
                if (ui.InitializationMode == UiInitializationMode.Automatic) ui.Initialize();
            });
            return ui;
        }
    }
}

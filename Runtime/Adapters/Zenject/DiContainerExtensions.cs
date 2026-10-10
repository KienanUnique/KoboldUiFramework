using System;
using KoboldUi.Composition;
using KoboldUi.Composition.Zenject;
using KoboldUi.Windows;
using KoboldUi.Element.Animations;
using UnityEngine;
using Zenject;

namespace KoboldUi.Utils
{
    /// <summary>Provides Zenject bindings for window prefabs.</summary>
    public static class DiContainerExtensions
    {
        public static void BindWindowFromPrefab<T>(this DiContainer container, Canvas canvas, T windowPrefab)
            where T : AWindowBase
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (canvas == null) throw new ArgumentNullException(nameof(canvas));
            if (windowPrefab == null) throw new ArgumentNullException(nameof(windowPrefab));

            var staging = new GameObject("KoboldUi Zenject window staging");
            staging.SetActive(false);
            try
            {
                var window = UnityEngine.Object.Instantiate(windowPrefab, staging.transform);
                container.Bind<WindowRegistry>().AsSingle().IfNotBound();
                container.Resolve<WindowRegistry>().Register(window);
                container.BindInterfacesAndSelfTo<T>().FromInstance(window).AsSingle();
                container.Bind<IInitializable>()
                    .FromInstance(new ZenjectWindowInitializer(container, canvas, staging, window))
                    .AsCached();
            }
            catch
            {
                DestroyStaging(staging);
                throw;
            }
        }

        private static void DestroyStaging(GameObject staging)
        {
            if (Application.isPlaying) UnityEngine.Object.Destroy(staging);
            else UnityEngine.Object.DestroyImmediate(staging);
        }

        private sealed class ZenjectWindowInitializer : IInitializable
        {
            private readonly DiContainer _container;
            private readonly Canvas _canvas;
            private readonly GameObject _staging;
            private readonly AWindowBase _window;

            public ZenjectWindowInitializer(DiContainer container, Canvas canvas, GameObject staging,
                AWindowBase window)
            {
                _container = container;
                _canvas = canvas;
                _staging = staging;
                _window = window;
            }

            public void Initialize()
            {
                try
                {
                    var defaults = _container.HasBinding<UiAnimationDefaults>()
                        ? _container.Resolve<UiAnimationDefaults>() : null;
                    var activator = new UiActivator(new ZenjectUiCompositionAdapter(_container), defaults);
                    _window.Configure(activator);
                    activator.PrepareGameObject(_window.gameObject);
                    _window.transform.SetParent(_canvas.transform, false);
                    _window.Initialize();
                }
                finally
                {
                    DestroyStaging(_staging);
                }
            }
        }
    }
}

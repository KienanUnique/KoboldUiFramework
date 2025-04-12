using KoboldUi.Windows;
using UnityEngine;

#if KOBOLD_ZENJECT_SUPPORT
using Zenject;
#elif KOBOLD_VCONTAINER_SUPPORT
using VContainer;
using VContainer.Unity;
#endif

namespace KoboldUi.Utils
{
    public static class DiContainerExtensions
    {
#if KOBOLD_ZENJECT_SUPPORT
        public static void BindWindowFromPrefab<T>(this DiContainer container, Canvas canvas, T windowPrefab)
            where T : AWindowBase, IInitializable
        {
            var window = Object.Instantiate(windowPrefab, canvas.transform);
            window.InstallBindings(container);
            container.QueueForInject(window);
            container.BindInterfacesAndSelfTo<T>().FromInstance(window).AsSingle();
        }
#elif KOBOLD_VCONTAINER_SUPPORT
        public static void BindWindowFromPrefab<T>(this IContainerBuilder builder, Canvas canvas, T windowPrefab)
            where T : AWindowBase, IInitializable
        {
            builder.Register<T>(resolver =>
            {
                var window = Object.Instantiate(windowPrefab, canvas.transform);
                resolver.InjectGameObject(window.gameObject);
                window.InstallBindings(resolver);
                return window;
            }, Lifetime.Singleton).AsSelf();
        }
#endif
}
}
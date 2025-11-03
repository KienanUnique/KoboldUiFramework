using KoboldUi.Windows;
using UnityEngine;
using Zenject;

namespace KoboldUi.Utils
{
    /// <summary>
    /// Provides helper extensions for installing window prefabs into the DI container.
    /// </summary>
    public static class DiContainerExtensions
    {
        /// <summary>
        /// Instantiates a window prefab under the provided canvas and binds it to the container.
        /// </summary>
        /// <typeparam name="T">Type of the window component.</typeparam>
        /// <param name="container">DI container receiving the bindings.</param>
        /// <param name="canvas">Canvas acting as parent for the window instance.</param>
        /// <param name="windowPrefab">Prefab to instantiate and bind.</param>
        public static void BindWindowFromPrefab<T>(this DiContainer container, Canvas canvas, T windowPrefab)
            where T : AWindowBase, IInitializable
        {
            var window = Object.Instantiate(windowPrefab, canvas.transform);
            window.InstallBindings(container);
            container.QueueForInject(window);
            container.BindInterfacesAndSelfTo<T>().FromInstance(window).AsSingle();
        }
    }
}
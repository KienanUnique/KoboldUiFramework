using KoboldUi.Windows;
using UnityEngine;
using Zenject;

namespace KoboldUi.Utils
{
    public static class DiContainerExtensions
    {
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
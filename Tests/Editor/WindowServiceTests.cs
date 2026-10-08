using System.Collections.Generic;
using KoboldUi.Services.WindowsService.Impl;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Impl.Common;
using KoboldUi.UiAction.Pool;
using KoboldUi.Utils;
using KoboldUi.Windows;
using NUnit.Framework;
using Zenject;

namespace KoboldUi.Tests
{
    public class WindowServiceTests
    {
        [Test]
        public void OpenWindowResolvesRegisteredInstanceAndInvokesCallbackAfterActivation()
        {
            var container = new DiContainer();
            var window = new ServiceWindow();
            container.BindInstance(window).AsSingle();
            var service = new LocalWindowsService(container);
            var callbackSawActiveWindow = false;

            try
            {
                service.OpenWindow<ServiceWindow>(() =>
                    callbackSawActiveWindow = service.CurrentWindow == window &&
                                            window.States.Contains(EWindowState.Active));

                Assert.That(service.CurrentWindow, Is.SameAs(window));
                Assert.That(service.IsOpened<ServiceWindow>(), Is.True);
                Assert.That(callbackSawActiveWindow, Is.True);
            }
            finally
            {
                service.Dispose();
            }
        }

        [Test]
        public void CloseWindowClosesRegisteredInstanceAndClearsCurrentWindow()
        {
            var container = new DiContainer();
            var window = new ServiceWindow();
            container.BindInstance(window).AsSingle();
            var service = new LocalWindowsService(container);

            try
            {
                service.OpenWindow<ServiceWindow>();
                service.CloseWindow(null, false);

                Assert.That(window.States, Is.EqualTo(new[]
                {
                    EWindowState.Active, EWindowState.Closed
                }));
                Assert.That(service.CurrentWindow, Is.Null);
                Assert.That(service.IsOpened<ServiceWindow>(), Is.False);
            }
            finally
            {
                service.Dispose();
            }
        }

        private sealed class ServiceWindow : IWindow
        {
            public bool IsInitialized => true;
            public string Name => nameof(ServiceWindow);
            public bool IsPopup => false;
            public bool IsBackLogicIgnorable => false;
            public List<EWindowState> States { get; } = new List<EWindowState>();

            public IUiAction WaitInitialization(in IUiActionsPool pool)
            {
                pool.GetAction(out EmptyAction action);
                return action;
            }

            public IUiAction SetState(EWindowState state, in IUiActionsPool pool)
            {
                States.Add(state);
                pool.GetAction(out EmptyAction action);
                return action;
            }

            public void ApplyOrder(int order)
            {
            }
        }
    }
}

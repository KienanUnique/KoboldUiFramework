using System.Collections.Generic;
using KoboldUi.Services.WindowsService;
using KoboldUi.UiAction;
using KoboldUi.UiAction.Impl.Common;
using KoboldUi.UiAction.Impl.Service;
using KoboldUi.UiAction.Pool;
using KoboldUi.UiAction.Pool.Impl;
using KoboldUi.Utils;
using KoboldUi.Windows;
using KoboldUi.WindowsStack.Impl;
using NUnit.Framework;

namespace KoboldUi.Tests
{
    public class WindowNavigationTests
    {
        private WindowsStackHolder _stack;
        private UiActionsPool _pool;

        [SetUp]
        public void SetUp()
        {
            _stack = new WindowsStackHolder();
            _pool = new UiActionsPool(_stack);
            _pool.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            _pool.Dispose();
        }

        [Test]
        public void RemoveMiddleWindowPreservesStackOrder()
        {
            var first = new RecordingWindow();
            var middle = new RecordingWindow();
            var top = new RecordingWindow();
            _stack.Push(first);
            _stack.Push(middle);
            _stack.Push(top);

            Assert.That(_stack.Remove(middle), Is.True);
            Assert.That(_stack.CurrentWindow, Is.SameAs(top));
            Assert.That(_stack.Pop(), Is.SameAs(top));
            Assert.That(_stack.Pop(), Is.SameAs(first));
            Assert.That(_stack.IsEmpty, Is.True);
        }

        [Test]
        public void OpenWindowClosesPreviousWindowBeforeActivatingNext()
        {
            var previous = new RecordingWindow();
            var next = new RecordingWindow();
            _stack.Push(previous);

            _pool.GetAction(out OpenWindowAction action, next, EPreviousWindowPolicy.Default);
            action.Start().GetAwaiter().GetResult();

            Assert.That(previous.States, Is.EqualTo(new[] { EWindowState.Closed }));
            Assert.That(next.States, Is.EqualTo(new[] { EWindowState.Active }));
            Assert.That(next.AppliedOrder, Is.EqualTo(int.MaxValue));
            Assert.That(_stack.CurrentWindow, Is.SameAs(next));
            Assert.That(_stack.Contains(previous), Is.True);
        }

        [Test]
        public void OpeningPopupRemovesFocusWithoutClosingPreviousWindow()
        {
            var previous = new RecordingWindow();
            var popup = new RecordingWindow(isPopup: true);
            _stack.Push(previous);

            _pool.GetAction(out OpenWindowAction action, popup, EPreviousWindowPolicy.Default);
            action.Start().GetAwaiter().GetResult();

            Assert.That(previous.States, Is.EqualTo(new[] { EWindowState.NonFocused }));
            Assert.That(popup.States, Is.EqualTo(new[] { EWindowState.Active }));
            Assert.That(_stack.CurrentWindow, Is.SameAs(popup));
        }

        [Test]
        public void CloseAndForgetRemovesPreviousWindowFromStack()
        {
            var previous = new RecordingWindow();
            var next = new RecordingWindow();
            _stack.Push(previous);

            _pool.GetAction(out OpenWindowAction action, next, EPreviousWindowPolicy.CloseAndForget);
            action.Start().GetAwaiter().GetResult();

            Assert.That(previous.States, Is.EqualTo(new[] { EWindowState.Closed }));
            Assert.That(_stack.Contains(previous), Is.False);
            Assert.That(_stack.CurrentWindow, Is.SameAs(next));
        }

        [Test]
        public void CloseAfterOpenAndForgetDefocusesThenClosesPreviousWindow()
        {
            var previous = new RecordingWindow();
            var next = new RecordingWindow();
            _stack.Push(previous);

            _pool.GetAction(out OpenWindowAction action, next,
                EPreviousWindowPolicy.CloseAfterOpenAndForget);
            action.Start().GetAwaiter().GetResult();

            Assert.That(previous.States, Is.EqualTo(new[]
            {
                EWindowState.NonFocused, EWindowState.Closed
            }));
            Assert.That(next.States, Is.EqualTo(new[] { EWindowState.Active }));
            Assert.That(_stack.Contains(previous), Is.False);
            Assert.That(_stack.CurrentWindow, Is.SameAs(next));
        }

        [Test]
        public void OpeningAlreadyActiveWindowDoesNotPushItAgain()
        {
            var window = new RecordingWindow();
            _stack.Push(window);

            _pool.GetAction(out OpenWindowAction action, window, EPreviousWindowPolicy.Default);
            action.Start().GetAwaiter().GetResult();

            Assert.That(_stack.Stack.Count, Is.EqualTo(1));
            Assert.That(window.States, Is.Empty);
        }

        [Test]
        public void ClosingTopWindowReactivatesPreviousWindow()
        {
            var previous = new RecordingWindow();
            var top = new RecordingWindow();
            _stack.Push(previous);
            _stack.Push(top);

            _pool.GetAction(out CloseWindowAction action, false);
            action.Start().GetAwaiter().GetResult();

            Assert.That(top.States, Is.EqualTo(new[] { EWindowState.Closed }));
            Assert.That(previous.States, Is.EqualTo(new[] { EWindowState.Active }));
            Assert.That(_stack.CurrentWindow, Is.SameAs(previous));
        }

        [Test]
        public void ClosingToWindowClosesOnlyWindowsAboveTarget()
        {
            var target = new RecordingWindow();
            var middle = new RecordingWindow();
            var top = new RecordingWindow();
            _stack.Push(target);
            _stack.Push(middle);
            _stack.Push(top);

            _pool.GetAction(out CloseToWindowAction action, target, false);
            action.Start().GetAwaiter().GetResult();

            Assert.That(top.States, Is.EqualTo(new[] { EWindowState.Closed }));
            Assert.That(middle.States, Is.EqualTo(new[] { EWindowState.Closed }));
            Assert.That(target.States, Is.EqualTo(new[] { EWindowState.Active }));
            Assert.That(_stack.CurrentWindow, Is.SameAs(target));
        }

        [Test]
        public void BackLogicIgnoresMarkedWindow()
        {
            var top = new RecordingWindow(isBackLogicIgnorable: true);
            _stack.Push(top);

            _pool.GetAction(out CloseWindowAction action, true);
            action.Start().GetAwaiter().GetResult();

            Assert.That(_stack.CurrentWindow, Is.SameAs(top));
            Assert.That(top.States, Is.Empty);
        }

        [Test]
        public void CloseAllWindowsStopsAtIgnorableWindow()
        {
            var bottom = new RecordingWindow();
            var ignorable = new RecordingWindow(isBackLogicIgnorable: true);
            var top = new RecordingWindow();
            _stack.Push(bottom);
            _stack.Push(ignorable);
            _stack.Push(top);

            _pool.GetAction(out CloseAllWindowsAction action, true);
            action.Start().GetAwaiter().GetResult();

            Assert.That(top.States, Is.EqualTo(new[] { EWindowState.Closed }));
            Assert.That(ignorable.States, Is.Empty);
            Assert.That(bottom.States, Is.Empty);
            Assert.That(_stack.CurrentWindow, Is.SameAs(ignorable));
        }

        private sealed class RecordingWindow : IWindow
        {
            public RecordingWindow(bool isPopup = false, bool isBackLogicIgnorable = false)
            {
                IsPopup = isPopup;
                IsBackLogicIgnorable = isBackLogicIgnorable;
            }

            public bool IsInitialized => true;
            public string Name => nameof(RecordingWindow);
            public bool IsPopup { get; }
            public bool IsBackLogicIgnorable { get; }
            public int AppliedOrder { get; private set; }
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
                AppliedOrder = order;
            }
        }
    }
}

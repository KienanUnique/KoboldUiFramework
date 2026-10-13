using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using KoboldUi.Element.Animations;
using KoboldUi.UiAction.Pool.Impl;
using KoboldUi.WindowsStack.Impl;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace KoboldUi.Tests.PlayMode
{
    public class AsyncAnimationPlayModeTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private UiActionsPool _pool;

        [SetUp]
        public void SetUp()
        {
            _pool = new UiActionsPool(new WindowsStackHolder());
            _pool.Initialize();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            foreach (var item in _objects)
            {
                if (item != null)
                    UnityEngine.Object.Destroy(item);
            }

            _objects.Clear();
            _pool.Dispose();
            yield return null;
        }

        [UnityTest]
        public IEnumerator WaitingActionCompletesAfterAnimationAndThenHidesView()
        {
            var animation = CreateAnimation();
            animation._needWaitAnimation = true;

            var appear = animation.Appear(_pool).Start();
            Assert.That(animation.AppearCount, Is.EqualTo(1));
            Assert.That(appear.Status, Is.EqualTo(UniTaskStatus.Pending));

            animation.CompleteAppear = true;
            yield return null;
            Assert.That(appear.Status, Is.EqualTo(UniTaskStatus.Succeeded));
            Assert.That(animation.gameObject.activeSelf, Is.True);

            var disappear = animation.Disappear(_pool).Start();
            Assert.That(disappear.Status, Is.EqualTo(UniTaskStatus.Pending));
            Assert.That(animation.gameObject.activeSelf, Is.True);

            animation.CompleteDisappear = true;
            yield return null;
            Assert.That(disappear.Status, Is.EqualTo(UniTaskStatus.Succeeded));
            Assert.That(animation.gameObject.activeSelf, Is.False);
        }

        [UnityTest]
        public IEnumerator NonWaitingActionCompletesWhileAnimationContinues()
        {
            var animation = CreateAnimation();
            animation._needWaitAnimation = false;

            var appear = animation.Appear(_pool).Start();
            Assert.That(appear.Status, Is.EqualTo(UniTaskStatus.Succeeded));
            Assert.That(animation.AppearCount, Is.EqualTo(1));
            Assert.That(animation.AppearFinished, Is.False);

            animation.CompleteAppear = true;
            yield return null;
            Assert.That(animation.AppearFinished, Is.True);

            var disappear = animation.Disappear(_pool).Start();
            Assert.That(disappear.Status, Is.EqualTo(UniTaskStatus.Succeeded));
            Assert.That(animation.gameObject.activeSelf, Is.True);

            animation.CompleteDisappear = true;
            yield return null;
            Assert.That(animation.gameObject.activeSelf, Is.False);
        }

        [UnityTest]
        public IEnumerator NewTransitionCancelsOldOneAndOldCloseCannotHideView()
        {
            var animation = CreateAnimation();
            animation._needWaitAnimation = false;

            animation.Disappear(_pool).Start().GetAwaiter().GetResult();
            Assert.That(animation.gameObject.activeSelf, Is.True);

            animation.Appear(_pool).Start().GetAwaiter().GetResult();
            yield return null;

            Assert.That(animation.DisappearWasCancelled, Is.True);
            Assert.That(animation.gameObject.activeSelf, Is.True);

            animation.CompleteAppear = true;
            yield return null;
            Assert.That(animation.gameObject.activeSelf, Is.True);
        }

        [Test]
        public void WaitingActionPropagatesAnimationError()
        {
            var gameObject = new GameObject("Faulting animation");
            _objects.Add(gameObject);
            var animation = gameObject.AddComponent<FaultingAsyncAnimation>();
            animation._needWaitAnimation = true;

            var exception = Assert.Throws<InvalidOperationException>(
                () => animation.Appear(_pool).Start().GetAwaiter().GetResult());
            Assert.That(exception.Message, Is.EqualTo("animation failed"));
        }

        [UnityTest]
        public IEnumerator NonWaitingActionPublishesAnimationErrorAfterActionReturns()
        {
            var gameObject = new GameObject("Faulting background animation");
            _objects.Add(gameObject);
            var animation = gameObject.AddComponent<FaultingAsyncAnimation>();
            animation._needWaitAnimation = false;
            animation.DelayError = true;
            Exception observed = null;
            Action<Exception> onError = exception => observed = exception;
            UniTaskScheduler.UnobservedTaskException += onError;

            try
            {
                Assert.DoesNotThrow(() => animation.Appear(_pool).Start().GetAwaiter().GetResult());
                Assert.That(observed, Is.Null);
                yield return null;
                Assert.That(observed, Is.TypeOf<InvalidOperationException>());
                Assert.That(observed.Message, Is.EqualTo("animation failed"));
            }
            finally
            {
                UniTaskScheduler.UnobservedTaskException -= onError;
            }
        }

        [UnityTest]
        public IEnumerator InstantDisappearCancelsNonWaitingAnimation()
        {
            var animation = CreateAnimation();
            animation._needWaitAnimation = false;
            animation.Disappear(_pool).Start().GetAwaiter().GetResult();

            animation.DisappearInstantly();
            yield return null;

            Assert.That(animation.DisappearWasCancelled, Is.True);
            Assert.That(animation.gameObject.activeSelf, Is.False);
        }

        private ProbeAsyncAnimation CreateAnimation()
        {
            var gameObject = new GameObject("Probe animation");
            _objects.Add(gameObject);
            return gameObject.AddComponent<ProbeAsyncAnimation>();
        }
    }

    public sealed class ProbeAsyncAnimation : AUiAsyncAnimation
    {
        public bool CompleteAppear;
        public bool CompleteDisappear;
        public bool AppearFinished;
        public bool DisappearWasCancelled;
        public int AppearCount;

        protected override async UniTask AnimateAppearAsync(CancellationToken token)
        {
            AppearCount++;
            await UniTask.WaitUntil(() => CompleteAppear, cancellationToken: token);
            AppearFinished = true;
        }

        protected override async UniTask AnimateDisappearAsync(CancellationToken token)
        {
            try
            {
                await UniTask.WaitUntil(() => CompleteDisappear, cancellationToken: token);
            }
            finally
            {
                DisappearWasCancelled = token.IsCancellationRequested;
            }
        }
    }

    public sealed class FaultingAsyncAnimation : AUiAsyncAnimation
    {
        public bool DelayError;

        protected override async UniTask AnimateAppearAsync(CancellationToken token)
        {
            if (DelayError)
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            throw new InvalidOperationException("animation failed");
        }

        protected override UniTask AnimateDisappearAsync(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
    }
}

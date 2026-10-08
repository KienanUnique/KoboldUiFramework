using System;
using DG.Tweening;
using KoboldUi.Collections.Base;
using KoboldUi.Collections.Concrete.Impl;
using KoboldUi.Element.Animations;
using KoboldUi.Element.Animations.Parameters.Impl;
using NUnit.Framework;
using UnityEngine;
using Zenject;

namespace KoboldUi.Tests
{
    public class CollectionAndAnimationTests
    {
        [Test]
        public void PooledCollectionInjectsNewItemOnlyOnceAndReusesIt()
        {
            var dependency = new CollectionDependency();
            var container = new DiContainer();
            container.BindInstance(dependency).AsSingle();
            var collectionObject = new GameObject("Collection");
            var prefabObject = new GameObject("Item prefab");
            CollectionItem first = null;

            try
            {
                var collection = collectionObject.AddComponent<TestPooledCollection>();
                var prefab = prefabObject.AddComponent<CollectionItem>();
                collection.Configure(prefab, collectionObject.transform);
                container.InjectGameObject(collectionObject);

                first = collection.Create();
                Assert.That(first.Dependency, Is.SameAs(dependency));
                Assert.That(first.InjectionCount, Is.EqualTo(1));
                Assert.That(first.transform.parent, Is.SameAs(collectionObject.transform));
                Assert.That(first.AppearCount, Is.EqualTo(1));

                collection.ReturnToPool(first);
                var reused = collection.Create();

                Assert.That(reused, Is.SameAs(first));
                Assert.That(reused.InjectionCount, Is.EqualTo(1));
                Assert.That(reused.AppearCount, Is.EqualTo(2));
                Assert.That(reused.DisappearCount, Is.EqualTo(1));
            }
            finally
            {
                if (first != null)
                    UnityEngine.Object.DestroyImmediate(first.gameObject);
                UnityEngine.Object.DestroyImmediate(prefabObject);
                UnityEngine.Object.DestroyImmediate(collectionObject);
            }
        }

        [Test]
        public void AnimationUsesDefaultParametersInjectedIntoComponent()
        {
            var parameters = ScriptableObject.CreateInstance<FadeAnimationParameters>();
            var container = new DiContainer();
            container.BindInstance(parameters).AsSingle();
            var animationObject = new GameObject("Animation");

            try
            {
                var animation = animationObject.AddComponent<ParameterProbeAnimation>();
                container.InjectGameObject(animationObject);

                Assert.That(animation.ReadParameters(), Is.SameAs(parameters));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(animationObject);
                UnityEngine.Object.DestroyImmediate(parameters);
            }
        }

        [Test]
        public void AnimationReportsMissingDefaultParameters()
        {
            var animationObject = new GameObject("Animation");

            try
            {
                var animation = animationObject.AddComponent<ParameterProbeAnimation>();

                Assert.That(() => animation.ReadParameters(),
                    Throws.Exception.With.Message.Contains(nameof(FadeAnimationParameters)));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(animationObject);
            }
        }
    }

    public sealed class CollectionDependency
    {
    }

    public sealed class TestPooledCollection : AUiPooledCollection<CollectionItem>
    {
        public void Configure(CollectionItem itemPrefab, Transform parent)
        {
            prefab = itemPrefab;
            collectionContainer = parent;
        }
    }

    public sealed class CollectionItem : AUiCollectionView
    {
        public CollectionDependency Dependency { get; private set; }
        public int InjectionCount { get; private set; }
        public int AppearCount { get; private set; }
        public int DisappearCount { get; private set; }

        [Inject]
        public void Construct(CollectionDependency dependency)
        {
            Dependency = dependency;
            InjectionCount++;
        }

        public override void Appear()
        {
            AppearCount++;
        }

        public override void Disappear()
        {
            DisappearCount++;
        }
    }

    public sealed class ParameterProbeAnimation : AUiAnimation<FadeAnimationParameters>
    {
        public FadeAnimationParameters ReadParameters()
        {
            return AnimationParameters;
        }

        protected override void PrepareToAppear()
        {
        }

        protected override Tween AnimateAppear()
        {
            throw new NotSupportedException();
        }

        protected override Tween AnimateDisappear()
        {
            throw new NotSupportedException();
        }
    }
}

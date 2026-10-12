using System;
using DG.Tweening;
using KoboldUi.Composition;
using KoboldUi.Collections.Base;
using KoboldUi.Collections.Concrete.Impl;
using KoboldUi.Element.Animations;
using KoboldUi.Element.Animations.Parameters.Impl;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace KoboldUi.Tests
{
    public class CollectionAndAnimationTests
    {
        [Test]
        public void PooledCollectionCreatesAndReusesItemsWithoutAdapter()
        {
            var collectionObject = new GameObject("Collection");
            var prefabObject = new GameObject("Item prefab");
            CollectionItem first = null;
            try
            {
                var collection = collectionObject.AddComponent<TestPooledCollection>();
                collection.Configure(prefabObject.AddComponent<CollectionItem>(), collectionObject.transform);
                first = collection.Create();
                Assert.That(first, Is.Not.Null);
                Assert.That(first.InjectionCount, Is.Zero);
                collection.ReturnToPool(first);
                Assert.That(collection.Create(), Is.SameAs(first));
            }
            finally
            {
                if (first != null) UnityEngine.Object.DestroyImmediate(first.gameObject);
                UnityEngine.Object.DestroyImmediate(prefabObject);
                UnityEngine.Object.DestroyImmediate(collectionObject);
            }
        }

        [Test]
        public void PooledCollectionInjectsNewItemOnlyOnceAndReusesIt()
        {
            var dependency = new CollectionDependency();
            var collectionObject = new GameObject("Collection");
            var prefabObject = new GameObject("Item prefab");
            CollectionItem first = null;

            try
            {
                var collection = collectionObject.AddComponent<TestPooledCollection>();
                var prefab = prefabObject.AddComponent<CollectionItem>();
                collection.Configure(prefab, collectionObject.transform);
                new UiActivator(new CollectionAdapter(dependency)).PrepareGameObject(collectionObject);

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
        public void AnimationUsesDefaultsFromAsset()
        {
            var parameters = ScriptableObject.CreateInstance<FadeAnimationParameters>();
            var defaults = ScriptableObject.CreateInstance<UiAnimationDefaults>();
            defaults.Add(parameters);
            var animationObject = new GameObject("Animation");

            try
            {
                var animation = animationObject.AddComponent<ParameterProbeAnimation>();
                new UiActivator(animationDefaults: defaults).PrepareGameObject(animationObject);

                Assert.That(animation.ReadParameters(), Is.SameAs(parameters));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(animationObject);
                UnityEngine.Object.DestroyImmediate(parameters);
                UnityEngine.Object.DestroyImmediate(defaults);
            }
        }

        [Test]
        public void LocalDefaultsOverrideOnlyMatchingProjectParameterTypes()
        {
            var projectFade = ScriptableObject.CreateInstance<FadeAnimationParameters>();
            var projectScale = ScriptableObject.CreateInstance<ScaleAnimationParameters>();
            var localFade = ScriptableObject.CreateInstance<FadeAnimationParameters>();
            var projectDefaults = ScriptableObject.CreateInstance<UiAnimationDefaults>();
            var localDefaults = ScriptableObject.CreateInstance<UiAnimationDefaults>();
            try
            {
                projectDefaults.Add(projectFade);
                projectDefaults.Add(projectScale);
                localDefaults.Add(localFade);
                var serialized = new SerializedObject(localDefaults);
                var parent = serialized.FindProperty("projectDefaults");
                Assert.That(parent, Is.Not.Null);
                parent.objectReferenceValue = projectDefaults;
                serialized.ApplyModifiedPropertiesWithoutUndo();

                Assert.That(localDefaults.TryGet<FadeAnimationParameters>(out var selectedFade), Is.True);
                Assert.That(selectedFade, Is.SameAs(localFade));
                Assert.That(localDefaults.TryGet<ScaleAnimationParameters>(out var selectedScale), Is.True);
                Assert.That(selectedScale, Is.SameAs(projectScale));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(localDefaults);
                UnityEngine.Object.DestroyImmediate(projectDefaults);
                UnityEngine.Object.DestroyImmediate(localFade);
                UnityEngine.Object.DestroyImmediate(projectScale);
                UnityEngine.Object.DestroyImmediate(projectFade);
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

    public sealed class CollectionAdapter : IUiCompositionAdapter
    {
        private readonly CollectionDependency _dependency;
        public CollectionAdapter(CollectionDependency dependency) { _dependency = dependency; }
        public void Inject(object instance)
        {
            if (instance is CollectionItem item) item.Construct(_dependency);
        }
        public T Create<T>(params object[] arguments) where T : class =>
            throw new NotSupportedException(typeof(T).Name);
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

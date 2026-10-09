using System;
using KoboldUi.Composition.Zenject;
using NUnit.Framework;
using Zenject;

namespace KoboldUi.Tests.Zenject
{
    public class ZenjectUiResolverTests
    {
        [Test]
        public void TryResolveReturnsRegisteredInstance()
        {
            var container = new DiContainer();
            var instance = new Service();
            container.BindInstance(instance).AsSingle();
            var resolver = new ZenjectUiResolver(container);
            Assert.That(resolver.TryResolve(typeof(Service), out var resolved), Is.True);
            Assert.That(resolved, Is.SameAs(instance));
        }

        [Test]
        public void TryResolveReturnsFalseForUnboundType()
        {
            var resolver = new ZenjectUiResolver(new DiContainer());
            Assert.That(resolver.TryResolve(typeof(Service), out var resolved), Is.False);
            Assert.That(resolved, Is.Null);
        }

        [Test]
        public void TryResolveUsesParentContainerBindings()
        {
            var parent = new DiContainer();
            var instance = new Service();
            parent.BindInstance(instance).AsSingle();
            var resolver = new ZenjectUiResolver(new DiContainer(parent));
            Assert.That(resolver.TryResolve(typeof(Service), out var resolved), Is.True);
            Assert.That(resolved, Is.SameAs(instance));
        }

        [Test]
        public void ConstructorRejectsNullContainer()
        {
            Assert.Throws<ArgumentNullException>(() => new ZenjectUiResolver(null));
        }

        [Test]
        public void TryResolveRejectsNullType()
        {
            var resolver = new ZenjectUiResolver(new DiContainer());
            Assert.Throws<ArgumentNullException>(() => resolver.TryResolve(null, out _));
        }

        private sealed class Service { }
    }
}

using System;
using KoboldUi.Composition.Manual;
using NUnit.Framework;

namespace KoboldUi.Tests
{
    public class ManualUiResolverTests
    {
        [Test]
        public void AddInstanceResolvesTheRegisteredContract()
        {
            var resolver = new ManualUiResolver();
            var instance = new Service();
            var chained = resolver.AddInstance<IService>(instance);

            Assert.That(chained, Is.SameAs(resolver));
            Assert.That(resolver.TryResolve(typeof(IService), out var resolved), Is.True);
            Assert.That(resolved, Is.SameAs(instance));
        }

        [Test]
        public void AddFactoryCreatesAnInstanceForEachResolution()
        {
            var resolver = new ManualUiResolver();
            var count = 0;
            var chained = resolver.AddFactory<IService>(() => new Service(++count));

            Assert.That(chained, Is.SameAs(resolver));
            Assert.That(resolver.TryResolve(typeof(IService), out var first), Is.True);
            Assert.That(resolver.TryResolve(typeof(IService), out var second), Is.True);
            Assert.That(first, Is.Not.SameAs(second));
            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void MissingContractReturnsFalseAndNull()
        {
            var resolver = new ManualUiResolver();

            Assert.That(resolver.TryResolve(typeof(IService), out var resolved), Is.False);
            Assert.That(resolved, Is.Null);
        }

        [Test]
        public void DuplicateLocalContractIsRejectedWithoutReplacingOriginal()
        {
            var resolver = new ManualUiResolver();
            var original = new Service();
            resolver.AddInstance<IService>(original);

            Assert.Throws<InvalidOperationException>(() => resolver.AddFactory<IService>(() => new Service()));
            Assert.That(resolver.TryResolve(typeof(IService), out var resolved), Is.True);
            Assert.That(resolved, Is.SameAs(original));
        }

        [Test]
        public void ScopeFallsBackToParentAndCanOverrideLocally()
        {
            var parent = new ManualUiResolver();
            var inherited = new Service();
            var local = new Service();
            parent.AddInstance<IService>(inherited);
            var scope = parent.CreateScope();

            Assert.That(scope.TryResolve(typeof(IService), out var beforeOverride), Is.True);
            Assert.That(beforeOverride, Is.SameAs(inherited));
            scope.AddInstance<IService>(local);
            Assert.That(scope.TryResolve(typeof(IService), out var afterOverride), Is.True);
            Assert.That(afterOverride, Is.SameAs(local));
            Assert.That(parent.TryResolve(typeof(IService), out var parentValue), Is.True);
            Assert.That(parentValue, Is.SameAs(inherited));
        }

        [Test]
        public void NullArgumentsAreRejected()
        {
            var resolver = new ManualUiResolver();

            Assert.Throws<ArgumentNullException>(() => resolver.AddInstance<IService>(null));
            Assert.Throws<ArgumentNullException>(() => resolver.AddFactory<IService>(null));
            Assert.Throws<ArgumentNullException>(() => resolver.TryResolve(null, out _));
        }

        [Test]
        public void FactoryReturningNullIsRejected()
        {
            var resolver = new ManualUiResolver();
            resolver.AddFactory<IService>(() => null);

            Assert.Throws<InvalidOperationException>(() => resolver.TryResolve(typeof(IService), out _));
        }

        private interface IService
        {
        }

        private sealed class Service : IService
        {
            public Service(int value = 0)
            {
                Value = value;
            }

            public int Value { get; }
        }
    }
}

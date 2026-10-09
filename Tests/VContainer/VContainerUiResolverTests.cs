using System;
using KoboldUi.Adapters.VContainer;
using NUnit.Framework;
using VContainer;

namespace KoboldUi.Tests.VContainer
{
    public class VContainerUiResolverTests
    {
        private sealed class RegisteredService
        {
        }

        private sealed class MissingService
        {
        }

        [Test]
        public void TryResolveReturnsRegisteredInstance()
        {
            var service = new RegisteredService();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(service);

            using (var container = builder.Build())
            {
                var resolver = new VContainerUiResolver(container);

                Assert.That(resolver.TryResolve(typeof(RegisteredService), out var value), Is.True);
                Assert.That(value, Is.SameAs(service));
            }
        }

        [Test]
        public void TryResolveReturnsFalseForMissingService()
        {
            using (var container = new ContainerBuilder().Build())
            {
                var resolver = new VContainerUiResolver(container);

                Assert.That(resolver.TryResolve(typeof(MissingService), out var value), Is.False);
                Assert.That(value, Is.Null);
            }
        }

        [Test]
        public void ConstructorRejectsNullContainer()
        {
            Assert.Throws<ArgumentNullException>(() => new VContainerUiResolver(null));
        }
    }
}

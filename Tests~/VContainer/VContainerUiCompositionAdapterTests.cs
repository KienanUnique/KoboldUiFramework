using System;
using KoboldUi.Adapters.VContainer;
using NUnit.Framework;
using VContainer;

namespace KoboldUi.Tests.VContainer
{
    public class VContainerUiCompositionAdapterTests
    {
        [Test]
        public void CreatesControllerWithExplicitViewAndContainerDependency()
        {
            var service = new Service();
            var view = new View();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(service);
            using (var container = builder.Build())
            {
                var controller = new VContainerUiCompositionAdapter(container).Create<Controller>(view);
                Assert.That(controller.View, Is.SameAs(view));
                Assert.That(controller.Service, Is.SameAs(service));
            }
        }

        [Test]
        public void InjectsExistingObject()
        {
            var service = new Service();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(service);
            using (var container = builder.Build())
            {
                var target = new Target();
                new VContainerUiCompositionAdapter(container).Inject(target);
                Assert.That(target.Service, Is.SameAs(service));
            }
        }

        [Test]
        public void RejectsNullContainerAndArgument()
        {
            Assert.Throws<ArgumentNullException>(() => new VContainerUiCompositionAdapter(null));
            using (var container = new ContainerBuilder().Build())
            {
                var adapter = new VContainerUiCompositionAdapter(container);
                Assert.Throws<ArgumentException>(() => adapter.Create<Controller>(new object[] { null }));
            }
        }

        public sealed class Service { }
        public sealed class View { }
        public sealed class Controller
        {
            public View View { get; }
            public Service Service { get; }
            public Controller(View view, Service service) { View = view; Service = service; }
        }
        public sealed class Target
        {
            public Service Service { get; private set; }
            [Inject] public void Construct(Service service) { Service = service; }
        }
    }
}

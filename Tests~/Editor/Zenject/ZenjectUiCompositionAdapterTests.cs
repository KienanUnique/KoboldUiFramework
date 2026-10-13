using System;
using KoboldUi.Composition.Zenject;
using NUnit.Framework;
using Zenject;

namespace KoboldUi.Tests.Zenject
{
    public class ZenjectUiCompositionAdapterTests
    {
        [Test]
        public void CreatesControllerWithExplicitViewAndContainerDependency()
        {
            var container = new DiContainer();
            var service = new Service();
            var view = new View();
            container.BindInstance(service).AsSingle();
            var controller = new ZenjectUiCompositionAdapter(container).Create<Controller>(view);
            Assert.That(controller.View, Is.SameAs(view));
            Assert.That(controller.Service, Is.SameAs(service));
        }

        [Test]
        public void IgnoresExplicitViewWhenConstructorDoesNotUseIt()
        {
            var container = new DiContainer();
            var service = new Service();
            container.BindInstance(service).AsSingle();
            var controller = new ZenjectUiCompositionAdapter(container).Create<LegacyController>(new View());
            Assert.That(controller.Service, Is.SameAs(service));
        }

        [Test]
        public void InjectsExistingObjectOnce()
        {
            var container = new DiContainer();
            var service = new Service();
            container.BindInstance(service).AsSingle();
            var target = new Target();
            new ZenjectUiCompositionAdapter(container).Inject(target);
            Assert.That(target.Service, Is.SameAs(service));
            Assert.That(target.Calls, Is.EqualTo(1));
        }

        [Test]
        public void RejectsNullContainerAndTarget()
        {
            Assert.Throws<ArgumentNullException>(() => new ZenjectUiCompositionAdapter(null));
            Assert.Throws<ArgumentNullException>(() => new ZenjectUiCompositionAdapter(new DiContainer()).Inject(null));
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
            public int Calls { get; private set; }
            [Inject] public void Construct(Service service) { Service = service; Calls++; }
        }
        public sealed class LegacyController
        {
            public Service Service { get; }
            public LegacyController(Service service) { Service = service; }
        }
    }
}

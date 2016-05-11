using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace yocto.tests
{
    [TestClass]
    public class ChildContainerTests
    {
        [TestMethod]
        public void ChildContainer_CreateChild()
        {
            var c = Container.Root.GetChildContainer();
        }

        [TestMethod]
        public void ChildContainer_RegisterInParentResolveInChild()
        {
            Container.Root.Register<IAnimal, Dog>().AsMultiInstance();
            Container.Root.Register<IPerson, Person>().AsMultiInstance();

            var c = Container.Root.GetChildContainer();
            c.Resolve<IPerson>();
        }

        [TestMethod]
        public void ChildContainer_RegisterInChild()
        {
            var c = Container.Root.GetChildContainer();
            c.Register<IAnimal, Cat>();
        }

        [TestMethod]
        public void ChildContainer_DisposeOfSingletonResource()
        {
            var c = Container.Root.GetChildContainer();
            c.Register<DisposableResource, DisposableResource>().AsSingleton();
            c.Resolve<DisposableResource>();
            c.Dispose();
            c.Dispose();
        }

        [TestMethod]
        public void ChildContainer_TryResolveSuccess()
        {
            var c = Container.Root.GetChildContainer();

            Container.Root.Register<IAnimal, Dog>().AsMultiInstance();

            IAnimal animal;

            Assert.IsTrue(c.TryResolve(out animal));
        }

        [TestMethod]
        public void ChildContainer_TryResolveFailure()
        {
            var c = Container.Root.GetChildContainer();

            IUnknownParameter unknown;

            Assert.IsTrue(!c.TryResolve(out unknown));
        }
    }
}

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
            var c = Application.Current.GetChildContainer();
        }

        [TestMethod]
        public void ChildContainer_RegisterInParentResolveInChild()
        {
            Application.Current.Register<IAnimal, Dog>().AsMultiInstance();
            Application.Current.Register<IPerson, Person>().AsMultiInstance();

            var c = Application.Current.GetChildContainer();
            c.Resolve<IPerson>();
        }

        [TestMethod]
        public void ChildContainer_RegisterInChild()
        {
            var c = Application.Current.GetChildContainer();
            c.Register<IAnimal, Cat>();
        }

        [TestMethod]
        public void ChildContainer_DisposeOfSingletonResource()
        {
            var c = Application.Current.GetChildContainer();
            c.Register<DisposableResource, DisposableResource>().AsSingleton();
            c.Resolve<DisposableResource>();
            c.Dispose();
            c.Dispose();
        }

        [TestMethod]
        public void ChildContainer_TryResolveSuccess()
        {
            var c = Application.Current.GetChildContainer();

            Application.Current.Register<IAnimal, Dog>().AsMultiInstance();

            IAnimal animal;

            Assert.IsTrue(c.TryResolve(out animal));
        }

        [TestMethod]
        public void ChildContainer_TryResolveFailure()
        {
            var c = Application.Current.GetChildContainer();

            IUnknownParameter unknown;

            Assert.IsTrue(!c.TryResolve(out unknown));
        }
    }
}

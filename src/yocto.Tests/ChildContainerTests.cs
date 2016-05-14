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
            Application.Current.Register<IAnimal, Dog>().AsMultiple();
            Application.Current.Register<IPerson, Person>().AsMultiple();

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

            Application.Current.Register<IAnimal, Dog>().AsMultiple();

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

        [TestMethod]
        public void ChildContainer_Dispose()
        {
            var c = Application.Current.GetChildContainer();
            c.Register<IDisposable, DisposableResource>();
            c.Resolve<IDisposable>();
            c.Dispose();
            c.Dispose();

            var c1 = Application.Current.GetChildContainer();
            c1.RegisterSingleton<IDisposable, DisposableResource>();
            c1.Resolve<IDisposable>();
            c1.Dispose();
            c1.Dispose();

            var c2 = Application.Current.GetChildContainer();
            c2.RegisterPerThread<IDisposable, DisposableResource>();
            c2.Resolve<IDisposable>();
            c2.Dispose();
            c2.Dispose();
        }
    }
}

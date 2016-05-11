using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace yocto.tests
{
    [TestClass]
    public class ResolveTests
    {
        [TestMethod, ExpectedException(typeof(Exception))]
        public void Resolve_CannotResolve()
        {
            Container.Root.Resolve<IUnknownParameter>();
        }

        [TestMethod]
        public void Resolve_CanCanResolve()
        {
            Container.Root.Register<IAnimal, Dog>().AsMultiInstance();

            Assert.IsTrue(Container.Root.CanResolve<IAnimal>());
        }

        [TestMethod]
        public void Resolve_CannotCanResolve()
        {
            Assert.IsFalse(Container.Root.CanResolve<IUnknownParameter>());
        }

        [TestMethod]
        public void Resolve_CanCanResolveFromParent()
        {
            Container.Root.Register<IAnimal, Dog>().AsMultiInstance();
            var c = Container.Root.GetChildContainer();

            Assert.IsTrue(c.CanResolve<IAnimal>());

            c.Dispose();
        }

        [TestMethod]
        public void Resolve_TryResolveSuccess()
        {
            var c = Container.Root;
            
            c.Register<IAnimal, Dog>().AsMultiInstance();

            IAnimal animal;

            Assert.IsTrue(c.TryResolve(out animal));
        }

        [TestMethod]
        public void Resolve_TryResolveFailure()
        {
            var c = Container.Root;
            
            IUnknownParameter unknown;

            Assert.IsTrue(!c.TryResolve(out unknown));
        }
    }
}

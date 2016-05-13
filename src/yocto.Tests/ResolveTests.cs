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
            Application.Current.Resolve<IUnknownParameter>();
        }

        [TestMethod]
        public void Resolve_CanCanResolve()
        {
            Application.Current.Register<IAnimal, Dog>().AsMultiInstance();

            Assert.IsTrue(Application.Current.CanResolve<IAnimal>());
        }

        [TestMethod]
        public void Resolve_CannotCanResolve()
        {
            Assert.IsFalse(Application.Current.CanResolve<IUnknownParameter>());
        }

        [TestMethod]
        public void Resolve_CanCanResolveFromParent()
        {
            Application.Current.Register<IAnimal, Dog>().AsMultiInstance();
            var c = Application.Current.GetChildContainer();

            Assert.IsTrue(c.CanResolve<IAnimal>());

            c.Dispose();
        }

        [TestMethod]
        public void Resolve_TryResolveSuccess()
        {
            var c = Application.Current;
            
            c.Register<IAnimal, Dog>().AsMultiInstance();

            IAnimal animal;

            Assert.IsTrue(c.TryResolve(out animal));
        }

        [TestMethod]
        public void Resolve_TryResolveFailure()
        {
            var c = Application.Current;
            
            IUnknownParameter unknown;

            Assert.IsTrue(!c.TryResolve(out unknown));
        }
    }
}

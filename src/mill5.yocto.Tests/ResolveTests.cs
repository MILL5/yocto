using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mill5.yocto.Tests
{
    [TestClass]
    public class ResolveTests
    {
        [TestMethod, ExpectedException(typeof(Exception))]
        public void CannotResolve()
        {
            Container.Root.Resolve<IUnknownParameter>();
        }

        [TestMethod]
        public void CanCanResolve()
        {
            Container.Root.Register<IAnimal, Dog>();

            Assert.IsTrue(Container.Root.CanResolve<IAnimal>());
        }

        [TestMethod]
        public void CannotCanResolve()
        {
            Assert.IsFalse(Container.Root.CanResolve<IUnknownParameter>());
        }

        [TestMethod]
        public void CanCanResolveFromParent()
        {
            Container.Root.Register<IAnimal, Dog>();
            var c = Container.Root.CreateChild();

            Assert.IsTrue(c.CanResolve<IAnimal>());

            c.Dispose();
        }
    }
}

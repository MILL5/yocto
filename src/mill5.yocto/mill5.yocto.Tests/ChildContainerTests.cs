using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mill5.yocto.Tests
{
    [TestClass]
    public class ChildContainerTests
    {
        [TestMethod]
        public void CreateChild()
        {
            var c = Container.Root.CreateChild();
        }

        [TestMethod]
        public void RegisterInParentResolveInChild()
        {
            Container.Root.Register<IAnimal, Dog>();
            Container.Root.Register<IPerson, Person>();

            var c = Container.Root.CreateChild();
            c.Resolve<IPerson>();
        }

        [TestMethod]
        public void RegisterInChild()
        {
            var c = Container.Root.CreateChild();
            c.Register<IAnimal, Cat>();
        }

        [TestMethod]
        public void DisposeThis()
        {
            var c = Container.Root.CreateChild();
            c.Register<DisposableResource, DisposableResource>(Lifetime.Singleton);
            c.Resolve<DisposableResource>();
            (c as IDisposable)?.Dispose();
            (c as IDisposable)?.Dispose();
        }
    }
}

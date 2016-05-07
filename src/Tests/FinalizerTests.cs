using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mill5.yocto.Tests
{
    [TestClass]
    public class FinalizerTests
    {
        [TestMethod]
        public void ChildContainer()
        {
            var c = Container.Root.CreateChild();
            c.Register<DisposableResource, DisposableResource>(Lifetime.Singleton);
            c.Resolve<DisposableResource>();
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace yocto.Tests
{
    [TestClass]
    public class FinalizerTests
    {
        [TestMethod]
        public void Finalizer_ChildContainer()
        {
            var c = Container.Root.GetChildContainer();
            c.Register<DisposableResource, DisposableResource>().AsSingleton();
            c.Resolve<DisposableResource>();
        }
    }
}

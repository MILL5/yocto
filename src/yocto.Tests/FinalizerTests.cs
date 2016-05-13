using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace yocto.tests
{
    [TestClass]
    public class FinalizerTests
    {
        [TestMethod]
        public void Finalizer_ChildContainer()
        {
            var c = Application.Current.GetChildContainer();
            c.Register<DisposableResource, DisposableResource>().AsSingleton();
            c.Resolve<DisposableResource>();
        }
    }
}

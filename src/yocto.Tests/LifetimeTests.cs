using System;
using yocto.tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace yocto.tests
{
    [TestClass]
    public class LifetimeTests
    {
        [TestMethod]
        public void Lifetime_InsertCustomLifetime()
        {
            using (new LoggingLifetimeScope(Instancing.SingletonInstance))
            {
                var r = Container.Root.Register<IAnimal, Dog>();

                r.AsMultiInstance();
                r.AsSingleton();

                r.AsMultiInstance().AsSingleton();
            }
        }
    }
}

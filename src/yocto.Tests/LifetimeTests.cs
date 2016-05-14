using System;
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
                var r = Application.Current.Register<IAnimal, Dog>();

                r.AsMultiInstance();
                r.AsSingleton();

                r.AsMultiInstance().AsSingleton();
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void Lifetime_UnknownLifetime()
        {
            var r = Application.Current.Register<IAnimal, Dog>();

            r.AsUnknownInstance();
        }

        [TestMethod]
        public void Lifetime_CustomLifetime()
        {
            Application.Current.Register<IAnimal, Dog>().AsCustomInstance();

            Application.Current.Resolve<IAnimal>();
        }
    }
}

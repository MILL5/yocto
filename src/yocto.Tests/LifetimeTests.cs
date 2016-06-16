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

                r.AsMultiple();
                r.AsSingleton();

                r.AsMultiple().AsSingleton();
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

        [TestMethod]
        public void Lifetime_Dispose()
        {
            ILifetimeFactory lf;
            IInstanceFactory inf;

            var c = Application.Current.GetChildContainer();

            lf = Lifetimes.GetLifetimeFactory(Instancing.InstancePerThread);
            inf = lf.GetInstanceFactory(c, typeof(DisposableResource3), typeof(DisposableResource3));
            inf.Create<DisposableResource3>();
            inf.Dispose();
            inf.Dispose();


            lf = Lifetimes.GetLifetimeFactory(Instancing.SingletonInstance);
            inf = lf.GetInstanceFactory(c, typeof(DisposableResource3), typeof(DisposableResource3));
            inf.Create<DisposableResource3>();
            inf.Dispose();
            inf.Dispose();

            lf = Lifetimes.GetLifetimeFactory(Instancing.MultiInstance);
            inf = lf.GetInstanceFactory(c, typeof(DisposableResource3), typeof(DisposableResource3));
            inf.Create<DisposableResource3>();
            inf.Dispose();
            inf.Dispose();

            lf = Lifetimes.GetLifetimeFactory(Instancing.PooledInstance);
            inf = lf.GetInstanceFactory(c, typeof(DisposableResource3), typeof(DisposableResource3));
            inf.Create<DisposableResource3>();
            inf.Dispose();
            inf.Dispose();
        }
    }
}

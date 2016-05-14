using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace yocto.tests
{
    [TestClass]
    public class PerThreadTests
    {
        private CountdownEvent _countDown = new CountdownEvent(2);
        private volatile IAnimal _animal1;
        private volatile IAnimal _animal2;

        [TestMethod]
        public void PerThread_InstancePerThread()
        {
            Application.Current.Register<IAnimal, Cat>().AsPerThread();
            
            var t1 = new Thread(Initialize1);
            t1.IsBackground = true;
            t1.Start();

            var t2 = new Thread(Initialize2);
            t2.IsBackground = true;
            t2.Start();

            _countDown.Wait();

            Assert.AreNotEqual(_animal1, _animal2);
        }

        [TestMethod]
        public void PerThread_OneInstanceOnSameThread()
        {
            Application.Current.Register<IAnimal, Cat>().AsPerThread();

            _animal1 = Application.Current.Resolve<IAnimal>();
            _animal2 = Application.Current.Resolve<IAnimal>();

            Assert.AreEqual(_animal1, _animal2);
        }

        [TestMethod]
        public void PerThread_RegisterPerThread()
        {
            Application.Current.RegisterPerThread<IAnimal, Cat>();

            _animal1 = Application.Current.Resolve<IAnimal>();
            _animal2 = Application.Current.Resolve<IAnimal>();

            Assert.AreEqual(_animal1, _animal2);
        }

        private void Initialize1()
        {
            _animal1 = Application.Current.Resolve<IAnimal>();
            _countDown.Signal();
        }

        private void Initialize2()
        {
            _animal2 = Application.Current.Resolve<IAnimal>();
            _countDown.Signal();
        }
    }
}

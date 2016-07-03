using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace yocto.tests
{
    [TestClass]
    public class FinalizerTests
    {
        private AutoResetEvent _wait = new AutoResetEvent(false);

        private volatile Exception _ex;
        
        [TestMethod]
        public void Finalizer_ChildContainer()
        {
            var t = new Thread(Initialize);
            t.IsBackground = true;
            t.Start();

            _wait.WaitOne();

            GC.Collect();

            if (_ex != null)
                throw _ex;
        }

        [TestMethod]
        public void Finalizer_ChildContainerWithFactory()
        {
            var t = new Thread(InitializeWithFactory);
            t.IsBackground = true;
            t.Start();

            _wait.WaitOne();

            GC.Collect();

            if (_ex != null)
                throw _ex;
        }

        private void Initialize()
        {
            try
            {
                var c = Application.Current.GetChildContainer();

                c.Register<DisposableResource, DisposableResource>().AsSingleton();
                c.Register<DisposableResource1, DisposableResource1>().AsPerThread();
                c.Register<DisposableResource2, DisposableResource2>().AsMultiple();

                c.Resolve<DisposableResource>();
                c.Resolve<DisposableResource1>();
                c.Resolve<DisposableResource2>();
                c = null;

                throw new Exception("DO NOTHING");
            }
            catch (Exception ex)
            {
                if (ex.Message != "DO NOTHING")
                    _ex = ex;
            }
            finally
            {
                _wait.Set();
            }
        }

        private void InitializeWithFactory()
        {
            try
            {
                var c = Application.Current.GetChildContainer();

                c.Register(() => new DisposableResource()).AsSingleton();
                c.Register(() => new DisposableResource1()).AsPerThread();
                c.Register(() => new DisposableResource2()).AsMultiple();

                c.Resolve<DisposableResource>();
                c.Resolve<DisposableResource1>();
                c.Resolve<DisposableResource2>();
                c = null;

                throw new Exception("DO NOTHING");
            }
            catch (Exception ex)
            {
                if (ex.Message != "DO NOTHING")
                    _ex = ex;
            }
            finally
            {
                _wait.Set();
            }
        }
    }
}

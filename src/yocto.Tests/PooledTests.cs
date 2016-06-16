using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using yocto.tests.Mocks;

namespace yocto.tests
{
    [TestClass]
    public class PooledTests
    {
        [TestMethod]
        public void AsPooled_CheckPooling()
        {
            Pooled.Reset();

            var c = Application.Current;
            c.Register<IPooled, Pooled>().AsPooled();

            var p1 = c.Resolve<IPooled>();
            var p2 = c.Resolve<IPooled>();
            var p3 = c.Resolve<IPooled>();
            var p4 = c.Resolve<IPooled>();
            var p5 = c.Resolve<IPooled>();
            var p6 = c.Resolve<IPooled>();
            var p7 = c.Resolve<IPooled>();
            var p8 = c.Resolve<IPooled>();
            var p9 = c.Resolve<IPooled>();

            Assert.IsTrue(p9.Count == 8, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p2, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p3, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p4, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p5, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p6, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p7, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p8, "Pooled instancing is broken.");
            Assert.IsTrue(p1 == p9, "Pooled instancing is broken.");
        }

        [TestMethod]
        public void AsPooled_CheckPoolingWithPoolSize()
        {
            Pooled.Reset();

            var c = Application.Current;
            c.Register<IPooled, Pooled>().AsPooled(4);

            var p1 = c.Resolve<IPooled>();
            var p2 = c.Resolve<IPooled>();
            var p3 = c.Resolve<IPooled>();
            var p4 = c.Resolve<IPooled>();
            var p5 = c.Resolve<IPooled>();
            var p6 = c.Resolve<IPooled>();
            var p7 = c.Resolve<IPooled>();
            var p8 = c.Resolve<IPooled>();
            var p9 = c.Resolve<IPooled>();

            Assert.IsTrue(p9.Count == 4, $"Pooled instancing is broken. Count={p9.Count} [1]");
            Assert.IsTrue(p1 != p2, "Pooled instancing is broken. [2]");
            Assert.IsTrue(p1 != p3, "Pooled instancing is broken. [3]");
            Assert.IsTrue(p1 != p4, "Pooled instancing is broken. [4]");
            Assert.IsTrue(p1 == p5, "Pooled instancing is broken. [5]");
            Assert.IsTrue(p2 == p6, "Pooled instancing is broken. [6]");
            Assert.IsTrue(p3 == p7, "Pooled instancing is broken. [7]");
            Assert.IsTrue(p4 == p8, "Pooled instancing is broken. [8]");
            Assert.IsTrue(p1 == p9, "Pooled instancing is broken. [9]");
        }

        [TestMethod]
        public void RegisterPooled_CheckPooling()
        {
            Pooled.Reset();

            var c = Application.Current;
            c.RegisterPooled<IPooled, Pooled>();

            var p1 = c.Resolve<IPooled>();
            var p2 = c.Resolve<IPooled>();
            var p3 = c.Resolve<IPooled>();
            var p4 = c.Resolve<IPooled>();
            var p5 = c.Resolve<IPooled>();
            var p6 = c.Resolve<IPooled>();
            var p7 = c.Resolve<IPooled>();
            var p8 = c.Resolve<IPooled>();
            var p9 = c.Resolve<IPooled>();

            Assert.IsTrue(p9.Count == 8, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p2, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p3, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p4, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p5, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p6, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p7, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p8, "Pooled instancing is broken.");
            Assert.IsTrue(p1 == p9, "Pooled instancing is broken.");
        }

        [TestMethod]
        public void RegisterPooled_CheckPoolingWithPoolSize()
        {
            Pooled.Reset();

            var c = Application.Current;
            c.RegisterPooled<IPooled, Pooled>(4);

            var p1 = c.Resolve<IPooled>();
            var p2 = c.Resolve<IPooled>();
            var p3 = c.Resolve<IPooled>();
            var p4 = c.Resolve<IPooled>();
            var p5 = c.Resolve<IPooled>();
            var p6 = c.Resolve<IPooled>();
            var p7 = c.Resolve<IPooled>();
            var p8 = c.Resolve<IPooled>();
            var p9 = c.Resolve<IPooled>();

            Assert.IsTrue(p9.Count == 4, $"Pooled instancing is broken. Count={p9.Count} [1]");
            Assert.IsTrue(p1 != p2, "Pooled instancing is broken. [2]");
            Assert.IsTrue(p1 != p3, "Pooled instancing is broken. [3]");
            Assert.IsTrue(p1 != p4, "Pooled instancing is broken. [4]");
            Assert.IsTrue(p1 == p5, "Pooled instancing is broken. [5]");
            Assert.IsTrue(p2 == p6, "Pooled instancing is broken. [6]");
            Assert.IsTrue(p3 == p7, "Pooled instancing is broken. [7]");
            Assert.IsTrue(p4 == p8, "Pooled instancing is broken. [8]");
            Assert.IsTrue(p1 == p9, "Pooled instancing is broken. [9]");
        }

        [TestMethod]
        public void RegisterPooled_Stress()
        {
            Pooled.Reset();

            var c = Application.Current;
            c.RegisterPooled<IPooled, Pooled>(4);

            c.Resolve<IPooled>();
            c.Resolve<IPooled>();
            c.Resolve<IPooled>();
            c.Resolve<IPooled>();

            Parallel.For(0, Environment.ProcessorCount, (x) =>
            {
                for (int i = 0; i <= 1000; i++)
                    c.Resolve<IPooled>();
            });

            c.Resolve<IPooled>();
            c.Resolve<IPooled>();
            c.Resolve<IPooled>();
            c.Resolve<IPooled>();
            c.Resolve<IPooled>();
            c.Resolve<IPooled>();
            c.Resolve<IPooled>();
            c.Resolve<IPooled>();
            c.Resolve<IPooled>();
        }

        [TestMethod]
        public void AsPooled_Disposable()
        {
            Pooled.Reset();

            var c = Application.Current.GetChildContainer();
            c.Register<IDisposable, DisposableResource>().AsPooled();

            var p1 = c.Resolve<IDisposable>();
            var p2 = c.Resolve<IDisposable>();
            var p3 = c.Resolve<IDisposable>();
            var p4 = c.Resolve<IDisposable>();
            var p5 = c.Resolve<IDisposable>();
            var p6 = c.Resolve<IDisposable>();
            var p7 = c.Resolve<IDisposable>();
            var p8 = c.Resolve<IDisposable>();
            var p9 = c.Resolve<IDisposable>();

            Assert.IsTrue(p1 != p2, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p3, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p4, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p5, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p6, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p7, "Pooled instancing is broken.");
            Assert.IsTrue(p1 != p8, "Pooled instancing is broken.");
            Assert.IsTrue(p1 == p9, "Pooled instancing is broken.");

            c.Dispose();
            c.Dispose();
        }
    }
}


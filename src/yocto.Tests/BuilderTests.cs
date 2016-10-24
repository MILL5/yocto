using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace yocto.tests
{
    [TestClass]
    public class BuilderTests
    {
        [TestMethod]
        public void ContainerBuilder_Create()
        {
            var builder = new ContainerBuilder();
            builder.Register<IAnimal, Cat>();
            builder.Build();

            var d = builder as IDisposable;
            d?.Dispose();
        }

        [TestMethod]
        public void ContainerBuilder_Finalize()
        {
            var builder = new ContainerBuilder();
            builder.Register<IAnimal, Cat>();
            builder.Build();

            var d = builder as IDisposable;
            d?.Dispose();
        }

        [TestMethod]
        public void ContainerBuilder_CreateWithFunc()
        {
            var builder = new ContainerBuilder();
            builder.Register<IAnimal>(() => new Cat());
            builder.Build();

            builder = null;
            GC.Collect();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ContainerBuilder_GetContainerAfterBuild()
        {
            var builder = new ContainerBuilder();
            builder.Register<IAnimal, Cat>();
            builder.Build();

            builder.Build();
        }

    }
}

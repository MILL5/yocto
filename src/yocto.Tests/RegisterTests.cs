using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace yocto.Tests
{
    [TestClass]
    public class RegisterTests
    {
        [TestMethod]
        public void Register_MultiInstance()
        {
            Container.Root.Register<IAnimal, Dog>().AsMultiInstance();

            Container.Root.Resolve<IAnimal>();
        }

        [TestMethod]
        public void Register_Singleton()
        {
            Container.Root.Register<IAnimal, Dog>().AsSingleton();

            Container.Root.Resolve<IAnimal>();
        }

        [TestMethod]
        public void Register_CheckMultiInstance()
        {
            Container.Root.Register<IAnimal, Dog>().AsMultiInstance();

            var dog1 = Container.Root.Resolve<IAnimal>();
            var dog2 = Container.Root.Resolve<IAnimal>();

            Assert.IsTrue(!dog1.Equals(dog2), "Multiinstancing is broken.");
        }

        [TestMethod]
        public void Register_CheckSingleton()
        {
            Container.Root.Register<IAnimal, Dog>().AsSingleton();

            var dog1 = Container.Root.Resolve<IAnimal>();
            var dog2 = Container.Root.Resolve<IAnimal>();

            Assert.IsTrue(dog1.Equals(dog2), "Singleton instancing is broken.");
        }

        [TestMethod]
        public void Register_CheckRegisterSingleton()
        {
            Container.Root.RegisterSingleton<IAnimal, Dog>();

            var dog1 = Container.Root.Resolve<IAnimal>();
            var dog2 = Container.Root.Resolve<IAnimal>();

            Assert.IsTrue(dog1.Equals(dog2), "Singleton instancing is broken.");
        }

        [TestMethod]
        public void Register_TwiceAsMultiInstance()
        {
            Container.Root.Register<IAnimal, Dog>().AsMultiInstance();
            Container.Root.Register<IAnimal, Dog>().AsMultiInstance();
        }


        [TestMethod]
        public void Register_TwiceAsSingleton()
        {
            Container.Root.Register<IAnimal, Dog>().AsSingleton();
            Container.Root.Register<IAnimal, Dog>().AsSingleton();
        }

        [TestMethod]
        public void Register_TwiceAsRegisterSingleton()
        {
            Container.Root.RegisterSingleton<IAnimal, Dog>();
            Container.Root.RegisterSingleton<IAnimal, Dog>();
        }


        [TestMethod]
        public void Register_TwiceRegister()
        {
            Container.Root.RegisterSingleton<IAnimal, Dog>();
            Container.Root.Register<IAnimal, Dog>();
        }

        [TestMethod, ExpectedException(typeof(Exception))]
        public void Register_MultipleConstructors()
        {
            var mc1 = new MultipleConstructors();
            var mc2 = new MultipleConstructors(new Cat());
            Container.Root.Register<IAnimal, Dog>().AsSingleton();
            Container.Root.Register<MultipleConstructors, MultipleConstructors>().AsMultiInstance();
        }

        [TestMethod, ExpectedException(typeof(Exception))]
        public void Register_PartialParameters()
        {
            new PartialParameters(new Cat(), new UnknownParameter());
            Container.Root.Register<PartialParameters, PartialParameters>().AsSingleton();
        }
    }
}

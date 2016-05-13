using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace yocto.tests
{
    [TestClass]
    public class RegisterTests
    {
        [TestMethod]
        public void Register_MultiInstance()
        {
            Application.Current.Register<IAnimal, Dog>().AsMultiInstance();

            Application.Current.Resolve<IAnimal>();
        }

        [TestMethod]
        public void Register_Singleton()
        {
            Application.Current.Register<IAnimal, Dog>().AsSingleton();

            Application.Current.Resolve<IAnimal>();
        }

        [TestMethod]
        public void Register_CheckMultiInstance()
        {
            Application.Current.Register<IAnimal, Dog>().AsMultiInstance();

            var dog1 = Application.Current.Resolve<IAnimal>();
            var dog2 = Application.Current.Resolve<IAnimal>();

            Assert.IsTrue(!dog1.Equals(dog2), "Multiinstancing is broken.");
        }

        [TestMethod]
        public void Register_CheckSingleton()
        {
            Application.Current.Register<IAnimal, Dog>().AsSingleton();

            var dog1 = Application.Current.Resolve<IAnimal>();
            var dog2 = Application.Current.Resolve<IAnimal>();

            Assert.IsTrue(dog1.Equals(dog2), "Singleton instancing is broken.");
        }

        [TestMethod]
        public void Register_CheckRegisterSingleton()
        {
            Application.Current.RegisterSingleton<IAnimal, Dog>();

            var dog1 = Application.Current.Resolve<IAnimal>();
            var dog2 = Application.Current.Resolve<IAnimal>();

            Assert.IsTrue(dog1.Equals(dog2), "Singleton instancing is broken.");
        }

        [TestMethod]
        public void Register_TwiceAsMultiInstance()
        {
            Application.Current.Register<IAnimal, Dog>().AsMultiInstance();
            Application.Current.Register<IAnimal, Dog>().AsMultiInstance();
        }


        [TestMethod]
        public void Register_TwiceAsSingleton()
        {
            Application.Current.Register<IAnimal, Dog>().AsSingleton();
            Application.Current.Register<IAnimal, Dog>().AsSingleton();
        }

        [TestMethod]
        public void Register_TwiceAsRegisterSingleton()
        {
            Application.Current.RegisterSingleton<IAnimal, Dog>();
            Application.Current.RegisterSingleton<IAnimal, Dog>();
        }


        [TestMethod]
        public void Register_TwiceRegister()
        {
            Application.Current.RegisterSingleton<IAnimal, Dog>();
            Application.Current.Register<IAnimal, Dog>();
        }

        [TestMethod, ExpectedException(typeof(Exception))]
        public void Register_MultipleConstructors()
        {
            var mc1 = new MultipleConstructors();
            var mc2 = new MultipleConstructors(new Cat());
            Application.Current.Register<IAnimal, Dog>().AsSingleton();
            Application.Current.Register<MultipleConstructors, MultipleConstructors>().AsMultiInstance();
        }

        [TestMethod, ExpectedException(typeof(Exception))]
        public void Register_PartialParameters()
        {
            new PartialParameters(new Cat(), new UnknownParameter());
            Application.Current.Register<PartialParameters, PartialParameters>().AsSingleton();
        }
    }
}

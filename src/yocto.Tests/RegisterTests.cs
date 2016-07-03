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
            Application.Current.Register<IAnimal, Dog>().AsMultiple();

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
            Application.Current.Register<IAnimal, Dog>().AsMultiple();

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
        public void Register_CheckRegisterSingletonWithEager()
        {
            Application.Current.RegisterSingleton<IAnimal, Dog>(true);

            var dog1 = Application.Current.Resolve<IAnimal>();
            var dog2 = Application.Current.Resolve<IAnimal>();

            Assert.IsTrue(dog1.Equals(dog2), "Singleton instancing is broken.");
        }


        [TestMethod]
        public void Register_TwiceAsMultiple()
        {
            Application.Current.Register<IAnimal, Dog>().AsMultiple();
            Application.Current.Register<IAnimal, Dog>().AsMultiple();
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
        public void Register_SingletonWithEagerLoad()
        {
            Application.Current.Register<IAnimal, Dog>().AsSingleton(true);
        }

        [TestMethod]
        public void Register_Instance()
        {
            IAnimal dog = new Dog();

            var c = Application.Current;

            c.Register(dog);

            c.Resolve<IAnimal>();
        }

        [TestMethod, ExpectedException(typeof(Exception))]
        public void Register_InstanceResolveInterface()
        {
            Clown clown = new Clown();

            var c = Application.Current.GetChildContainer();
            
            c.Register(clown);

            var d = c.Resolve<IFish>();
        }

        [TestMethod]
        public void Register_InstanceResolveImplementation()
        {
            Dog dog = new Dog();

            var c = Application.Current;

            c.Register(dog);

            var d = c.Resolve<Dog>();
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
            Application.Current.Register<MultipleConstructors, MultipleConstructors>().AsMultiple();
        }

        [TestMethod]
        public void Register_MultipleConstructors2()
        {
            var mc = new MultipleConstructors2();
            Application.Current.Register<MultipleConstructors2, MultipleConstructors2>().AsMultiple();
        }

        [TestMethod, ExpectedException(typeof(Exception))]
        public void Register_PartialParameters()
        {
            new PartialParameters(new Cat(), new UnknownParameter());
            Application.Current.Register<PartialParameters, PartialParameters>().AsSingleton();
        }
    }
}

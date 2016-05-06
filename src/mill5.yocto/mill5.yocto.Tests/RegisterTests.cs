using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mill5.yocto.Tests
{
    [TestClass]
    public class RegisterTests
    {
        [TestMethod]
        public void RegisterDefault()
        {
            Container.Root.Register<IAnimal, Dog>();

            Container.Root.Resolve<IAnimal>();
        }

        [TestMethod]
        public void RegisterMultiInstance()
        {
            Container.Root.Register<IAnimal, Dog>(Lifetime.MultiInstance);

            Container.Root.Resolve<IAnimal>();
        }

        [TestMethod]
        public void RegisterSingleton()
        {
            Container.Root.Register<IAnimal, Dog>(Lifetime.Singleton);

            Container.Root.Resolve<IAnimal>();
        }

        [TestMethod]
        public void CheckMultiInstance()
        {
            Container.Root.Register<IAnimal, Dog>(Lifetime.MultiInstance);

            var dog1 = Container.Root.Resolve<IAnimal>();
            var dog2 = Container.Root.Resolve<IAnimal>();

            if (dog1.Equals(dog2))
                throw new Exception("Multiinstancing is broken.");
        }

        [TestMethod]
        public void CheckSingleton()
        {
            Container.Root.Register<IAnimal, Dog>(Lifetime.Singleton);

            var dog1 = Container.Root.Resolve<IAnimal>();
            var dog2 = Container.Root.Resolve<IAnimal>();

            if (!dog1.Equals(dog2))
                throw new Exception("Singleton instancing is broken.");
        }


        [TestMethod]
        public void RegisterTwiceAsMultiInstance()
        {
            Container.Root.Register<IAnimal, Dog>(Lifetime.MultiInstance);
            Container.Root.Register<IAnimal, Dog>(Lifetime.MultiInstance);
        }


        [TestMethod]
        public void RegisterTwiceAsSingleton()
        {
            Container.Root.Register<IAnimal, Dog>(Lifetime.Singleton);
            Container.Root.Register<IAnimal, Dog>(Lifetime.Singleton);
        }

        [TestMethod, ExpectedException(typeof(Exception))]
        public void MultipleConstructors()
        {
            Container.Root.Register<IAnimal, Dog>(Lifetime.Singleton);
            Container.Root.Register<MultipleConstructors, MultipleConstructors>();
        }

        [TestMethod, ExpectedException(typeof(Exception))]
        public void PartialParameters()
        {
            Container.Root.Register<IAnimal, Dog>(Lifetime.Singleton);
            Container.Root.Register<PartialParameters, PartialParameters>();
        }
    }
}

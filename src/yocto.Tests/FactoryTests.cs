using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace yocto.tests
{
    [TestClass]
    public class FactoryTests
    {
        [TestMethod]
        public void Factory_Func()
        {
            int count = 0;

            Func<IAnimal> animalFactory = new Func<IAnimal>(() =>
            {
                count++;
                return new Dog();
            });

            Application.Current.Register(animalFactory);

            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();

            Assert.IsTrue(count == 2);
        }

        [TestMethod]
        public void Factory_FuncAsSingleton()
        {
            int count = 0;

            Func<IAnimal> animalFactory = new Func<IAnimal>(() =>
            {
                count++;
                return new Cat();
            });

            Application.Current.Register(animalFactory).AsSingleton();

            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();

            Assert.IsTrue(count == 1);
        }

        [TestMethod]
        public void Factory_FuncPerThread()
        {
            int count = 0;

            Func<IAnimal> animalFactory = new Func<IAnimal>(() =>
            {
                count++;
                return new Cat();
            });

            Application.Current.RegisterPerThread(animalFactory);

            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();

            Assert.IsTrue(count == 1);
        }


        [TestMethod]
        public void Factory_FuncPooled()
        {
            int count = 0;

            Func<IAnimal> animalFactory = new Func<IAnimal>(() =>
            {
                count++;
                return new Cat();
            });

            Application.Current.RegisterPooled(animalFactory);

            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();

            Assert.IsTrue(count == 8);
        }

        [TestMethod]
        public void Factory_FuncPooledWithPoolSize()
        {
            const int poolSize = 4;

            int count = 0;

            Func<IAnimal> animalFactory = new Func<IAnimal>(() =>
            {
                count++;
                return new Cat();
            });

            Application.Current.RegisterPooled(animalFactory, poolSize);

            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();

            Assert.IsTrue(count == poolSize);
        }

        [TestMethod]
        public void Factory_FuncSingleton()
        {
            int count = 0;

            Func<IAnimal> animalFactory = new Func<IAnimal>(() =>
            {
                count++;
                return new Cat();
            });

            Application.Current.RegisterSingleton(animalFactory);

            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();

            Assert.IsTrue(count == 1);
        }

        [TestMethod]
        public void Factory_FuncSingletonWithEager()
        {
            int count = 0;

            Func<IAnimal> animalFactory = new Func<IAnimal>(() =>
            {
                count++;
                return new Cat();
            });

            Application.Current.RegisterSingleton(animalFactory, true);

            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();
            Application.Current.Resolve<IAnimal>();

            Assert.IsTrue(count == 1);
        }
    }
}

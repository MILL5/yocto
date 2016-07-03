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
            Func<IAnimal> animalFactory = () => new Dog();

            Application.Current.Register<IAnimal, Dog>(animalFactory);
        }
    }
}

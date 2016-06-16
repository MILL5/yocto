using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace yocto.tests
{
    [TestClass]
    public class EnumerableTests
    {
        [TestMethod]
        public void Enumerable_RegisterEnumerable()
        {
            var c = Application.Current.GetChildContainer();

            c.RegisterEnumerable<IAnimal, Dog>();
            c.RegisterEnumerable<IAnimal, Cat>();
            c.RegisterEnumerable<IAnimal, Monkey>();

            var animals = c.ResolveAll<IAnimal>();

            Assert.IsTrue(animals.Count() == 3);
        }

        [TestMethod]
        public void Enumerable_RegisterAsEnumerable()
        {
            var c = Application.Current.GetChildContainer();

            c.Register<IAnimal, Dog>().AsEnumerable();
            c.Register<IAnimal, Cat>().AsEnumerable();
            c.Register<IAnimal, Monkey>().AsEnumerable();

            var animals = c.ResolveAll<IAnimal>();

            Assert.IsTrue(animals.Count() == 3);
        }

        [TestMethod, ExpectedException(typeof(Exception))]
        public void Enumerable_ResolveAllWithException()
        {
            var c = Application.Current.GetChildContainer();
            c.ResolveAll<IAnimal>();
        }

        [TestMethod, ExpectedException(typeof(Exception))]
        public void Enumerable_ContainerWithException()
        {
            var c = Application.Current.GetChildContainer();
            var r = c.RegisterEnumerable<IAnimal, Dog>();
            c.ResolveAll<IAnimal>();
            c.Dispose();
            c = null;
            GC.Collect();
            r.ResolveImplementation();
        }
    }
}

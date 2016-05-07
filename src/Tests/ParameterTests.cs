using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mill5.yocto.Tests
{
    /// <summary>
    /// Summary description for Parameter
    /// </summary>
    [TestClass]
    public class ParameterTests
    {
        public ParameterTests()
        {
        }

        [TestMethod, ExpectedException(typeof(Exception))]
        public void Parameter_RegisterShouldFailBecauseMissingParameter()
        {
            var catPerson = new CatPerson(new Cat());
            Container.Root.Register<CatPerson, CatPerson>().AsMultiInstance();
        }

        [TestMethod]
        public void Parameter_RegisterWithParameters()
        {
            Container.Root.Register<IAnimal, Dog>().AsMultiInstance();
            Container.Root.Register<IPerson, Person>().AsMultiInstance();
        }
    }
}

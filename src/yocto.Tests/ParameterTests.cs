using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace yocto.tests
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
            new CatPerson(new Cat());
            Application.Current.Register<CatPerson, CatPerson>().AsMultiInstance();
        }

        [TestMethod]
        public void Parameter_RegisterWithParameters()
        {
            Application.Current.Register<IAnimal, Dog>().AsMultiInstance();
            Application.Current.Register<IPerson, Person>().AsMultiInstance();
        }
    }
}

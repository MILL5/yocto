using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mill5.yocto.Tests
{
    [TestClass]
    public class PreconditionTests
    {

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CheckIfNullSuccess()
        {
            Preconditions.CheckIsNotNull("paramName", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void CheckParamName()
        {
            Preconditions.CheckIsNotNull("", null);
        }

        [TestMethod]
        public void CheckSuccess()
        {
            Preconditions.CheckIsNotNull("paramName", new object());
        }
    }
}

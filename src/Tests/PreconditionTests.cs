using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mill5.yocto.Tests
{
    [TestClass]
    public class PreconditionTests
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Precondition_CheckIsNotNullFail()
        {
            Preconditions.CheckIsNotNull("paramName", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Precondition_CheckIsNotNullParamName()
        {
            Preconditions.CheckIsNotNull("", null);
        }

        [TestMethod]
        public void Precondition_CheckIsNotNullSuccess()
        {
            Preconditions.CheckIsNotNull("paramName", new object());
        }
    }
}

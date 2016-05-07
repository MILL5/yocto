using System;
using System.ComponentModel.Design;
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

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Preconditions_CheckIsNotNullEmptyOrWhitespaceEmpty()
        {
            Preconditions.CheckIsNotNullEmptyOrWhitespace("paramName", string.Empty);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Preconditions_CheckIsNotNullEmptyOrWhitespaceNull()
        {
            Preconditions.CheckIsNotNullEmptyOrWhitespace("paramName", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Preconditions_CheckIsNotNullEmptyOrWhitespaceWhitespace()
        {
            Preconditions.CheckIsNotNullEmptyOrWhitespace("paramName", "      ");
        }
    }
}

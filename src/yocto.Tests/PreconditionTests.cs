using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace yocto.tests
{
    [TestClass]
    public class PreconditionTests
    {
        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Precondition_Check()
        {
            Preconditions.CheckIsGreaterThanOrEqual("paramName", 0, 2);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Precondition_CheckIfLengthLessThanOrEqual()
        {
            Preconditions.CheckIfLengthLessThanOrEqual("paramName", new object[] { "hello", 4 }, 1);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Precondition_CheckIfLengthLessThanOrEqualLessThanZero()
        {
            Preconditions.CheckIfLengthLessThanOrEqual("paramName", new object[] { "hello", 4 }, -5);
        }

        [TestMethod]
        public void Precondition_CheckIfLengthLessThanOrEqualWithNull()
        {
            Preconditions.CheckIfLengthLessThanOrEqual("paramName", null, 5);
        }

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

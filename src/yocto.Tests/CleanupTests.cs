using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace yocto.tests
{
    [TestClass]
    public class CleanupTests
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Cleanup_Fail()
        {
            Cleanup.SafeMethod(null);
        }

        [TestMethod]
        public void Cleanup_Success()
        {
            Cleanup.SafeMethod(() => {  });
        }

        [TestMethod]
        public void Cleanup_Exception()
        {
            Cleanup.SafeMethod(() => { throw new Exception(); });
        }

        [TestMethod]
        public void Cleanup_AggregateException()
        {
            Cleanup.SafeMethod(() => { throw new AggregateException(); });
        }
    }
}

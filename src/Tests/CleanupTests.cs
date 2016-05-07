using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mill5.yocto.Tests
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

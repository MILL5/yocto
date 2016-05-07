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
        public void CleanupFail()
        {
            Cleanup.SafeMethod(null);
        }

        [TestMethod]
        public void CleanupSuccess()
        {
            Cleanup.SafeMethod(() => {  });
        }

        [TestMethod]
        public void CleanupException()
        {
            Cleanup.SafeMethod(() => { throw new Exception(); });
        }

        [TestMethod]
        public void CleanupAggregateException()
        {
            Cleanup.SafeMethod(() => { throw new AggregateException(); });
        }
    }
}

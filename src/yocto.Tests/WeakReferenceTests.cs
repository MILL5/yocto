#if NET4
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace yocto.tests
{
    [TestClass]
    public class WeakReferenceTests
    {
        public class CleanMeUp
        {
        }

        [TestMethod]
        public void WeakReference_TargetNull()
        {
            var o = new CleanMeUp();
            
            var wr = new yocto.WeakReference<CleanMeUp>(o);

            o = null;

            GC.Collect();

            bool value = wr.TryGetTarget(out o);

            Assert.IsFalse(value);
        }
    }
}
#endif

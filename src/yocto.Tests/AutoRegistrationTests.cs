using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace yocto.tests
{
    [TestClass]
    public class AutoRegistrationTests
    {
        [TestMethod]
        public void AutoRegistration_Initialize()
        {
            AutoRegistration.Register(typeof(IBird).Assembly);

            Application.Current.Resolve<IBird>();
        }
    }
}

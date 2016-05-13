using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace yocto.tests
{
    [TestClass]
    public class AssemblyRegistrationTests
    {
        [TestMethod]
        public void AssemblyRegistration()
        {
            Cleanup.SafeMethod(() => yocto.tests.AssemblyRegistration.Initialize(null));
            Cleanup.SafeMethod(() => yocto.tests.bad.AssemblyRegistration.Initialize(null));
            Cleanup.SafeMethod(() => yocto.tests.morebad.AssemblyRegistration.Initialize());
        }

        [TestMethod]
        public void AssemblyRegistration_Initialize()
        {
            AutoRegistration.Register(typeof(IBird).Assembly);

            Application.Current.Resolve<IBird>();
        }
    }
}

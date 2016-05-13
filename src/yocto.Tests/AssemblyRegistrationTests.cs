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
            Cleanup.SafeMethod(() => tests.AssemblyRegistration.Initialize(null));
            Cleanup.SafeMethod(() => bad.AssemblyRegistration.Initialize(null));
            Cleanup.SafeMethod(() => morebad.AssemblyRegistration.Initialize(null, null));
            Cleanup.SafeMethod(() => evenmorebad.AssemblyRegistration.Init());
            Cleanup.SafeMethod(() => new yetevenmorebad.AssemblyRegistration().Initialize(null));
        }

        [TestMethod]
        public void AssemblyRegistration_Initialize()
        {
            AutoRegistration.Register(typeof(IBird).Assembly);

            Application.Current.Resolve<IBird>();
        }
    }
}

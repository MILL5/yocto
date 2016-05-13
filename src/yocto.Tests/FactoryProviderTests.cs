using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
namespace yocto.tests
{
    [TestClass]
    public class FactoryProviderTests
    {
        [TestMethod]
        public void FactoryProvider_ResolveParameterInChildFromParent()
        {
            Application.Current.Register<ILizard, Lizard>();

            var c = Application.Current.GetChildContainer();

            c.Register<IPerson, LizardPerson>();
            var person = c.Resolve<IPerson>();
        }

        [TestMethod, ExpectedException(typeof(Exception))]
        public void FactoryProvider_CannotResolveParameterInChildFromParent()
        {
            var c = Application.Current.GetChildContainer();

            c.Register<IPerson, MonkeyPerson>();
        }
    }
}

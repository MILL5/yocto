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

            // Check that we get an exception
            Exception rethrow = null;
            try
            {
                c.Register<IPerson, MonkeyPerson>();
            }
            catch (Exception ex)
            {
                rethrow = ex;
            }

            // Make sure that we hit all lines of code
            // related to this test
            c.Register<IMonkey, Monkey>();
            c.Register<IPerson, MonkeyPerson>();
            c.Resolve<IPerson>();

            if (rethrow != null)
                throw rethrow;
        }
    }
}

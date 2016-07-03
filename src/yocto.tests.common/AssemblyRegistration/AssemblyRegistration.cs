using System;

namespace yocto.tests
{
    public static class AssemblyRegistration
    {
        public static void Initialize(IContainer container)
        {
            container.Register<IBird, Eagle>();
        }
    }
}

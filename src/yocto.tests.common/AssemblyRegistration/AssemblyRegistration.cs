using System;
using static yocto.Preconditions;

namespace yocto.tests
{
    public static class AssemblyRegistration
    {
        public static void Initialize(IContainer container)
        {
            CheckIsNotNull(nameof(container), container);

            container.Register<IBird, Eagle>();
        }
    }
}

using System;
using yocto.tests.common;

namespace yocto.tests.lanretni
{
    public static class AssemblyRegistration
    {
        public static void Initialize(IContainer container)
        {
            container.Register<IInternal, Internal>();
            container.Register<IExternal, External>();
        }
    }
}

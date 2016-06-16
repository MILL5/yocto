using System;

namespace yocto
{
    internal class InstancePerThreadLifetimeFactory : ILifetimeFactory
    {
        public IInstanceFactory GetInstanceFactory(IContainer container, Type interfaceType, Type implementationType, params object[] values)
        {
            return new InstancePerThreadFactory(container, implementationType);
        }
    }
}

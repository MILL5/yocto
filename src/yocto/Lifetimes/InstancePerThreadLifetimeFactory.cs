using System;

namespace yocto
{
    internal class InstancePerThreadLifetimeFactory : ILifetimeFactory
    {
        public IInstanceFactory GetInstanceFactory(IContainer container, Type interfaceType, Type implementationType, Func<object> factory, params object[] values)
        {
            return new InstancePerThreadFactory(container, implementationType, factory);
        }
    }
}

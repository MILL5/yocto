using System;

namespace yocto
{
    internal class MultiInstanceLifetimeFactory : ILifetimeFactory
    {
        public IInstanceFactory GetInstanceFactory(IContainer container, Type interfaceType, Type implementationType, params object[] values)
        {
            return new MultiInstanceFactory(container, implementationType);
        }
    }
}

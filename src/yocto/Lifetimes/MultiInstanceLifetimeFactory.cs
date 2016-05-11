using System;

namespace yocto
{
    internal class MultiInstanceLifetimeFactory : ILifetimeFactory
    {
        public IInstanceFactory GetInstanceFactory(Container container, Type implementationType)
        {
            return new MultiInstanceFactory(container, implementationType);
        }
    }
}

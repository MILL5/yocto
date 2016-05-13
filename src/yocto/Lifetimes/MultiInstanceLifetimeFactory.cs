using System;

namespace yocto
{
    internal class MultiInstanceLifetimeFactory : ILifetimeFactory
    {
        public IInstanceFactory GetInstanceFactory(IContainer container, Type implementationType)
        {
            return new MultiInstanceFactory(container, implementationType);
        }
    }
}

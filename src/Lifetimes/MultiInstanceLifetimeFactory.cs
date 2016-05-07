using System;

namespace mill5.yocto
{
    internal class MultiInstanceLifetimeFactory : ILifetimeFactory
    {
        public IInstanceFactory GetInstanceFactory(Container container, Type implementationType)
        {
            return new MultiInstanceFactory(container, implementationType);
        }
    }
}

using System;

namespace mill5.yocto
{
    internal class SingletonLifetimeFactory : ILifetimeFactory
    {
        public IInstanceFactory GetInstanceFactory(Container container, Type implementationType)
        {
            return new SingletonFactory(container, implementationType);
        }
    }
}

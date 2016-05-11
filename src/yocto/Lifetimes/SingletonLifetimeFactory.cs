using System;

namespace yocto
{
    internal class SingletonLifetimeFactory : ILifetimeFactory
    {
        public IInstanceFactory GetInstanceFactory(Container container, Type implementationType)
        {
            return new SingletonFactory(container, implementationType);
        }
    }
}

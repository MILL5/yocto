using System;

namespace yocto
{
    internal class SingletonLifetimeFactory : ILifetimeFactory
    {
        public IInstanceFactory GetInstanceFactory(IContainer container, Type implementationType)
        {
            return new SingletonFactory(container, implementationType);
        }
    }
}

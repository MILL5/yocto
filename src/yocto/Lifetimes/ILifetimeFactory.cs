using System;

namespace yocto
{
    public interface ILifetimeFactory
    {
        IInstanceFactory GetInstanceFactory(Container container, Type implementationType);
    }
}

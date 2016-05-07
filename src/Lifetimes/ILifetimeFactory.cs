using System;

namespace mill5.yocto
{
    public interface ILifetimeFactory
    {
        IInstanceFactory GetInstanceFactory(Container container, Type implementationType);
    }
}

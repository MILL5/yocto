using System;

namespace yocto
{
    public interface ILifetimeFactory
    {
        IInstanceFactory GetInstanceFactory(IContainer container, Type interfaceType, Type implementationType, params object[] values);
    }
}

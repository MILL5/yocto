using System;

namespace yocto
{
    public interface ILifetimeFactory
    {
        IInstanceFactory GetInstanceFactory(IContainer container, Type interfaceType, Type implementationType, Func<object> factory, params object[] values);
    }
}

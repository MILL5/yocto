using System;

namespace yocto
{
    internal class MultiInstanceLifetimeFactory : ILifetimeFactory
    {
        public IInstanceFactory GetInstanceFactory(IContainer container, Type interfaceType, Type implementationType, Func<object> factory, params object[] values)
        {
            return new MultiInstanceFactory(container, implementationType, factory);
        }
    }
}

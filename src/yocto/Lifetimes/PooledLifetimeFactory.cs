using System;
using static yocto.Preconditions;

namespace yocto
{
    internal class PooledLifetimeFactory : ILifetimeFactory
    {
        public IInstanceFactory GetInstanceFactory(IContainer container, Type interfaceType, Type implementationType, Func<object> factory, params object[] values)
        {
            const int maxNumberOfParams = 1;

            CheckIfLengthLessThanOrEqual(nameof(values), values, maxNumberOfParams);

            if (values.Length == 0)
                return new PooledFactory(container, implementationType, factory);
           
            int poolSize = Convert.ToInt32(values[0]);
            return new PooledFactory(container, implementationType, poolSize, factory);
        }
    }
}

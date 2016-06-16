using System;
using static yocto.Preconditions;

namespace yocto
{
    internal class SingletonLifetimeFactory : ILifetimeFactory
    {
        public IInstanceFactory GetInstanceFactory(IContainer container, Type interfaceType, Type implementationType, params object[] values)
        {
            const int numberOfParams = 1;

            CheckIfLengthLessThanOrEqual(nameof(values), values, numberOfParams);

            bool eagerLoad = false;

            if (values.Length == 1)
                eagerLoad = Convert.ToBoolean(values[0]);
            
            return new SingletonFactory(container, implementationType, eagerLoad);
        }
    }
}

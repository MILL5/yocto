using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace yocto
{
    internal class InstancePerThreadLifetimeFactory : ILifetimeFactory
    {
        public IInstanceFactory GetInstanceFactory(IContainer container, Type implementationType)
        {
            return new InstancePerThreadFactory(container, implementationType);
        }
    }
}

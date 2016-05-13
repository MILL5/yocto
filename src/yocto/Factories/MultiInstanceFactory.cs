using System;
using static yocto.Preconditions;

namespace yocto
{
    internal class MultiInstanceFactory : IInstanceFactory
    {
        private readonly Constructor _constructor;

        public MultiInstanceFactory(IContainer container, Type implementationType)
        {
            CheckIsNotNull(nameof(container), container);
            CheckIsNotNull(nameof(implementationType), implementationType);

            _constructor = new Constructor(container, implementationType);
        }

        public T Create<T>() where T: class
        {
            return _constructor.Create<T>();
        }
    }
}
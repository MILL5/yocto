using System;
using static mill5.yocto.Preconditions;

namespace mill5.yocto
{
    internal class MultiInstanceFactory : IInstanceFactory
    {
        private readonly Constructor _constructor;

        public MultiInstanceFactory(Container container, Type implementationType)
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
using System;
using static yocto.Preconditions;

namespace yocto
{
    internal partial class Container
    {
        private class Registration<T, V> : IRegistration
        {
            private readonly Container _container;

            public Registration(Container container)
            {
                CheckIsNotNull(nameof(container), container);
                _container = container;
            }

            public IRegistration Register(string lifetime)
            {
                CheckIsNotNullEmptyOrWhitespace(nameof(lifetime), lifetime);

                var interfaceType = typeof(T);
                var implementationType = typeof(V);

                _container.CreateInstanceFactory(interfaceType, implementationType, lifetime);

                return this;
            }
        }
    }
}

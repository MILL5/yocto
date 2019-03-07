using System;
using static yocto.Preconditions;

namespace yocto
{
    internal partial class Container
    {
        private class FactoryRegistration<T> : IRegistration
            where T : class
        {
            private readonly WeakReference<IContainer> _container;
            private readonly Func<T> _factory;

            public FactoryRegistration(Container container, Func<T> factory)
            {
                CheckIsNotNull(nameof(container), container);
                CheckIsNotNull(nameof(factory), factory);

                _container = new WeakReference<IContainer>(container);
                _factory = factory;
            }

            public IContainer Container
            {
                get
                {
                    if (!_container.TryGetTarget(out var c))
                        throw new Exception("Could not access container.");

                    return c;
                }
            }

            public Type InterfaceType => typeof(T);

            public Type ImplementationType => typeof(T);

            public void Remove()
            {
                ((Container)Container).Remove<T>();
            }

            public object ResolveImplementation()
            {
                return Container.Resolve<T>();
            }

            public IRegistration RegisterImplementation()
            {
                return Container.Register(_factory);
            }

            public IRegistration Register(string lifetime, params object[] values)
            {
                CheckIsNotNullEmptyOrWhitespace(nameof(lifetime), lifetime);

                ((Container)Container).CreateInstanceFactory(InterfaceType, ImplementationType, lifetime, _factory, values);

                return this;
            }
        }
    }
}
using System;
using static yocto.Preconditions;

namespace yocto
{
    internal partial class Container
    {
        private class Registration<T, V> : IRegistration
            where V : class, T
            where T : class
        {
            private readonly WeakReference<IContainer> _container;

            public Registration(Container container)
            {
                CheckIsNotNull(nameof(container), container);
                _container = new WeakReference<IContainer>(container);
            }

            public IContainer Container
            {
                get
                {
                    IContainer c;

                    if (!_container.TryGetTarget(out c))
                        throw new Exception("Could not access container.");

                    return c;
                }
            }

            public Type InterfaceType => typeof(T);

            public Type ImplementationType => typeof(V);

            public void Remove()
            {
                Container.Remove<T>();    
            }

            public object ResolveImplementation()
            {
                return Container.Resolve<V>();
            }

            public IRegistration RegisterImplementation()
            {
                return Container.Register<V, V>();
            }

            public IRegistration Register(string lifetime, params object[] values)
            {
                CheckIsNotNullEmptyOrWhitespace(nameof(lifetime), lifetime);
                
                ((Container)Container).CreateInstanceFactory(InterfaceType, ImplementationType, lifetime, values);

                return this;
            }
        }
    }
}
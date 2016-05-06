using System;
using System.Collections.Concurrent;
using static mill5.yocto.Preconditions;
// ReSharper disable InconsistentNaming

namespace mill5.yocto
{
    public class Container
    {
        private readonly ConcurrentBag<Container> _childContainers = new ConcurrentBag<Container>(); 
        private readonly ConcurrentDictionary<Type, IInstanceFactory> _factories =
            new ConcurrentDictionary<Type, IInstanceFactory>();

        private readonly Container _parent;

        private bool _disposed;
        
        private Container()
        {
        }

        protected Container(Container parent)
        {
            CheckIsNotNull(nameof(parent), parent);
            _parent = parent;
        }

        ~Container()
        {
            Dispose(false);
        }

        protected void Dispose(bool dispose)
        {
            if (_disposed)
                return;

            Cleanup.SafeMethod(() =>
            {
                foreach (var c in _childContainers)
                {
                    ((IDisposable)c).Dispose();
                }

                foreach (var f in _factories.Values)
                {
                    (f as IDisposable)?.Dispose();
                }
            });

            _disposed = true;

            if (dispose)
                GC.SuppressFinalize(this);
        }

        public static Container Root { get; } = new Container();

        public Container CreateChild()
        {
            var child = new ChildContainer(this);
            _childContainers.Add(child);
            return child;
        }

        public void Register<T,V>(Lifetime lifetime = Lifetime.MultiInstance) where V : T
        {
            var interfaceType = typeof(T);
            var implementationType = typeof(V);

            _factories.AddOrUpdate(interfaceType,
                t => CreateInstanceFactory(implementationType, lifetime),
                (t, of) =>
                {
                    (of as IDisposable)?.Dispose();
                    return CreateInstanceFactory(implementationType, lifetime);
                });
        }

        public T Resolve<T>() where T : class
        {
            IInstanceFactory instanceFactory;

            if (_factories.TryGetValue(typeof (T), out instanceFactory))
            {
                return instanceFactory.Create<T>();
            }

            if (_parent != null)
            {
                return _parent.Resolve<T>();
            }

            throw new Exception("Interface type is not registered.");
        }

        internal bool TryGetFactory(Type interfaceType, out IInstanceFactory instanceFactory)
        {
            return _factories.TryGetValue(interfaceType, out instanceFactory);
        }

        internal bool ContainsFactory(Type interfaceType)
        {
            return _factories.ContainsKey(interfaceType);
        }

        private IInstanceFactory CreateInstanceFactory(Type implementationType, Lifetime lifetime)
        {
            switch (lifetime)
            {
                case Lifetime.MultiInstance:
                    return new MultiInstanceFactory(this, implementationType);
                case Lifetime.Singleton:
                    return new SingletonFactory(this, implementationType);
                default:
                    throw new Exception("Lifetime was not supported.");
            }
        }
    }
}
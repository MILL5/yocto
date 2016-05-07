using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Dynamic;
using System.Linq;
using static mill5.yocto.Preconditions;
// ReSharper disable InconsistentNaming

namespace mill5.yocto
{
    public class Container
    {
        private readonly object _syncLock = new object();
        private readonly List<Container> _children = new List<Container>(); 
   
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
                _parent?.RemoveChild(this);
            
                List<Container> childrenToDispose;

                lock (_syncLock)
                {
                    childrenToDispose = _children.ToList();
                    _children.Clear();
                }

                foreach (var c in childrenToDispose)
                {
                    ((IDisposable)c).Dispose();
                }

                List<IInstanceFactory> factoriesToDispose = _factories.Values.ToList();

                foreach (var f in factoriesToDispose)
                {
                    (f as IDisposable)?.Dispose();
                }

                _factories.Clear();
            });

            _disposed = true;

            if (dispose)
                GC.SuppressFinalize(this);
        }

        public static Container Root { get; } = new Container();

        public ChildContainer CreateChild()
        {
            var child = new ChildContainer(this);

            lock (_syncLock)
            {
                _children.Add(child);
            }

            return child;
        }

        private void RemoveChild(Container child)
        {
            lock (_syncLock)
            {
                _children.Remove(child);
            }
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

        public bool CanResolve<T>()
        {
            bool canResolve = _factories.ContainsKey(typeof (T));

            if ((!canResolve) && (_parent != null))
            {
                return _parent.CanResolve<T>();
            }

            return canResolve;
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
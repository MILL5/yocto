﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

using static yocto.Preconditions;
// ReSharper disable InconsistentNaming

namespace yocto
{
    internal partial class Container : IContainer, IFactoryProvider, IResolveByType
    {
        private readonly object _syncLock = new object();
        private readonly List<Container> _children = new List<Container>();

        private readonly ConcurrentDictionary<Type, IInstanceFactory> _factories =
            new ConcurrentDictionary<Type, IInstanceFactory>();

        protected readonly Container _parent;

        private bool _disposed;
        

        internal Container()
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

                var factoriesToDispose = _factories.Values.ToList();

                foreach (var f in factoriesToDispose)
                {
                    f.Dispose();
                }

                _factories.Clear();
            });

            _disposed = true;

            if (dispose)
                GC.SuppressFinalize(this);
        }

        public IChildContainer GetChildContainer()
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

        public IRegistration Register<T>(T instance) where T : class
        {
            CheckIsNotNull(nameof(instance), instance);

            return new FactoryRegistration<T>(this, () => instance).AsMultiple();
        }

        public IRegistration Register<T>(Func<T> factory) where T : class
        {
            CheckIsNotNull(nameof(factory), factory);

            return new FactoryRegistration<T>(this, factory).AsMultiple();
        }

        public IRegistration Register<T,V>() where V : class, T where T : class
        {
            return new Registration<T, V>(this).AsMultiple();
        }

        private void Remove<T>() where T : class
        {
            var interfaceType = typeof(T);

            if (_factories.TryRemove(interfaceType, out var instanceFactory))
            {
                instanceFactory.Dispose();
            }
        }

        public T Resolve<T>() where T : class
        {
            if (!TryResolve(out T instance))
                throw new Exception("Interface type is not registered.");

            return instance;
        }

        public T Resolve<T>(Type type) where T : class
        {
            CheckIsNotNull(nameof(type), type);

            if (!TryResolve(type, out T instance))
                throw new Exception("Interface type is not registered.");

            return instance;
        }

        public object Resolve(Type type)
        {
            CheckIsNotNull(nameof(type), type);

            if (!TryResolve(type, out object instance))
                throw new Exception("Interface type is not registered.");

            return instance;
        }

        public bool CanResolve<T>() where T : class
        {
            bool canResolve = _factories.ContainsKey(typeof (T));

            if ((!canResolve) && (_parent != null))
            {
                return _parent.CanResolve<T>();
            }

            return canResolve;
        }

        public bool CanResolve(Type type)
        {
            CheckIsNotNull(nameof(type), type);

            bool canResolve = _factories.ContainsKey(type);

            if ((!canResolve) && (_parent != null))
            {
                return _parent.CanResolve(type);
            }

            return canResolve;
        }

        public bool TryGetFactory(Type type, out IInstanceFactory factory)
        {
            CheckIsNotNull(nameof(type), type);

            bool found = _factories.TryGetValue(type, out factory);

            if (!found && _parent != null)
            {
                found = _parent.TryGetFactory(type, out factory);
            }

            return found;
        }

        public bool TryResolve<T>(out T instance) where T : class
        {
            instance = null;

            if (_factories.TryGetValue(typeof(T), out var instanceFactory))
            {
                instance = instanceFactory.Create<T>();
            }
            else if (_parent != null)
            {
                return _parent.TryResolve(out instance);
            }

            return (instance != null);
        }

        public bool TryResolve<T>(Type type, out T instance) where T : class
        {
            instance = null;

            if (_factories.TryGetValue(type, out var instanceFactory))
            {
                instance = instanceFactory.Create<T>();
            }
            else if (_parent != null)
            {
                return _parent.TryResolve(type, out instance);
            }

            return (instance != null);
        }

        private void CreateInstanceFactory(Type interfaceType, Type implementationType, string lifetime, params object[] values)
        {
            CreateInstanceFactory(interfaceType, implementationType, lifetime, null, values);
        }

        private void CreateInstanceFactory(Type interfaceType, Type implementationType, string lifetime, Func<object> factory, params object[] values)
        {
            var lifetimeFactory = Lifetimes.GetLifetimeFactory(lifetime);
            var instanceFactory = lifetimeFactory.GetInstanceFactory(this, interfaceType, implementationType, factory, values);

            _factories.AddOrUpdate(interfaceType, t => instanceFactory,
                (t, of) =>
                {
                    of.Dispose();
                    return instanceFactory;
                });
        }
    }
}
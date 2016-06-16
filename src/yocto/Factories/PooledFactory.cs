using System;
using System.Collections.Generic;
using System.Threading;
using static yocto.Preconditions;

namespace yocto
{
    internal class PooledFactory : IInstanceFactory
    {
        private readonly Constructor _constructor;

        private readonly object _syncLock = new object();
        private readonly List<object> _instances = new List<object>();
        private readonly int _poolSize = Math.Max(Environment.ProcessorCount, 8);
        private int _current = -1;
        private bool _disposed;

        public PooledFactory(IContainer container, Type implementationType)
        {
            CheckIsNotNull(nameof(container), container);
            CheckIsNotNull(nameof(implementationType), implementationType);

            _constructor = new Constructor(container, implementationType);
        }

        public PooledFactory(IContainer container, Type implementationType, int poolSize) : this(container, implementationType)
        {
            const int minimumPoolSize = 2;

            CheckIsGreaterThanOrEqual(nameof(poolSize), poolSize, minimumPoolSize);

            _poolSize = poolSize;
        }

        private int GetNextIndex()
        {
            int index = Interlocked.Increment(ref _current);

            if (index == _poolSize)
                index = Interlocked.Exchange(ref _current, 0);

            int nextIndex = Math.Abs(index) % _poolSize;

            return nextIndex;
        }

        public T Create<T>() where T : class
        {
            lock (_syncLock)
            {
                bool growPool = _instances.Count < _poolSize;

                if (growPool)
                {
                    var pooledInstance = _constructor.Create<T>();
                    _instances.Add(pooledInstance);
                }
            }

            int index = GetNextIndex();

            return (T)_instances[index];
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            // We do not support a finalizer because our owner has one
            Cleanup.SafeMethod(() =>
            {
                foreach (var o in _instances)
                {
                    (o as IDisposable)?.Dispose();
                }
            });

            _disposed = true;
        }
    }
}
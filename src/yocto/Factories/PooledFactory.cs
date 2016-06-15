//using System;
//using System.Collections.Immutable;
//using System.Threading;
//using static yocto.Preconditions;

//namespace yocto
//{
//    internal class PooledFactory : IInstanceFactory, IDisposable
//    {
//        private Constructor _constructor;
//        private bool _disposed;

//        private object _syncLock = new object();
//        private ImmutableList<object> _instances = ImmutableList<object>.Empty;
//        private int _poolSize = Math.Max(Environment.ProcessorCount * 4, 8);
//        private int _current;

//        public PooledFactory(IContainer container, Type implementationType)
//        {
//            CheckIsNotNull(nameof(container), container);
//            CheckIsNotNull(nameof(implementationType), implementationType);

//            _constructor = new Constructor(container, implementationType);
//        }

//        public int GetNextIndex()
//        {
//            int index = Interlocked.Increment(ref _current);

//            if (index == int.MinValue)
//                index = Interlocked.Increment(ref _current);

//            return Math.Abs(index) % _poolSize;
//        }

//        public T Create<T>() where T : class
//        {
//            T pooledInstance;
//            bool growPool = _instances.Count < _poolSize;

//            if (growPool)
//            {
//                lock (_syncLock)
//                {
//                    growPool = _instances.Count < _poolSize;

//                    if (growPool)
//                    {
//                        pooledInstance = _constructor.Create<T>();
//                        _instances = _instances.Add(pooledInstance);
//                    }
//                }
//            }

//            int index = GetNextIndex();

//            return (T)_instances[index];
//        }

//        public void Dispose()
//        {
//            // We do not support a finalizer because our owner has one
//            if (_disposed)
//                return;

//            Cleanup.SafeMethod(() =>
//            {
//                foreach (var o in _instances)
//                {
//                    (o as IDisposable)?.Dispose();
//                }
//            });

//            _disposed = true;
//        }
//    }
//}
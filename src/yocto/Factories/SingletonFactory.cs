using System;
using static yocto.Preconditions;

namespace yocto
{
    internal class SingletonFactory : IInstanceFactory, IDisposable
    {
        private readonly object _instance;
        private bool _disposed;

        public SingletonFactory(IContainer container, Type implementationType)
        {
            CheckIsNotNull(nameof(container), container);
            CheckIsNotNull(nameof(implementationType), implementationType);

            _instance = (new Constructor(container, implementationType)).Create<object>();
        }

        public T Create<T>() where T: class
        {
            return (T)_instance;
        }

        public void Dispose()
        {
            // We do not support a finalizer because our owner has one
            if (_disposed)
                return;

            Cleanup.SafeMethod(() =>
            {
                (_instance as IDisposable)?.Dispose();
            });

            _disposed = true;
        }
    }
}

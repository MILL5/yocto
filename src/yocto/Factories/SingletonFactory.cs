using System;
using static yocto.Preconditions;

namespace yocto
{
    internal class SingletonFactory : IInstanceFactory, IDisposable
    {
        private readonly object _instance;
        private bool _disposed;

        public SingletonFactory(Container container, Type implementationType)
        {
            CheckIsNotNull(nameof(container), container);
            CheckIsNotNull(nameof(implementationType), implementationType);

            _instance = (new Constructor(container, implementationType)).Create<object>();
        }

        ~SingletonFactory()
        {
            Dispose(false);
        }

        public T Create<T>() where T: class
        {
            return (T)_instance;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool dispose)
        {
            if (_disposed)
                return;

            Cleanup.SafeMethod(() =>
            {
                (_instance as IDisposable)?.Dispose();
            });

            _disposed = true;

            if (dispose)
                GC.SuppressFinalize(this);
        }
    }
}

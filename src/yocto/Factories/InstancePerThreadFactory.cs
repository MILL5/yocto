using System;
using System.Threading;
using static yocto.Preconditions;

namespace yocto
{
    internal class InstancePerThreadFactory : IInstanceFactory, IDisposable
    {
        private Constructor _constructor;
        private bool _disposed;

        private ThreadLocal<object> _instance;
        
        public InstancePerThreadFactory(IContainer container, Type implementationType)
        {
            CheckIsNotNull(nameof(container), container);
            CheckIsNotNull(nameof(implementationType), implementationType);

            _constructor = new Constructor(container, implementationType);

            _instance = new ThreadLocal<object>(() =>
            {
                return _constructor.Create<object>();
            }, true);
        }

        ~InstancePerThreadFactory()
        {
            Dispose(false);
        }

        public T Create<T>() where T: class
        {
            return (T)_instance.Value;
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
                foreach (var o in _instance.Values)
                {
                    (o as IDisposable)?.Dispose();
                }
            });

            _disposed = true;

            if (dispose)
                GC.SuppressFinalize(this);
        }
    }
}
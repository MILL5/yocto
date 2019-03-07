using System;
using System.Threading;
using static yocto.Preconditions;

namespace yocto
{
    internal class InstancePerThreadFactory : IInstanceFactory
    {
        private bool _disposed;

        private readonly ThreadLocal<object> _instance;
        
        public InstancePerThreadFactory(IContainer container, Type implementationType, Func<object> factory)
        {
            CheckIsNotNull(nameof(container), container);
            CheckIsNotNull(nameof(implementationType), implementationType);

            var constructor = new Constructor(container, implementationType, factory);

            _instance = new ThreadLocal<object>(() => constructor.Create<object>());
        }

        public T Create<T>() where T: class
        {
            return (T)_instance.Value;
        }

        public void Dispose()
        {
            // We do not support a finalizer because our owner has one
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
        }
    }
}
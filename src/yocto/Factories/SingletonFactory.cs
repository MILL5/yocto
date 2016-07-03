using System;
using static yocto.Preconditions;

namespace yocto
{
    internal class SingletonFactory : IInstanceFactory
    {
        private readonly Lazy<object> _instance;

        private bool _disposed;

        public SingletonFactory(IContainer container, Type implementationType, bool eagerLoad, Func<object> factory)
        {
            CheckIsNotNull(nameof(container), container);
            CheckIsNotNull(nameof(implementationType), implementationType);

            _instance = new Lazy<object>(() => new Constructor(container, implementationType, factory).Create<object>());

            if (eagerLoad)
            {
                var o = _instance.Value;
            }
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
                if (_instance.IsValueCreated)
                {
                    (_instance.Value as IDisposable)?.Dispose();
                }
            });

            _disposed = true;
        }
    }
}

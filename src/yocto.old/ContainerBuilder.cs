using System;
// ReSharper disable InconsistentNaming

namespace yocto
{
    public class ContainerBuilder : IRegisterType, IDisposable
    {
        private readonly object _syncLock = new object();
        private volatile IContainer _container;

        private bool _disposed;

        public ContainerBuilder()
        {
            _container = Application.Current.GetChildContainer();
        }

        ~ContainerBuilder()
        {
            InternalDispose();
        }

        public IRegistration Register<T>(Func<T> factory) where T : class
        {
            return GetContainer().Register(factory);
        }

        public IRegistration Register<T, V>() where V : class, T where T : class
        {
            return GetContainer().Register<T, V>();
        }

        public IContainer Build()
        {
            IContainer container;

            lock (_syncLock)
            {
                container = _container;
                
                if (container == null)
                    throw new Exception("Container already built.");

                _container = null;
            }

            return container;
        }

        private IContainer GetContainer()
        {
            IContainer container;

            lock (_syncLock)
            {
                container = _container;
            }

            if (container == null)
                throw new Exception("Container already built.");

            return container;
        }

        protected virtual void InternalDispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                Cleanup.SafeMethod(() => (_container as IDisposable)?.Dispose());
            }
        }

        public void Dispose()
        {
            InternalDispose();
            GC.SuppressFinalize(this);
        }
    }
}
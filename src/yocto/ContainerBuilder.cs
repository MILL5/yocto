using System;

namespace yocto
{
    public class ContainerBuilder : IRegisterType, IDisposable
    {
        private object _syncLock = new object();
        private IContainer _container;

        private bool disposed = false;

        public ContainerBuilder()
        {
            _container = Application.Current.GetChildContainer();
        }

        ~ContainerBuilder()
        {
            Dispose(false);
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

        protected virtual void Dispose(bool dispose)
        {
            if (!disposed)
            {
                Cleanup.SafeMethod(() => (_container as IDisposable)?.Dispose());
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
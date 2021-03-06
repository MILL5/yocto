﻿using System;
using System.Collections.Generic;
using System.Linq;
using static yocto.Preconditions;

namespace yocto
{
    internal class MultiInstanceFactory : IInstanceFactory
    {
        private readonly List<InstanceTracker> _trackers = new List<InstanceTracker>();
        private readonly object _syncLock = new object();
        private bool _disposed;

        private readonly Constructor _constructor;

        private class InstanceTracker : IDisposable
        {
            private readonly WeakReference _weakReference;

            public InstanceTracker(IDisposable disposable)
            {
                CheckIsNotNull(nameof(disposable), disposable);

                _weakReference = new WeakReference(disposable, false);
            }

            public void Dispose()
            {
                // We do not support a finalizer because MultiInstanceFactory
                // calls Dispose() for us.  It also manages calling Dispose()
                // once and only once.

                Cleanup.SafeMethod(() =>
                {
                    (_weakReference.Target as IDisposable)?.Dispose();
                });
            }
        }

        public MultiInstanceFactory(IContainer container, Type implementationType, Func<object> factory)
        {
            CheckIsNotNull(nameof(container), container);
            CheckIsNotNull(nameof(implementationType), implementationType);
            
            _constructor = new Constructor(container, implementationType, factory);
        }

        ~MultiInstanceFactory()
        {
            Dispose(false);
        }

        public T Create<T>() where T: class
        {
            T instance = _constructor.Create<T>();

            if (instance is IDisposable disposable)
            {
                lock (_syncLock)
                {
                    _trackers.Add(new InstanceTracker(disposable));
                }
            }

            return instance;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private List<InstanceTracker> GetTrackers()
        {
            lock (_syncLock)
            {
                return _trackers.ToList();
            }
        }

        private void Dispose(bool dispose)
        {
            if (_disposed)
                return;

            Cleanup.SafeMethod(() =>
            {
                var trackers = GetTrackers();

                foreach (var t in trackers)
                {
                    t.Dispose();
                }
            });

            _disposed = true;
        }
    }
}
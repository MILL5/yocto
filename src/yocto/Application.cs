using System;

namespace yocto
{
    public class Application
    {
        private static volatile Container _root = new Container();
        private static readonly object _syncLock = new object();

        public static IContainer Current { get; } = _root;

        public IContainer Push()
        {
            lock (_syncLock)
            {
                _root = (Container)_root.GetChildContainer();
                return _root;
            }
        }

        public IContainer Pop()
        {
            lock (_syncLock)
            {
                if (_root is IChildContainer child)
                {
                    _root = child.Parent as Container;
                    child.Dispose();
                }

                return _root;
            }
        }

        public static T Resolve<T>() where T : class
        {
            return Current.Resolve <T> ();
        }

        public static bool CanResolve<T>() where T : class
        {
            return Current.CanResolve<T>();
        }

        public static bool TryResolve<T>(out T instance) where T : class
        {
            return Current.TryResolve(out instance);
        }

        public static IRegistration Register<T>(T instance) where T : class
        {
            return Current.Register(() => instance);
        }

        public static IRegistration Register<T>(Func<T> factory) where T : class
        {
            return Current.Register(factory);
        }

        public static IRegistration Register<T, V>() where T : class where V : class, T
        {
            return Current.Register<T,V>();
        }
    }
}

using System;

namespace yocto
{
    public class Application
    {
        public static IContainer Current { get; } = new Container();

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

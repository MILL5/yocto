using System;
using static yocto.Preconditions;

namespace yocto
{
    public static class AsSingletonExtension
    {
        public static IRegistration AsSingleton(this IRegistration registration, bool eagerLoad = false)
        {
            CheckIsNotNull(nameof(registration), registration);

            return registration.Register(Instancing.SingletonInstance, eagerLoad);
        }

        public static IRegistration RegisterSingleton<T, V>(this IContainer container) where V : class, T where T : class
        {
            CheckIsNotNull(nameof(container), container);

            return container.Register<T,V>().AsSingleton();
        }

        public static IRegistration RegisterSingleton<T, V>(this IContainer container, bool eagerLoad) where V : class, T where T : class
        {
            CheckIsNotNull(nameof(container), container);

            return container.Register<T, V>().AsSingleton(eagerLoad);
        }

        public static IRegistration RegisterSingleton<T>(this IContainer container, Func<T> factory) where T : class
        {
            CheckIsNotNull(nameof(container), container);
            CheckIsNotNull(nameof(factory), factory);

            return container.Register(factory).AsSingleton();
        }

        public static IRegistration RegisterSingleton<T>(this IContainer container, Func<T> factory, bool eagerLoad) where T : class
        {
            CheckIsNotNull(nameof(container), container);
            CheckIsNotNull(nameof(factory), factory);

            return container.Register(factory).AsSingleton(eagerLoad);
        }
    }
}

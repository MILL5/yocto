using System;
using static yocto.Preconditions;

namespace yocto
{
    public static class AsPooledExtension
    {
        public static IRegistration AsPooled(this IRegistration registration)
        {
            CheckIsNotNull(nameof(registration), registration);

            return registration.Register(Instancing.PooledInstance);
        }

        public static IRegistration AsPooled(this IRegistration registration, int poolSize)
        {
            CheckIsNotNull(nameof(registration), registration);

            return registration.Register(Instancing.PooledInstance, poolSize);
        }

        public static IRegistration RegisterPooled<T, V>(this IContainer container) where V : class, T where T : class
        {
            CheckIsNotNull(nameof(container), container);

            return container.Register<T, V>().AsPooled();
        }

        public static IRegistration RegisterPooled<T, V>(this IContainer container, int poolSize) where V : class, T where T : class
        {
            CheckIsNotNull(nameof(container), container);

            return container.Register<T, V>().AsPooled(poolSize);
        }

        public static IRegistration RegisterPooled<T>(this IContainer container, Func<T> factory) where T : class
        {
            CheckIsNotNull(nameof(container), container);
            CheckIsNotNull(nameof(factory), factory);

            return container.Register(factory).AsPooled();
        }

        public static IRegistration RegisterPooled<T>(this IContainer container, Func<T> factory, int poolSize) where T : class
        {
            CheckIsNotNull(nameof(container), container);
            CheckIsNotNull(nameof(factory), factory);

            return container.Register(factory).AsPooled(poolSize);
        }
    }
}

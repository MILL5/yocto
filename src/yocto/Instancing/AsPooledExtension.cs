using System;

namespace yocto
{
    public static class AsPooledExtension
    {
        public static IRegistration AsPooled(this IRegistration registration)
        {
            return registration.Register(Instancing.PooledInstance);
        }

        public static IRegistration AsPooled(this IRegistration registration, int poolSize)
        {
            return registration.Register(Instancing.PooledInstance, poolSize);
        }

        public static IRegistration RegisterPooled<T, V>(this IContainer container) where V : class, T where T : class
        {
            return container.Register<T, V>().AsPooled();
        }

        public static IRegistration RegisterPooled<T, V>(this IContainer container, int poolSize) where V : class, T where T : class
        {
            return container.Register<T, V>().AsPooled(poolSize);
        }
    }
}

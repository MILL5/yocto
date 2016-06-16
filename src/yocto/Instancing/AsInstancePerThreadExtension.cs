using System;

namespace yocto
{
    public static class AsInstancePerThreadExtension
    {
        public static IRegistration AsPerThread(this IRegistration registration)
        {
            return registration.Register(Instancing.InstancePerThread);
        }

        public static IRegistration RegisterPerThread<T, V>(this IContainer container) where V : class, T where T : class
        {
            return container.Register<T, V>().AsPerThread();
        }
    }
}

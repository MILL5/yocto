using System;

namespace yocto
{
    public static class AsInstancePerThreadExtension
    {
        public static IRegistration AsInstancePerThread(this IRegistration registration)
        {
            return registration.Register(Instancing.InstancePerThread);
        }

        public static IRegistration RegisterPerThread<T, V>(this IContainer container) where V : T
        {
            return container.Register<T, V>().AsInstancePerThread();
        }
    }
}

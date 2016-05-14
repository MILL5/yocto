using System;

namespace yocto
{
    public static class AsSingletonExtension
    {
        public static IRegistration AsSingleton(this IRegistration registration)
        {
            return registration.Register(Instancing.SingletonInstance);
        }

        public static IRegistration RegisterSingleton<T, V>(this IContainer container) where V : T
        {
            return container.Register<T,V>().AsSingleton();
        }
    }
}

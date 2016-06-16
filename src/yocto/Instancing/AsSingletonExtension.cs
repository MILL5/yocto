using System;

namespace yocto
{
    public static class AsSingletonExtension
    {
        public static IRegistration AsSingleton(this IRegistration registration, bool eagerLoad = false)
        {
            return registration.Register(Instancing.SingletonInstance, eagerLoad);
        }

        public static IRegistration RegisterSingleton<T, V>(this IContainer container, bool eagerLoad = false) where V : class, T where T : class
        {
            return container.Register<T,V>().AsSingleton(eagerLoad);
        }
    }
}

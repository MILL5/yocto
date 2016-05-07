using System;

namespace mill5.yocto
{
    public static class AsSingletonExtension
    {
        public static IRegistration AsSingleton(this IRegistration registration)
        {
            return registration.Register(Instancing.SingletonInstance);
        }
    }
}

using System;

namespace yocto
{
    public static class AsMultiInstanceExtension
    {
        public static IRegistration AsMultiInstance(this IRegistration registration)
        {
            return registration.Register(Instancing.MultiInstance);
        }
    }
}

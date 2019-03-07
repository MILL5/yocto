using System;

namespace yocto
{
    public static class AsMultiInstanceExtension
    {
        public static IRegistration AsMultiple(this IRegistration registration)
        {
            return registration.Register(Instancing.MultiInstance);
        }
    }
}

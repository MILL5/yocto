using System;

namespace mill5.yocto
{
    public static class AsMultiInstanceExtension
    {

        public static IRegistration AsMultiInstance(this IRegistration registration)
        {
            return registration.Register(Instancing.MultiInstance);
        }
    }
}

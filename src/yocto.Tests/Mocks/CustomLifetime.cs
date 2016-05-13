using System;

namespace yocto
{
    public static class AsCustomInstanceExtension
    {
        private const string UNKNOWN_LIFETIME = "Unknown";

        public static IRegistration AsCustomInstance(this IRegistration registration)
        {
            return registration.Register(UNKNOWN_LIFETIME);
        }
    }
}
using System;

namespace yocto.tests
{
    public static class AsUnknownInstanceExtension
    {
        private const string UNKNOWN_LIFETIME = "Unknown";

        public static IRegistration AsUnknownInstance(this IRegistration registration)
        {
            return registration.Register(UNKNOWN_LIFETIME, null);
        }
    }
}
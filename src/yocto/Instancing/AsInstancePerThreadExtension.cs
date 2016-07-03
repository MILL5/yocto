using System;
using static yocto.Preconditions;

namespace yocto
{
    public static class AsInstancePerThreadExtension
    {
        public static IRegistration AsPerThread(this IRegistration registration)
        {
            CheckIsNotNull(nameof(registration), registration);

            return registration.Register(Instancing.InstancePerThread);
        }

        public static IRegistration RegisterPerThread<T, V>(this IContainer container) where V : class, T where T : class
        {
            CheckIsNotNull(nameof(container), container);

            return container.Register<T, V>().AsPerThread();
        }

        public static IRegistration RegisterPerThread<T>(this IContainer container, Func<T> factory) where T : class
        {
            CheckIsNotNull(nameof(container), container);
            CheckIsNotNull(nameof(factory), factory);

            return container.Register(factory).AsPerThread();
        }
    }
}

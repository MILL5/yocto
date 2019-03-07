using System;
using static yocto.Preconditions;

namespace yocto
{
    public static class AsInstanceExtension
    {
        public static IRegistration RegisterInstance<T>(this IContainer container, T instance) where T : class
        {
            CheckIsNotNull(nameof(instance), instance);

            return container.Register(() => instance).AsSingleton();
        }
    }
}

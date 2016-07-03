using System;

namespace yocto.tests
{
    public class CustomLifetimeFactory : ILifetimeFactory
    {
        public IInstanceFactory GetInstanceFactory(IContainer container, Type interfaceType, Type implementationType, Func<object> factory, params object[] values)
        {
            return new CustomInstanceFactory(container, implementationType, factory);
        }
    }

    public class CustomInstanceFactory : IInstanceFactory
    {
        public CustomInstanceFactory(IContainer container, Type implementationType, Func<object> factory)
        {
        }

        public T Create<T>() where T : class
        {
            return new Dog() as T;
        }

        public void Dispose()
        {
            
        }
    }

    public static class AsCustomExtensions
    {
        private const string CUSTOM = "custom";

        static AsCustomExtensions()
        {
            Lifetimes.RegisterLifetimeFactory(CUSTOM, new CustomLifetimeFactory());
        }

        public static IRegistration AsCustomInstance(this IRegistration registration)
        {
            return registration.Register(CUSTOM, null);
        }

        public static IRegistration AsCustomInstance<T>(this IRegistration registration, Func<T> factory) where T : class
        {
            return registration.Register(CUSTOM, factory);
        }
    }
}

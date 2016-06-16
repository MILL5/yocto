using System;

namespace yocto.tests
{
    public class CustomLifetimeFactory : ILifetimeFactory
    {
        public IInstanceFactory GetInstanceFactory(IContainer container, Type interfaceType, Type implementationType, params object[] values)
        {
            return new CustomInstanceFactory(container, implementationType);
        }
    }

    public class CustomInstanceFactory : IInstanceFactory
    {
        public CustomInstanceFactory(IContainer container, Type implementationType)
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
            return registration.Register(CUSTOM);
        }
    }
}

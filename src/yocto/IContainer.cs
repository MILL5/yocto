using System;

namespace yocto
{
    public interface IContainer
    {
        IChildContainer GetChildContainer();
        IRegistration Register<T>(Func<T> factory) where T : class;
        IRegistration Register<T, V>() where V : class, T where T : class;
        T Resolve<T>() where T : class;
        bool CanResolve<T>() where T : class;
        bool TryResolve<T>(out T instance) where T : class;
    }
}
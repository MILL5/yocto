using System;

namespace yocto
{
    public interface IContainer
    {
        IChildContainer GetChildContainer();
        IRegistration RegisterSingleton<T, V>() where V : T;
        IRegistration Register<T,V>() where V : T;
        T Resolve<T>() where T : class;
        bool CanResolve<T>() where T : class;
        bool TryResolve<T>(out T instance) where T : class;
    }
}
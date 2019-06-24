using System;

namespace yocto
{
    public interface IRegisterType
    {
        IRegistration Register<T>(T instance) where T : class;
        IRegistration Register<T>(Func<T> factory) where T : class;
        IRegistration Register<T, V>() where V : class, T where T : class;
    }

    public interface IResolveType
    {
        T Resolve<T>() where T : class;
        bool CanResolve<T>() where T : class;
        bool TryResolve<T>(out T instance) where T : class;
    }

    public interface ICreateChildContainer
    {
        IChildContainer GetChildContainer();
    }

    internal interface IResolveByType
    {
        T Resolve<T>(Type type) where T : class;
        object Resolve(Type type);
        bool CanResolve(Type type);
        bool TryResolve<T>(Type type, out T instance) where T : class;
    }

    public interface IContainer : IRegisterType, IResolveType, ICreateChildContainer
    {

    }
}
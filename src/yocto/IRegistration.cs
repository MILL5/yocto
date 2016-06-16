using System;

namespace yocto
{
    public interface IRegistration
    {
        IContainer Container { get; }
        Type InterfaceType { get; }
        Type ImplementationType { get; }
        void Remove();
        object ResolveImplementation();
        IRegistration RegisterImplementation();
        IRegistration Register(string lifetime, params object[] values);
    }
}

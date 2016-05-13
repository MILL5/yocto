using System;

namespace yocto
{
    internal interface IFactoryProvider
    {
        bool CanResolve(Type type);
        bool TryGetFactory(Type type, out IInstanceFactory factory);
    }
}

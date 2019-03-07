using System;

namespace yocto
{
    public interface IInstanceFactory : IDisposable
    {
        T Create<T>() where T : class;
    }
}
using System;

namespace yocto
{
    public interface IInstanceFactory
    {
        T Create<T>() where T : class;
    }
}
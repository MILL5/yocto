using System;

namespace mill5.yocto
{
    public interface IInstanceFactory
    {
        T Create<T>() where T : class;
    }
}
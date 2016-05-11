using System;

namespace yocto
{
    public interface IRegistration
    {
        IRegistration Register(string lifetime);
    }
}

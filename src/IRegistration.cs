using System;
using System.Diagnostics.Contracts;

namespace mill5.yocto
{
    public interface IRegistration
    {
        IRegistration Register(string lifetime);
    }
}

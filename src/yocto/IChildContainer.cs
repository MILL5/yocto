using System;

namespace yocto
{
    public interface IChildContainer : IContainer, IDisposable
    {
        IContainer Parent { get; }
    }
}

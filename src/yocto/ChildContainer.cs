using System;

namespace yocto
{
    internal class ChildContainer : Container, IChildContainer
    {
        public ChildContainer(Container parent) : base(parent)
        {
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public IContainer Parent => _parent;
    }
}

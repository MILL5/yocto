using System;

namespace mill5.yocto
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
    }
}

using System;

namespace mill5.yocto
{
    internal class ChildContainer : Container, IDisposable
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

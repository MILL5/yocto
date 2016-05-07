using System;

namespace mill5.yocto
{
    public class ChildContainer : Container, IDisposable
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

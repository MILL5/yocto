using System;

namespace yocto.tests.common
{
    internal interface IInternal
    {
        bool Foo();
    }

    internal class Internal : IInternal
    {
        public Internal()
        {

        }

        public bool Foo()
        {
            throw new NotImplementedException();
        }
    }

    internal interface IExternal
    {

    }

    internal class External : IExternal
    {
        public External(IInternal someDependency)
        {

        }
    }
}

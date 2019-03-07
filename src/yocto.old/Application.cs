using System;

namespace yocto
{
    public class Application
    {
        public static IContainer Current { get; } = new Container();
    }
}

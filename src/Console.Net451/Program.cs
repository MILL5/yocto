using yocto;
using System;

namespace Console.Net451
{
    public class Program
    {
        static Program()
        {
            Container.Root.Register<ILogger, Logger>();
            Container.Root.Register<IJob, TenSecondJob>();
        }

        static void Main(string[] args)
        {
            var job = Container.Root.Resolve<IJob>();

            job.Run();
        }
    }
}

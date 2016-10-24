using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yocto;

namespace sample.wpf.library
{
    public static class AssemblyRegistration
    {
        public static void Initialize(IContainer container)
        {
            container.Register<ILogger, SimpleLogger>();
        }
    }
}

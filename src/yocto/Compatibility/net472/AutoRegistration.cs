#if NET472
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yocto
{
    public static partial class AutoRegistration
    {
        public static void Register(Type assemblyFromType)
        {
            Register(assemblyFromType.Assembly, Application.Current);
        }
    }
}
#endif

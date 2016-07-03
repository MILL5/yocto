using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace yocto
{
    public static class TypeInfoExtensions
    {
        public static IEnumerable<ConstructorInfo> DeclaredConstructors(this TypeInfo extendThis)
        {
            return extendThis.DeclaredConstructors;
        }
    }
}

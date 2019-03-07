#if NET472
using System;
using System.Collections.Generic;
using System.Reflection;

namespace yocto
{
    internal static class TypeInfoExtensions
    {
        public static IEnumerable<ConstructorInfo> DeclaredConstructors(this TypeInfo extendThis)
        {
            return extendThis.DeclaredConstructors;
        }
    }
}
#endif
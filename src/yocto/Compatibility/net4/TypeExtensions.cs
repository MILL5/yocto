#if NET4
using System;
using System.Collections.Generic;
using System.Reflection;

namespace yocto
{
    internal static class TypeExtensions
    {
        public static TypeInfo GetTypeInfo(this Type type)
        {
            return new TypeInfo(type);
        }

        public static IEnumerable<MethodInfo> GetRuntimeMethods(this Type type)
        {
            return type.GetMethods();
        }
    }
}
#endif
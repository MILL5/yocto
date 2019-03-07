#if NETSTANDARD1_0
using System;
using System.Collections.Generic;
using System.Reflection;

namespace yocto
{
    internal static class AssemblyExtensions
    {
        public static IEnumerable<TypeInfo> ExportedTypes(this Assembly extendThis)
        {
            var typeInfo = new List<TypeInfo>();

            foreach (var type in extendThis.ExportedTypes)
            {
                typeInfo.Add(type.GetTypeInfo());
            }

            return typeInfo;
        }
    }
}
#endif

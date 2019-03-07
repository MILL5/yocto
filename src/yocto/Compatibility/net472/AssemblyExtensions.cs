#if NET472
using System;
using System.Collections.Generic;
using System.Reflection;

namespace yocto
{
    internal static class AssemblyExtensions
    {
        public static IEnumerable<TypeInfo> ExportedTypes(this Assembly extendThis)
        {
            var types = new List<TypeInfo>();

            foreach (var type in extendThis.GetExportedTypes())
            {
                types.Add(type.GetTypeInfo());
            }

            return types;
        }
    }
}
#endif
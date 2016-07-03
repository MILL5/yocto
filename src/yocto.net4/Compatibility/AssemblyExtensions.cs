using System;
using System.Collections.Generic;
using System.Reflection;

namespace yocto
{
    internal static class AssemblyExtensions
    {
        public static IEnumerable<TypeInfo> ExportedTypes(this Assembly extendThis)
        {
            List<TypeInfo> types = new List<TypeInfo>();

            foreach (var t in extendThis.GetExportedTypes())
            {
                types.Add(new TypeInfo(t));
            }

            return types;
        }
    }
}

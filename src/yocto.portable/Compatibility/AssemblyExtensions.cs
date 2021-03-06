﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace yocto
{
    internal static class AssemblyExtensions
    {
        public static IEnumerable<TypeInfo> ExportedTypes(this Assembly extendThis)
        {
            return extendThis.DefinedTypes;
        }
    }
}

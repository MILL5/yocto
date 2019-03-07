using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static yocto.Preconditions;
// ReSharper disable UseNullPropagation
// ReSharper disable InconsistentNaming

namespace yocto
{
    /// <summary>
    /// We are not adding support for detecting multiple registration calls since
    /// the container has last one wins approach for registration
    /// </summary>
    public static partial class AutoRegistration
    {
        private static bool TryGetRegister(Assembly assembly, out List<MethodInfo> initializers)
        {
            const string AssemblyRegistration = "AssemblyRegistration";
            const string Initialize = "Initialize";

            bool found = false;
            initializers = new List<MethodInfo>();

            var types = assembly.ExportedTypes();

            var registrationTypes = types.Where(t => t.Name.Equals(AssemblyRegistration));

            foreach (var typeinfo in registrationTypes)
            {
                var type = typeinfo.AsType();

                var members = type.GetRuntimeMethods().Where(m => m.Name.Equals(Initialize));

                foreach (var member in members)
                {
                    var mi = member;
                    
                    if (mi.IsPublic && mi.IsStatic)
                    {
                        var parameters = mi.GetParameters();

                        if (parameters.Length == 1 &&
                            parameters[0].ParameterType == typeof(IContainer))
                        {
                            initializers.Add(mi);
                            found = true;
                        }
                    }
                }
            }

            return found;
        }

        public static void Register(Assembly assembly, IContainer container)
        {
            CheckIsNotNull(nameof(assembly), assembly);
            CheckIsNotNull(nameof(container), container);

            if (TryGetRegister(assembly, out var initializers))
            {
                foreach (var initializer in initializers)
                {
                    initializer.Invoke(null, new object[] { Application.Current });
                }
            }
        }

        public static void Register(Assembly assembly)
        {
            Register(assembly, Application.Current);
        }
    }
}
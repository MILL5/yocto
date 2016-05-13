using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static yocto.Preconditions;
// ReSharper disable UseNullPropagation
// ReSharper disable InconsistentNaming

namespace yocto
{
    public static class AutoRegistration
    {
        static AutoRegistration()
        {
        }

        private static bool TryGetRegister(Assembly assembly, out List<MethodInfo> intializers)
        {
            const string AssemblyRegistration = "AssemblyRegistration";
            const string Initialize = "Initialize";

            bool found = false;
            intializers = new List<MethodInfo>();

            var types = assembly.DefinedTypes;

            var registrationTypes = types.Where(t => t.Name.Equals(AssemblyRegistration));

            foreach (var typeinfo in registrationTypes)
            {
                Type type = typeinfo.AsType();

                var members = type.GetRuntimeMethods().Where(m => m.Name.Equals(Initialize));

                foreach (var member in members)
                {
                    var mi = member as MethodInfo;
                    if (mi != null)
                    {
                        var parameters = mi.GetParameters();

                        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(IContainer))
                        {
                            intializers.Add(mi);
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

            List<MethodInfo> intializers;

            if (TryGetRegister(assembly, out intializers))
            {
                foreach (var initializer in intializers)
                {
                    initializer.Invoke(null, new[] { Application.Current });
                }
            }
        }

        public static void Register(Assembly assembly)
        {
            Register(assembly, Application.Current);
        }
    }
}
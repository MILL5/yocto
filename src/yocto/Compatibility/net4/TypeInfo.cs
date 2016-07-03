#if NET4
using System;
using System.Collections.Generic;
using System.Reflection;
using static yocto.Preconditions;

namespace yocto
{
    internal class TypeInfo
    {
        private readonly Type _type;

        public TypeInfo(Type type)
        {
            CheckIsNotNull(nameof(type), type);
            _type = type;
        }

        public string Name
        {
            get
            {
                return _type.Name;
            }
        }

        public Type AsType()
        {
            return _type;
        }

        public IEnumerable<ConstructorInfo> DeclaredConstructors()
        {
            return _type.GetConstructors();
        }
    }
}
#endif
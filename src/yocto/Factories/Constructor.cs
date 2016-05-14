using System;
using System.Collections.Generic;
using System.Reflection;
using static yocto.Preconditions;
// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace yocto
{
    internal class Constructor
    {
        private readonly ConstructorInfo _constructorInfo;
        private readonly List<IInstanceFactory> _paramFactories;

        public Constructor(IContainer container, Type implementationType)
        {
            CheckIsNotNull(nameof(container), container);
            CheckIsNotNull(nameof(implementationType), implementationType);

            var validConstructors = GetValidConstructors(container, implementationType);

            int numOfValidConstructors = validConstructors.Count;

            switch (numOfValidConstructors)
            {
                case 0:
                    throw new Exception($"Could not find a constructor to create the type. [{implementationType.Name}]");
                case 1:
                    _constructorInfo = validConstructors[0];
                    _paramFactories = GetParameterFactories(container, _constructorInfo);
                    break;
                default:
                    throw new Exception($"Found more than one constructor to create the type. [{implementationType.Name}]");
            }
        }

        public T Create<T>() where T : class
        {
            var parameters = _constructorInfo.GetParameters();
            var paramObjects = new List<object>(parameters.Length);

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameterFactory = _paramFactories[i];
                var o = parameterFactory.Create<object>();

                paramObjects.Add(o);
            }

            return (T)_constructorInfo.Invoke(paramObjects.ToArray());
        }

        private static List<IInstanceFactory> GetParameterFactories(IContainer container, ConstructorInfo constructor)
        {
            IFactoryProvider factoryProvider = container as IFactoryProvider;

            CheckIsNotNull(nameof(factoryProvider), factoryProvider);

            var paramFactories = new List<IInstanceFactory>();

            foreach (var p in constructor.GetParameters())
            {
                var paramType = p.ParameterType;
                IInstanceFactory pf;
                
                if (factoryProvider.TryGetFactory(paramType, out pf))
                {
                    paramFactories.Add(pf);
                }
                else
                    throw new Exception($"Could not resolve factory for parameter type. [{paramType.Name}]");
            }

            return paramFactories;
        }

        private static List<ConstructorInfo> GetValidConstructors(IContainer container, Type implementationType)
        {
            IFactoryProvider factoryProvider = container as IFactoryProvider;

            CheckIsNotNull(nameof(factoryProvider), factoryProvider);

            var typeInfo = implementationType.GetTypeInfo();

            var constructors = typeInfo.DeclaredConstructors;

            var validConstructors = new List<ConstructorInfo>();

            foreach (var c in constructors)
            {
                bool canConstruct = true;

                foreach (var p in c.GetParameters())
                {
                    if (!factoryProvider.CanResolve(p.ParameterType))
                    {
                        canConstruct = false;
                        break;
                    }
                }

                if (canConstruct)
                    validConstructors.Add(c);
            }
            return validConstructors;
        }
    }
}

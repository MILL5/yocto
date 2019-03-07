using System;
using System.Collections.Generic;
using System.Reflection;
using static yocto.Preconditions;
// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace yocto
{
    internal class Constructor
    {
        private readonly IContainer _container;
        private readonly ConstructorInfo _constructorInfo;
        private readonly Func<object> _factory;

        public Constructor(IContainer container, Type implementationType, Func<object> factory)
        {
            CheckIsNotNull(nameof(container), container);
            CheckIsNotNull(nameof(implementationType), implementationType);

            if (factory != null)
            {
                _factory = factory;
                return;
            }

            var validConstructors = GetValidConstructors(container, implementationType);

            int numOfValidConstructors = validConstructors.Count;

            switch (numOfValidConstructors)
            {
                case 0:
                    throw new Exception($"Could not find a constructor to create the type. [{implementationType.Name}]");
                case 1:
                    _constructorInfo = validConstructors[0];
                    _container = container;
                    break;
                default:
                    throw new Exception($"Found more than one constructor to create the type. [{implementationType.Name}]");
            }
        }

        public T Create<T>() where T : class
        {
            if (_factory == null)
                return CreateUsingConstructor<T>();

            return (T) _factory();
        }

        private T CreateUsingConstructor<T>() where T : class
        {
            var parameters = _constructorInfo.GetParameters();
            var paramObjects = new List<object>(parameters.Length);

            var paramFactories = GetParameterFactories(_container, _constructorInfo);

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameterFactory = paramFactories[i];
                var o = parameterFactory.Create<object>();

                paramObjects.Add(o);
            }

            return (T) _constructorInfo.Invoke(paramObjects.ToArray());
        }

        private static List<IInstanceFactory> GetParameterFactories(IContainer container, ConstructorInfo constructor)
        {
            IFactoryProvider factoryProvider = container as IFactoryProvider;

            CheckIsNotNull(nameof(factoryProvider), factoryProvider);

            var paramFactories = new List<IInstanceFactory>();

            foreach (var p in constructor.GetParameters())
            {
                var paramType = p.ParameterType;

                // ReSharper disable once PossibleNullReferenceException
                if (factoryProvider.TryGetFactory(paramType, out var pf))
                {
                    paramFactories.Add(pf);
                }
            }

            return paramFactories;
        }

        private static List<ConstructorInfo> GetValidConstructors(IContainer container, Type implementationType)
        {
            IFactoryProvider factoryProvider = (IFactoryProvider)container;

            var typeInfo = implementationType.GetTypeInfo();

            var constructors = typeInfo.DeclaredConstructors();

            var validConstructors = new List<ConstructorInfo>();

            foreach (var c in constructors)
            {
                if (c.IsStatic)
                    continue;

                var parameters = c.GetParameters();

                bool canConstruct = parameters.Length == 0;

                if (!canConstruct)
                {
                    bool canConstructParams = true;

                    foreach (var p in c.GetParameters())
                    {
                        if (!factoryProvider.CanResolve(p.ParameterType))
                        {
                            canConstructParams = false;
                            break;
                        }
                    }

                    canConstruct = canConstructParams;
                }

                if (canConstruct)
                    validConstructors.Add(c);
            }
            return validConstructors;
        }
    }
}

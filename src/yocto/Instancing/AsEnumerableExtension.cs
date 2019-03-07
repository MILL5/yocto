using System;
using System.Collections.Generic;

using static yocto.Preconditions;

namespace yocto
{
    public static class AsEnumerableExtension
    {
        private static readonly Dictionary<string,List<IRegistration>> Enumerables = new Dictionary<string, List<IRegistration>>();
        private static readonly object SyncLock = new object();

        private static string GetEnumerableKey(IContainer container, Type interfaceType)
        {
            return $"{container.GetHashCode()}_{interfaceType.GetHashCode()}";
        }

        public static IRegistration AsEnumerable(this IRegistration registration)
        {
            CheckIsNotNull(nameof(registration), registration);

            registration.Remove();

            var registrationForImplementation = registration.RegisterImplementation();

            Register(registration.Container, registration.InterfaceType, registrationForImplementation);

            return registrationForImplementation;
        }

        public static IRegistration RegisterEnumerable<T, V>(this IContainer container) where V : class, T where T : class
        {
            CheckIsNotNull(nameof(container), container);

            var registrationForImplementation = container.Register<V, V>();

            Register(container, typeof(T), registrationForImplementation);

            return registrationForImplementation;
        }

        public static IRegistration RegisterEnumerable<T>(this IContainer container, Func<T> factory) where T : class
        {
            CheckIsNotNull(nameof(container), container);
            CheckIsNotNull(nameof(factory), factory);

            var registrationForImplementation = container.Register(factory);

            Register(container, typeof(T), registrationForImplementation);

            return registrationForImplementation;
        }

        private static void Register(IContainer container, Type interfaceType, IRegistration registrationForImplementation)
        {
            var key = GetEnumerableKey(container, interfaceType);

            lock (SyncLock)
            {
                if (!Enumerables.TryGetValue(key, out var registrations))
                {
                    registrations = new List<IRegistration>();
                    Enumerables.Add(key, registrations);
                }

                registrations.Add(registrationForImplementation);
            }
        }

        public static IEnumerable<T> ResolveAll<T>(this IContainer container)
        {
            CheckIsNotNull(nameof(container), container);

            var key = GetEnumerableKey(container, typeof(T));
            List<T> listOfT;

            lock (SyncLock)
            {
                if (!Enumerables.TryGetValue(key, out var registrations))
                {
                    throw new Exception("Interface type is not registered.");
                }

                listOfT = new List<T>();

                foreach (var r in registrations)
                {
                    listOfT.Add((T)r.ResolveImplementation());
                }
            }

            return listOfT.ToArray();
        }
    }
}

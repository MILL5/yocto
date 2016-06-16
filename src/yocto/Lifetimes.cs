using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using static yocto.Preconditions;
// ReSharper disable InconsistentNaming

namespace yocto
{
    public static class Lifetimes
    {
        private static readonly ConcurrentDictionary<string, ILifetimeFactory> _lifetimes;

        static Lifetimes()
        {
            var lifetimes = new Dictionary<string, ILifetimeFactory>
            {
                {Instancing.MultiInstance, new MultiInstanceLifetimeFactory()},
                {Instancing.SingletonInstance, new SingletonLifetimeFactory()},
                {Instancing.InstancePerThread, new InstancePerThreadLifetimeFactory()},
                {Instancing.PooledInstance, new PooledLifetimeFactory()}
            };


            _lifetimes = new ConcurrentDictionary<string, ILifetimeFactory>(lifetimes);
        }

        public static ILifetimeFactory GetLifetimeFactory(string lifetime)
        {
            CheckIsNotNullEmptyOrWhitespace(nameof(lifetime), lifetime);

            ILifetimeFactory lifetimeFactory;

            if (_lifetimes.TryGetValue(lifetime, out lifetimeFactory))
            {
                return lifetimeFactory;
            }

            throw new NotSupportedException($"Lifetime not found. [{lifetime}]");
        }

        public static void RegisterLifetimeFactory(string lifetime, ILifetimeFactory lifetimeFactory)
        {
            CheckIsNotNullEmptyOrWhitespace(nameof(lifetime), lifetime);
            CheckIsNotNull(nameof(lifetimeFactory), lifetimeFactory);

            _lifetimes.AddOrUpdate(lifetime, lt => lifetimeFactory,
                (lt, olf) => lifetimeFactory);
        }
    }
}

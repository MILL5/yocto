using System;

namespace yocto.tests
{
    public class LoggingLifetimeScope : IDisposable
    {
        private readonly string _lifetime;
        private readonly ILifetimeFactory _previousLifetimeFactory;

        public LoggingLifetimeScope(string lifetime)
        {
            Preconditions.CheckIsNotNullEmptyOrWhitespace(nameof(lifetime), lifetime);

            _lifetime = lifetime;
            _previousLifetimeFactory = Lifetimes.GetLifetimeFactory(lifetime);
            var loggingLifetimeFactory = new LoggingLifetimeFactory(lifetime, _previousLifetimeFactory, null);
            Lifetimes.RegisterLifetimeFactory(lifetime, loggingLifetimeFactory);
        }

        public void Dispose()
        {
            Lifetimes.RegisterLifetimeFactory(_lifetime, _previousLifetimeFactory);
        }
    }
}

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Yocto;

/// <summary>Produces instances for one registration according to a lifetime policy.</summary>
public interface IInstanceFactory : IDisposable
{
    /// <param name="scope">The container the service is being resolved from.</param>
    object GetInstance(IContainer scope);
}

/// <summary>Creates <see cref="IInstanceFactory"/> objects for a named lifetime. Register custom ones with <see cref="Lifetimes.Register"/>.</summary>
public interface ILifetimeFactory
{
    /// <param name="context">Activates new instances and tracks them for disposal.</param>
    /// <param name="options">Lifetime-specific options passed to <see cref="IRegistration.WithLifetime"/>.</param>
    IInstanceFactory CreateInstanceFactory(LifetimeContext context, IReadOnlyList<object?> options);
}

/// <summary>What a lifetime needs from the container: activation and disposal tracking.</summary>
public sealed class LifetimeContext
{
    private readonly Registration _registration;

    internal LifetimeContext(Registration registration) => _registration = registration;

    public IRegistration Registration => _registration;

    /// <summary>The container that owns the registration. Use it for instances that outlive any one scope.</summary>
    public IContainer Owner => _registration.Owner;

    /// <summary>Creates a new instance, resolving dependencies from <paramref name="scope"/>. The instance is not tracked.</summary>
    public object Activate(IContainer scope) => _registration.Activate(scope);

    /// <summary>Disposes <paramref name="instance"/> when <paramref name="scope"/> is disposed (in reverse creation order).</summary>
    /// <param name="weak">Track without keeping the instance alive (used for transient services).</param>
    [SuppressMessage("Performance", "CA1822", Justification = "Instance API by design.")]
    public void TrackForDisposal(IContainer scope, object instance, bool weak = false) =>
        AsContainer(scope).Track(instance, weak);

    /// <summary>Stops tracking <paramref name="instance"/>. Returns <c>false</c> if the scope already released it.</summary>
    [SuppressMessage("Performance", "CA1822", Justification = "Instance API by design.")]
    public bool Untrack(IContainer scope, object instance) => AsContainer(scope).Untrack(instance);

    private static Container AsContainer(IContainer scope) =>
        scope as Container ?? throw new ArgumentException("Scope must be a yocto container.", nameof(scope));
}

/// <summary>The lifetime registry. Built-in lifetimes are always available; custom ones can be added by name.</summary>
public static class Lifetimes
{
    /// <summary>A new instance per resolve (the default). Disposables are tracked weakly by the resolving scope.</summary>
    public const string Multiple = "MultiInstance";

    /// <summary>One instance per owning container, created lazily. Option: <c>bool eagerLoad</c>.</summary>
    public const string Singleton = "SingletonInstance";

    /// <summary>One instance per container (scope) that resolves it.</summary>
    public const string Scoped = "ScopedInstance";

    /// <summary>One instance per thread.</summary>
    public const string PerThread = "InstancePerThread";

    /// <summary>A fixed-size, round-robin pool. Option: <c>int poolSize</c> (minimum 2).</summary>
    public const string Pooled = "PooledInstance";

    /// <summary>A pre-built instance owned by the caller. Never disposed by the container.</summary>
    public const string Instance = "ExternalInstance";

    private static readonly ConcurrentDictionary<string, ILifetimeFactory> s_factories = new(StringComparer.Ordinal)
    {
        [Multiple] = new DelegateLifetimeFactory((c, _) => new MultiInstanceFactory(c)),
        [Singleton] = new DelegateLifetimeFactory((c, o) => new SingletonInstanceFactory(c, o.Count > 0 && Convert.ToBoolean(o[0], null))),
        [Scoped] = new DelegateLifetimeFactory((c, _) => new ScopedInstanceFactory(c)),
        [PerThread] = new DelegateLifetimeFactory((c, _) => new PerThreadInstanceFactory(c)),
        [Pooled] = new DelegateLifetimeFactory((c, o) => new PooledInstanceFactory(c, o.Count > 0 ? Convert.ToInt32(o[0], null) : PooledInstanceFactory.DefaultPoolSize)),
        [Instance] = new DelegateLifetimeFactory((c, _) => new ExternalInstanceFactory(c)),
    };

    public static ILifetimeFactory Get(string lifetime)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(lifetime);
        return s_factories.TryGetValue(lifetime, out var factory)
            ? factory
            : throw new NotSupportedException($"Lifetime not found. [{lifetime}]");
    }

    /// <summary>Adds or replaces a lifetime.</summary>
    public static void Register(string lifetime, ILifetimeFactory factory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(lifetime);
        ArgumentNullException.ThrowIfNull(factory);
        s_factories[lifetime] = factory;
    }

    /// <summary>Lifetimes whose instances outlive a scope, and so must not depend on scoped services.</summary>
    internal static bool IsLongLived(string lifetime) =>
        lifetime is Singleton or PerThread or Pooled;

    private sealed class DelegateLifetimeFactory(Func<LifetimeContext, IReadOnlyList<object?>, IInstanceFactory> create) : ILifetimeFactory
    {
        public IInstanceFactory CreateInstanceFactory(LifetimeContext context, IReadOnlyList<object?> options) => create(context, options);
    }
}

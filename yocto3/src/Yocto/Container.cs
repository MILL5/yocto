using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace Yocto;

/// <summary>
/// The yocto container. Registrations default to <see cref="Lifetimes.Multiple"/>; unresolved services fall back to the
/// parent container. Disposing a container disposes its children, then every instance it owns in reverse creation order.
/// </summary>
/// <remarks>
/// Also a drop-in <see cref="IServiceProvider"/> for Microsoft.Extensions.DependencyInjection: it supports keyed services,
/// open generics, <see cref="IEnumerable{T}"/>, scopes (<see cref="IServiceScopeFactory"/>) and async disposal.
/// </remarks>
public sealed class Container : IContainer, IServiceScope, IServiceProviderIsKeyedService, ISupportRequiredService
{
    private const int PruneThreshold = 256;

    private readonly ConcurrentDictionary<ServiceIdentifier, Registration[]> _registrations = new();
    private readonly ConcurrentDictionary<ServiceIdentifier, (long Version, Array Items)> _enumerables = new();
    private readonly Dictionary<Registration, object> _scoped = new(ReferenceEqualityComparer.Instance);
    private readonly List<object> _disposables = [];
    private readonly List<Container> _children = [];
    private readonly Lock _lock = new();
    private readonly Lock _scopedLock = new();
    private readonly Container? _parent;
    private readonly Container _scopeRoot;
    private ScopeFactory? _scopeFactory;
    private int _pruneAt = PruneThreshold;
    private long _version;
    private int _disposed;

    public Container()
    {
        _scopeRoot = this;
    }

    private Container(Container parent, Container? scopeRoot)
    {
        _parent = parent;
        _scopeRoot = scopeRoot ?? this;
    }

    public IContainer? Parent => _parent;

    IServiceProvider IServiceScope.ServiceProvider => this;

    public IContainer CreateChildContainer()
    {
        return Attach(new Container(this, scopeRoot: null));
    }

    #region Registration

    public IRegistration Register(Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType,
        object? serviceKey = null) =>
        Add(CreateRegistration(serviceType, implementationType, serviceKey), replace: true);

    public IRegistration Register(Type serviceType, Func<IContainer, object?, object> factory, object? serviceKey = null) =>
        Add(CreateRegistration(serviceType, factory, serviceKey), replace: true);

    public IRegistration RegisterEnumerable(Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType,
        object? serviceKey = null) =>
        Add(CreateRegistration(serviceType, implementationType, serviceKey), replace: false);

    public IRegistration RegisterEnumerable(Type serviceType, Func<IContainer, object?, object> factory, object? serviceKey = null) =>
        Add(CreateRegistration(serviceType, factory, serviceKey), replace: false);

    public bool Unregister(Type serviceType, object? serviceKey = null)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        Registration[]? removed;
        lock (_lock)
        {
            if (!_registrations.TryRemove(new(serviceType, serviceKey), out removed))
                return false;
        }
        Release(removed);
        return true;
    }

    internal void Remove(Registration registration)
    {
        var id = new ServiceIdentifier(registration.ServiceType, registration.ServiceKey);
        lock (_lock)
        {
            if (!_registrations.TryGetValue(id, out var existing))
                return;
            var remaining = Array.FindAll(existing, r => !ReferenceEquals(r, registration));
            if (remaining.Length == existing.Length)
                return;
            if (remaining.Length == 0)
                _registrations.TryRemove(id, out _);
            else
                _registrations[id] = remaining;
        }
        Release([registration]);
    }

    internal void OnRegistrationsChanged() => Interlocked.Increment(ref _version);

    /// <summary>
    /// Changes whenever a registration changes in this container or an ancestor: the sum of their versions, which only
    /// increase. Unrelated containers do not affect it.
    /// </summary>
    internal long RegistrationStamp
    {
        get
        {
            long stamp = 0;
            for (var c = this; c is not null; c = c._parent)
                stamp += Volatile.Read(ref c._version);
            return stamp;
        }
    }

    private Registration CreateRegistration(Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType,
        object? serviceKey)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(implementationType);
        if (implementationType.IsAbstract || implementationType.IsInterface)
            throw new ArgumentException($"Implementation type must be a concrete class. [{implementationType.Name}]", nameof(implementationType));

        if (serviceType.IsGenericTypeDefinition != implementationType.IsGenericTypeDefinition)
            throw new ArgumentException($"Open generic services require open generic implementations, and vice versa. [{serviceType.Name} -> {implementationType.Name}]", nameof(implementationType));
        if (!serviceType.IsGenericTypeDefinition && !serviceType.IsAssignableFrom(implementationType))
            throw new ArgumentException($"[{implementationType.Name}] does not implement [{serviceType.Name}].", nameof(implementationType));

        return new Registration(this, serviceType, implementationType, null, serviceKey);
    }

    private Registration CreateRegistration(Type serviceType, Func<IContainer, object?, object> factory, object? serviceKey)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(factory);
        if (serviceType.IsGenericTypeDefinition)
            throw new ArgumentException($"Factories cannot be registered for open generic services. [{serviceType.Name}]", nameof(serviceType));
        return new Registration(this, serviceType, null, factory, serviceKey);
    }

    private Registration Add(Registration registration, bool replace)
    {
        ThrowIfDisposed();
        var id = new ServiceIdentifier(registration.ServiceType, registration.ServiceKey);
        Registration[]? replaced = null;
        lock (_lock)
        {
            // Rechecked under the lock: disposal snapshots registrations under it, so none can be added afterwards.
            ThrowIfDisposed();
            _registrations.TryGetValue(id, out var existing);
            if (replace)
                replaced = existing;
            _registrations[id] = replace || existing is null ? [registration] : [.. existing, registration];
        }
        OnRegistrationsChanged();
        if (replaced is not null)
            Release(replaced);
        return registration;
    }

    private void Release(Registration[] registrations)
    {
        if (registrations.Length == 0)
            return;
        OnRegistrationsChanged();
        foreach (var r in registrations)
        {
            r.Release();
            r.ReleaseDerived();
        }
    }

    #endregion

    #region Resolution

    public bool CanResolve(Type serviceType, object? serviceKey = null)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        ThrowIfDisposed();
        if (serviceType.IsGenericTypeDefinition)
            return false;
        return FindSingle(serviceType, serviceKey) is not null
            || (serviceKey is null && IsBuiltIn(serviceType))
            || TryGetEnumerableElementType(serviceType, out _);
    }

    public object Resolve(Type serviceType, object? serviceKey = null) =>
        TryResolve(serviceType, out var instance, serviceKey)
            ? instance
            : throw new InvalidOperationException(
                $"Service type is not registered. [{serviceType.FullName}{(serviceKey is null ? "" : $", key: {serviceKey}")}]");

    public bool TryResolve(Type serviceType, [NotNullWhen(true)] out object? instance, object? serviceKey = null)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        ThrowIfDisposed();

        // An open generic definition is not a resolvable service; its registration has no activator or factory.
        if (serviceType.IsGenericTypeDefinition)
        {
            instance = null;
            return false;
        }

        if (TryGetEnumerableElementType(serviceType, out var elementType))
        {
            instance = ResolveEnumerable(elementType, serviceKey);
            return true;
        }
        if (ReferenceEquals(serviceKey, KeyedService.AnyKey))
            throw new InvalidOperationException("KeyedService.AnyKey cannot be used to resolve a single service.");
        if (FindSingle(serviceType, serviceKey) is { } registration)
        {
            instance = registration.GetInstance(this);
            return true;
        }
        instance = serviceKey is not null || !IsBuiltIn(serviceType) ? null
            : serviceType == typeof(IServiceScopeFactory) ? _scopeRoot._scopeFactory ??= new ScopeFactory(_scopeRoot)
            : this;
        return instance is not null;
    }

    public IReadOnlyList<object> ResolveAll(Type serviceType, object? serviceKey = null)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        ThrowIfDisposed();
        var registrations = FindAll(serviceType, serviceKey);
        var instances = new object[registrations.Count];
        for (var i = 0; i < instances.Length; i++)
            instances[i] = registrations[i].GetInstance(this);
        return instances;
    }

    // Enumerables made only of shared instances (singletons, external instances) are cached, as in Microsoft's container.
    private Array ResolveEnumerable(Type elementType, object? serviceKey)
    {
        var id = new ServiceIdentifier(elementType, serviceKey);
        var version = RegistrationStamp;
        if (_enumerables.TryGetValue(id, out var cached) && cached.Version == version)
            return cached.Items;

        var registrations = FindAll(elementType, serviceKey);
        var items = ToTypedArray(elementType, [.. registrations.Select(r => r.GetInstance(this))]);
        if (registrations.All(r => r.Lifetime is Lifetimes.Singleton or Lifetimes.Instance))
            _enumerables[id] = (version, items);
        return items;
    }

    object? IServiceProvider.GetService(Type serviceType) =>
        TryResolve(serviceType, out var instance) ? instance : null;

    object? IKeyedServiceProvider.GetKeyedService(Type serviceType, object? serviceKey) =>
        TryResolve(serviceType, out var instance, serviceKey) ? instance : null;

    object IKeyedServiceProvider.GetRequiredKeyedService(Type serviceType, object? serviceKey) =>
        Resolve(serviceType, serviceKey);

    object ISupportRequiredService.GetRequiredService(Type serviceType) => Resolve(serviceType);

    bool IServiceProviderIsService.IsService(Type serviceType) => IsService(serviceType, null);

    bool IServiceProviderIsKeyedService.IsKeyedService(Type serviceType, object? serviceKey) => IsService(serviceType, serviceKey);

    private bool IsService(Type serviceType, object? serviceKey) =>
        !serviceType.IsGenericTypeDefinition && CanResolve(serviceType, serviceKey);

    /// <summary>The registration a single resolve would use: the last match in the nearest container that has one.</summary>
    internal Registration? FindSingle(Type serviceType, object? serviceKey)
    {
        for (var c = this; c is not null; c = c._parent)
        {
            if (c.FindLocal(serviceType, serviceKey, single: true) is [var last])
                return last;
        }
        return null;
    }

    private List<Registration> FindAll(Type serviceType, object? serviceKey)
    {
        for (var c = this; c is not null; c = c._parent)
        {
            var found = c.FindLocal(serviceType, serviceKey, single: false);
            if (found.Count > 0)
                return found;
        }
        return [];
    }

    private List<Registration> FindLocal(Type serviceType, object? serviceKey, bool single)
    {
        var results = new List<Registration>();
        var anyKey = ReferenceEquals(serviceKey, KeyedService.AnyKey);
        var openType = serviceType.IsConstructedGenericType ? serviceType.GetGenericTypeDefinition() : null;

        if (anyKey)
        {
            // Every keyed registration for the service, excluding the AnyKey fallbacks themselves.
            foreach (var (id, registrations) in _registrations)
            {
                if (id.Key is null || ReferenceEquals(id.Key, KeyedService.AnyKey))
                    continue;
                if (id.ServiceType == serviceType)
                    results.AddRange(registrations);
                else if (id.ServiceType == openType)
                    results.AddRange(registrations.Select(r => Close(r, serviceType, r.ServiceKey)).OfType<Registration>());
            }
            return Ordered(results);
        }

        Collect(results, serviceType, serviceKey, serviceKey, openType);

        // AnyKey registrations are a fallback for single resolves only; they never appear in enumerables.
        if (single && serviceKey is not null && results.Count == 0)
            Collect(results, serviceType, KeyedService.AnyKey, serviceKey, openType);

        if (single && results.Count > 0)
            return [PreferredSingle(results, serviceType)];
        return Ordered(results);
    }

    private void Collect(List<Registration> results, Type serviceType, object? lookupKey, object? effectiveKey, Type? openType)
    {
        var deriveKey = !Equals(lookupKey, effectiveKey);
        if (_registrations.TryGetValue(new(serviceType, lookupKey), out var exact))
            results.AddRange(deriveKey ? exact.Select(r => DeriveCached(r, serviceType, effectiveKey)) : exact);
        if (openType is not null && _registrations.TryGetValue(new(openType, lookupKey), out var open))
            results.AddRange(open.Select(r => Close(r, serviceType, effectiveKey)).OfType<Registration>());
    }

    // Closed registrations win over open generic ones; otherwise the most recent registration wins.
    private static Registration PreferredSingle(List<Registration> candidates, Type serviceType)
    {
        var closed = candidates.Where(r => r.ServiceType == serviceType && !r.IsDerivedFromOpenGeneric).ToList();
        var pool = closed.Count > 0 ? closed : candidates;
        return pool.MaxBy(r => r.Sequence)!;
    }

    private static List<Registration> Ordered(List<Registration> registrations) =>
        registrations.Count > 1 ? [.. registrations.OrderBy(r => r.Sequence)] : registrations;

    private static Registration? Close(Registration open, Type closedServiceType, object? key)
    {
        if (!RuntimeFeature.IsDynamicCodeSupported && closedServiceType.GetGenericArguments().Any(a => a.IsValueType))
            throw new InvalidOperationException($"Open generic services cannot be closed over value types when dynamic code is unsupported. [{closedServiceType}]");
        try
        {
            return DeriveCached(open, closedServiceType, key);
        }
        catch (ArgumentException)
        {
            // Generic constraints not satisfied: this open generic does not apply to the requested type.
            return null;
        }
    }

    private static Registration DeriveCached(Registration source, Type serviceType, object? key) =>
        source.GetOrDerive(serviceType, key);

    private static bool IsBuiltIn(Type serviceType) =>
        serviceType == typeof(IContainer) || serviceType == typeof(IServiceProvider) || serviceType == typeof(IKeyedServiceProvider)
        || serviceType == typeof(IServiceScopeFactory) || serviceType == typeof(IServiceProviderIsService)
        || serviceType == typeof(IServiceProviderIsKeyedService);

    private static bool TryGetEnumerableElementType(Type serviceType, [NotNullWhen(true)] out Type? elementType)
    {
        elementType = serviceType.IsConstructedGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
            ? serviceType.GetGenericArguments()[0]
            : null;
        return elementType is not null;
    }

    [UnconditionalSuppressMessage("AOT", "IL3050",
        Justification = "Services are reference types; arrays of reference types share canonical code under Native AOT.")]
    private static Array ToTypedArray(Type elementType, IReadOnlyList<object> items)
    {
        var array = Array.CreateInstance(elementType, items.Count);
        for (var i = 0; i < items.Count; i++)
            array.SetValue(items[i], i);
        return array;
    }

    #endregion

    #region Scopes and disposal tracking

    private Container Attach(Container child)
    {
        lock (_lock)
        {
            ThrowIfDisposed();
            _children.Add(child);
        }
        return child;
    }

    internal object GetOrAddScoped(IRegistration registration, Func<object> create)
    {
        ThrowIfDisposed();
        var key = (Registration)registration;
        lock (_scopedLock)
        {
            // Rechecked under the lock that disposal uses to clear scoped instances.
            ThrowIfDisposed();
            if (!_scoped.TryGetValue(key, out var instance))
                _scoped[key] = instance = create();
            return instance;
        }
    }

    internal void Track(object instance, bool weak)
    {
        lock (_lock)
        {
            if (Volatile.Read(ref _disposed) != 0)
            {
                Disposal.Dispose(instance);
                throw new ObjectDisposedException(nameof(Container));
            }
            if (weak && _disposables.Count >= _pruneAt)
            {
                _disposables.RemoveAll(o => o is WeakReference<object> w && !w.TryGetTarget(out _));
                _pruneAt = Math.Max(PruneThreshold, _disposables.Count * 2);
            }
            _disposables.Add(weak ? new WeakReference<object>(instance) : instance);
        }
    }

    internal bool Untrack(object instance)
    {
        lock (_lock)
        {
            var index = _disposables.FindLastIndex(o =>
                ReferenceEquals(o, instance) || (o is WeakReference<object> w && w.TryGetTarget(out var t) && ReferenceEquals(t, instance)));
            if (index < 0)
                return false;
            _disposables.RemoveAt(index);
            return true;
        }
    }

    public void Dispose()
    {
        if (!BeginDispose(out var children, out var disposables, out var registrations))
            return;
        for (var i = children.Length - 1; i >= 0; i--)
            children[i].Dispose();
        for (var i = disposables.Length - 1; i >= 0; i--)
            Disposal.Dispose(disposables[i]);
        Release(registrations);
    }

    public async ValueTask DisposeAsync()
    {
        if (!BeginDispose(out var children, out var disposables, out var registrations))
            return;
        for (var i = children.Length - 1; i >= 0; i--)
            await children[i].DisposeAsync().ConfigureAwait(false);
        for (var i = disposables.Length - 1; i >= 0; i--)
            await Disposal.DisposeAsync(disposables[i]).ConfigureAwait(false);
        Release(registrations);
    }

    private bool BeginDispose(out Container[] children, out object[] disposables, out Registration[] registrations)
    {
        children = [];
        disposables = [];
        registrations = [];
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return false;

        if (_parent is not null)
        {
            lock (_parent._lock)
            {
                _parent._children.Remove(this);
            }
        }
        lock (_lock)
        {
            children = [.. _children];
            _children.Clear();
            disposables = [.. _disposables.Select(o => o is WeakReference<object> w ? (w.TryGetTarget(out var t) ? t : null) : o).OfType<object>()];
            _disposables.Clear();
            registrations = [.. _registrations.Values.SelectMany(r => r)];
            _registrations.Clear();
        }
        lock (_scopedLock)
        {
            _scoped.Clear();
        }
        return true;
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);

    private sealed class ScopeFactory(Container root) : IServiceScopeFactory
    {
        // Scopes created through IServiceScopeFactory are flat: all are children of the container that created the factory.
        public IServiceScope CreateScope() => root.Attach(new Container(root, root));
    }

    #endregion

    #region Validation

    public void Validate()
    {
        var errors = new List<Exception>();
        var verified = new HashSet<(Registration, bool)>();
        for (var c = this; c is not null; c = c._parent)
        {
            foreach (var registration in c._registrations.Values.SelectMany(r => r))
            {
                try
                {
                    Verify(registration, [], Lifetimes.IsLongLived(registration.Lifetime), registration, verified);
                }
                catch (InvalidOperationException ex)
                {
                    errors.Add(ex);
                }
            }
        }
        if (errors.Count > 0)
            throw new AggregateException("One or more registrations are invalid.", errors);
    }

    private void Verify(Registration registration, List<Registration> path, bool longLived, Registration root,
        HashSet<(Registration, bool)> verified)
    {
        if (path.Contains(registration))
        {
            var chain = string.Join(" -> ", path.Append(registration).Select(r => r.ServiceType.Name));
            throw new InvalidOperationException($"Circular dependency detected. [{chain}]");
        }
        if (registration.IsOpenGeneric || registration.Activator is null || !verified.Add((registration, longLived)))
            return;

        path.Add(registration);
        foreach (var d in registration.Activator.GetDependencies(this))
        {
            if (d.IsServiceKey)
                continue;
            var dependencies = TryGetEnumerableElementType(d.Type, out var elementType)
                ? FindAll(elementType, d.Key)
                : FindSingle(d.Type, d.Key) is { } single ? [single] : [];
            foreach (var dependency in dependencies)
            {
                if (longLived && dependency.Lifetime == Lifetimes.Scoped)
                    throw new InvalidOperationException($"[{root}] cannot depend on scoped service [{dependency}].");
                var childLongLived = dependency.Lifetime == Lifetimes.Multiple ? longLived : Lifetimes.IsLongLived(dependency.Lifetime);
                Verify(dependency, path, childLongLived, root, verified);
            }
        }
        path.RemoveAt(path.Count - 1);
    }

    #endregion

    private readonly record struct ServiceIdentifier(Type ServiceType, object? Key);
}

using System.Diagnostics.CodeAnalysis;

namespace Yocto;

internal sealed class Registration : IRegistration
{
    private static long s_sequence;

    [ThreadStatic]
    private static List<Registration>? t_activating;

    private readonly Func<IContainer, object?, object>? _factory;
    private readonly ConstructorActivator? _activator;
    private readonly Lock _lock = new();
    private IInstanceFactory _instanceFactory;
    private string _lifetime;
    private bool _released;
    private object?[] _options;

    public Registration(Container owner, Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type? implementationType,
        Func<IContainer, object?, object>? factory, object? serviceKey, string lifetime = Lifetimes.Multiple,
        object?[]? options = null, long? sequence = null)
    {
        Owner = owner;
        ServiceType = serviceType;
        ImplementationType = implementationType;
        ServiceKey = serviceKey;
        Sequence = sequence ?? Interlocked.Increment(ref s_sequence);
        _factory = factory;
        if (implementationType is not null && !implementationType.ContainsGenericParameters)
            _activator = new ConstructorActivator(this, implementationType);
        _lifetime = lifetime;
        _options = options ?? [];
        _instanceFactory = Lifetimes.Get(lifetime).CreateInstanceFactory(new LifetimeContext(this), _options);
    }

    public Container Owner { get; }

    public IContainer Container => Owner;

    public Type ServiceType { get; }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public Type? ImplementationType { get; }

    public object? ServiceKey { get; }

    /// <summary>Global registration order, shared by registrations derived from this one (closed generics, any-key).</summary>
    public long Sequence { get; }

    public bool IsOpenGeneric => ServiceType.IsGenericTypeDefinition;

    public bool IsDerivedFromOpenGeneric { get; private init; }

    public string Lifetime => _lifetime;

    internal ConstructorActivator? Activator => _activator;

    internal Func<IContainer, object?, object>? Factory => _factory;

    public object GetInstance(IContainer scope) => Volatile.Read(ref _instanceFactory).GetInstance(scope);

    /// <summary>Creates a new instance with dependencies from <paramref name="scope"/>, detecting circular dependencies.</summary>
    public object Activate(IContainer scope)
    {
        var activating = t_activating ??= [];
        if (activating.Contains(this))
        {
            var chain = string.Join(" -> ", activating.Append(this).Select(r => r.ServiceType.Name));
            throw new InvalidOperationException($"Circular dependency detected. [{chain}]");
        }

        activating.Add(this);
        try
        {
            if (_activator is not null)
                return _activator.Create(scope);

            var instance = _factory!(scope, ServiceKey)
                ?? throw new InvalidOperationException($"Factory returned null. [{ServiceType.Name}]");
            return ServiceType.IsInstanceOfType(instance)
                ? instance
                : throw new InvalidOperationException($"Factory returned [{instance.GetType().Name}], which is not a [{ServiceType.Name}].");
        }
        finally
        {
            activating.RemoveAt(activating.Count - 1);
        }
    }

    /// <summary>Creates a registration for a closed generic service or a concrete key, inheriting lifetime and order.</summary>
    [UnconditionalSuppressMessage("Trimming", "IL2055", Justification = "Constructors of open generic implementations are preserved by the registration's annotation.")]
    [UnconditionalSuppressMessage("Trimming", "IL2068", Justification = "See above.")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Generic arguments are validated as reference types when dynamic code is unsupported.")]
    public Registration Derive(Type serviceType, object? serviceKey)
    {
        var implementationType = ImplementationType;
        if (IsOpenGeneric && implementationType is not null)
            implementationType = implementationType.MakeGenericType(serviceType.GetGenericArguments());
        return new Registration(Owner, serviceType, implementationType, _factory, serviceKey, _lifetime, _options, Sequence)
        {
            IsDerivedFromOpenGeneric = IsOpenGeneric || IsDerivedFromOpenGeneric,
        };
    }

    public IRegistration WithLifetime(string lifetime, params object?[] options)
    {
        options ??= [];
        var factory = Lifetimes.Get(lifetime).CreateInstanceFactory(new LifetimeContext(this), options);
        IInstanceFactory previous;
        lock (_lock)
        {
            if (_released)
            {
                // Removed or replaced concurrently: nothing would ever release the new factory.
                previous = factory;
            }
            else
            {
                previous = _instanceFactory;
                _lifetime = lifetime;
                _options = options;
                Volatile.Write(ref _instanceFactory, factory);
            }
        }
        previous.Dispose();
        if (ReferenceEquals(previous, factory))
            return this;
        Owner.OnLifetimeChanged(this);
        Owner.OnRegistrationsChanged();
        return this;
    }

    public void Remove() => Owner.Remove(this);

    /// <summary>Releases instances owned by the current lifetime.</summary>
    public void Release()
    {
        IInstanceFactory factory;
        lock (_lock)
        {
            _released = true;
            factory = _instanceFactory;
        }
        factory.Dispose();
    }

    public override string ToString() =>
        $"{ServiceType.Name}{(ServiceKey is null ? "" : $" [{ServiceKey}]")} -> {ImplementationType?.Name ?? "factory"} ({_lifetime})";
}

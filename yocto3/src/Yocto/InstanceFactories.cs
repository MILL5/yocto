namespace Yocto;

internal static class Disposal
{
    // Disposal is best effort: one failing instance must not prevent the rest from being released.
    public static void Dispose(object instance)
    {
        try
        {
            if (instance is IDisposable d)
                d.Dispose();
            else if (instance is IAsyncDisposable ad)
                ad.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
        catch
        {
        }
    }

    public static async ValueTask DisposeAsync(object instance)
    {
        try
        {
            if (instance is IAsyncDisposable ad)
                await ad.DisposeAsync().ConfigureAwait(false);
            else if (instance is IDisposable d)
                d.Dispose();
        }
        catch
        {
        }
    }

    public static bool IsDisposable(object instance) => instance is IDisposable or IAsyncDisposable;
}

internal sealed class MultiInstanceFactory(LifetimeContext context) : IInstanceFactory
{
    public object GetInstance(IContainer scope)
    {
        var instance = context.Activate(scope);
        if (Disposal.IsDisposable(instance))
            context.TrackForDisposal(scope, instance, weak: true);
        return instance;
    }

    public void Dispose()
    {
    }
}

/// <summary>Base for lifetimes whose instances belong to the owning container rather than the resolving scope.</summary>
internal abstract class OwnedInstanceFactory(LifetimeContext context) : IInstanceFactory
{
    private int _disposed;

    protected LifetimeContext Context => context;

    public abstract object GetInstance(IContainer scope);

    protected object Create()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        var instance = context.Activate(context.Owner);
        if (Disposal.IsDisposable(instance))
        {
            context.TrackForDisposal(context.Owner, instance);
            // Disposed while activating: Dispose may have missed this instance. Whoever untracks it disposes it.
            if (Volatile.Read(ref _disposed) != 0)
            {
                if (context.Untrack(context.Owner, instance))
                    Disposal.Dispose(instance);
                throw new ObjectDisposedException(GetType().Name);
            }
        }
        return instance;
    }

    protected abstract IEnumerable<object> TakeInstances();

    /// <summary>Called when the registration is replaced or removed: releases instances the owner still tracks.</summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;
        foreach (var instance in TakeInstances())
        {
            if (context.Untrack(context.Owner, instance))
                Disposal.Dispose(instance);
        }
    }
}

internal sealed class SingletonInstanceFactory : OwnedInstanceFactory
{
    private readonly Lazy<object> _instance;

    public SingletonInstanceFactory(LifetimeContext context, bool eagerLoad) : base(context)
    {
        _instance = new Lazy<object>(Create, LazyThreadSafetyMode.ExecutionAndPublication);
        if (eagerLoad)
            _ = _instance.Value;
    }

    public override object GetInstance(IContainer scope) => _instance.Value;

    protected override IEnumerable<object> TakeInstances() => _instance.IsValueCreated ? [_instance.Value] : [];
}

internal sealed class PerThreadInstanceFactory : OwnedInstanceFactory
{
    private readonly ThreadLocal<object> _instance;

    public PerThreadInstanceFactory(LifetimeContext context) : base(context) =>
        _instance = new ThreadLocal<object>(Create, trackAllValues: true);

    public override object GetInstance(IContainer scope) => _instance.Value!;

    protected override IEnumerable<object> TakeInstances()
    {
        var values = _instance.Values.ToArray();
        _instance.Dispose();
        return values;
    }
}

internal sealed class PooledInstanceFactory : OwnedInstanceFactory
{
    public const int MinimumPoolSize = 2;
    public static int DefaultPoolSize => Math.Max(Environment.ProcessorCount, 8);

    private readonly object?[] _pool;
    private readonly Lock _growLock = new();
    private uint _next = uint.MaxValue;

    public PooledInstanceFactory(LifetimeContext context, int poolSize) : base(context)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(poolSize, MinimumPoolSize);
        _pool = new object?[poolSize];
    }

    public int PoolSize => _pool.Length;

    public override object GetInstance(IContainer scope)
    {
        var index = (int)(Interlocked.Increment(ref _next) % (uint)_pool.Length);
        if (Volatile.Read(ref _pool[index]) is { } pooled)
            return pooled;

        lock (_growLock)
        {
            return _pool[index] ??= Create();
        }
    }

    protected override IEnumerable<object> TakeInstances()
    {
        lock (_growLock)
        {
            var owned = _pool.OfType<object>().ToArray();
            Array.Clear(_pool);
            return owned;
        }
    }
}

internal sealed class ScopedInstanceFactory(LifetimeContext context) : IInstanceFactory
{
    public object GetInstance(IContainer scope) =>
        ((Container)scope).GetOrAddScoped(context.Registration, () =>
        {
            var instance = context.Activate(scope);
            if (Disposal.IsDisposable(instance))
                context.TrackForDisposal(scope, instance);
            return instance;
        });

    public void Dispose()
    {
    }
}

internal sealed class ExternalInstanceFactory(LifetimeContext context) : IInstanceFactory
{
    private readonly Lazy<object> _instance = new(() => context.Activate(context.Owner), LazyThreadSafetyMode.ExecutionAndPublication);

    public object GetInstance(IContainer scope) => _instance.Value;

    public void Dispose()
    {
    }
}

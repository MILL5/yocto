using System.Diagnostics.CodeAnalysis;

namespace Yocto;

/// <summary>
/// Collects registrations and hands back a container exactly once. The container is a child of <c>parent</c> when one
/// is supplied, otherwise a new root. Disposing an unbuilt builder disposes its container.
/// </summary>
public sealed class ContainerBuilder(IContainer? parent = null) : IRegistrar, IDisposable
{
    private IContainer? _container = parent?.CreateChildContainer() ?? new Container();

    /// <summary>Run <see cref="IContainer.Validate"/> in <see cref="Build"/>.</summary>
    public bool ValidateOnBuild { get; init; }

    public IRegistration Register(Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType,
        object? serviceKey = null) =>
        Pending.Register(serviceType, implementationType, serviceKey);

    public IRegistration Register(Type serviceType, Func<IContainer, object?, object> factory, object? serviceKey = null) =>
        Pending.Register(serviceType, factory, serviceKey);

    public IRegistration RegisterEnumerable(Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType,
        object? serviceKey = null) =>
        Pending.RegisterEnumerable(serviceType, implementationType, serviceKey);

    public IRegistration RegisterEnumerable(Type serviceType, Func<IContainer, object?, object> factory, object? serviceKey = null) =>
        Pending.RegisterEnumerable(serviceType, factory, serviceKey);

    public IContainer Build()
    {
        var container = Interlocked.Exchange(ref _container, null) ?? throw new InvalidOperationException("Container already built.");
        if (ValidateOnBuild)
            container.Validate();
        return container;
    }

    public void Dispose() => Interlocked.Exchange(ref _container, null)?.Dispose();

    private IContainer Pending => Volatile.Read(ref _container) ?? throw new InvalidOperationException("Container already built.");
}

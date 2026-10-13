using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Yocto;

/// <summary>Registers services. Implemented by <see cref="IContainer"/> and <see cref="ContainerBuilder"/>.</summary>
/// <remarks>
/// <c>Register</c> replaces existing registrations for the same service type and key; <c>RegisterEnumerable</c> appends.
/// Single resolves return the last registration; <c>ResolveAll</c> and <c>IEnumerable&lt;T&gt;</c> return all of them in order.
/// The implementation type may be an open generic (e.g. <c>typeof(Repository&lt;&gt;)</c>).
/// </remarks>
public interface IRegistrar
{
    IRegistration Register(Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType,
        object? serviceKey = null);

    /// <param name="factory">Receives the resolving container and the requested service key.</param>
    IRegistration Register(Type serviceType, Func<IContainer, object?, object> factory, object? serviceKey = null);

    IRegistration RegisterEnumerable(Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType,
        object? serviceKey = null);

    /// <param name="factory">Receives the resolving container and the requested service key.</param>
    IRegistration RegisterEnumerable(Type serviceType, Func<IContainer, object?, object> factory, object? serviceKey = null);
}

/// <summary>
/// An IoC container. Every container is also a scope: <see cref="Lifetimes.Scoped"/> services get one instance per
/// container, and child containers fall back to their parent for registrations.
/// </summary>
public interface IContainer : IRegistrar, IKeyedServiceProvider, IDisposable, IAsyncDisposable
{
    /// <summary>The parent container, or <c>null</c> for a root container.</summary>
    IContainer? Parent { get; }

    /// <summary>Creates a child container (a scope). Disposing the parent disposes its children.</summary>
    IContainer CreateChildContainer();

    bool CanResolve(Type serviceType, object? serviceKey = null);

    /// <exception cref="InvalidOperationException">The service is not registered or cannot be constructed.</exception>
    object Resolve(Type serviceType, object? serviceKey = null);

    bool TryResolve(Type serviceType, [NotNullWhen(true)] out object? instance, object? serviceKey = null);

    /// <summary>Resolves every registration for <paramref name="serviceType"/>, in registration order.</summary>
    IReadOnlyList<object> ResolveAll(Type serviceType, object? serviceKey = null);

    /// <summary>Removes this container's registrations for <paramref name="serviceType"/>.</summary>
    bool Unregister(Type serviceType, object? serviceKey = null);

    /// <summary>
    /// Checks every registration can be constructed, has no circular dependencies, and that no singleton,
    /// pooled or per-thread service captures a scoped one.
    /// </summary>
    /// <exception cref="AggregateException">One <see cref="InvalidOperationException"/> per invalid registration.</exception>
    void Validate();
}

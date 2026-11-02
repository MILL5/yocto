using System.Diagnostics.CodeAnalysis;

namespace Yocto;

/// <summary>A single service registration. Use the lifetime extensions (<c>AsSingleton</c>, <c>AsScoped</c>, …) to configure it.</summary>
public interface IRegistration
{
    /// <summary>The container that owns this registration.</summary>
    IContainer Container { get; }

    Type ServiceType { get; }

    /// <summary>The implementation type, or <c>null</c> for factory and instance registrations.</summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    Type? ImplementationType { get; }

    object? ServiceKey { get; }

    /// <summary>The name of the current lifetime. See <see cref="Lifetimes"/>.</summary>
    string Lifetime { get; }

    /// <summary>Switches this registration to the named lifetime, disposing instances owned by the previous one.</summary>
    IRegistration WithLifetime(string lifetime, params object?[] options);

    /// <summary>Removes this registration from its container.</summary>
    void Remove();
}

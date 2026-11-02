using System.Diagnostics.CodeAnalysis;

namespace Yocto;

/// <summary>Strongly typed registration helpers.</summary>
public static class RegistrarExtensions
{
    private const DynamicallyAccessedMemberTypes Ctors = DynamicallyAccessedMemberTypes.PublicConstructors;

    public static IRegistration Register<TService, [DynamicallyAccessedMembers(Ctors)] TImplementation>(this IRegistrar registrar, object? serviceKey = null)
        where TService : class where TImplementation : class, TService =>
        Checked(registrar).Register(typeof(TService), typeof(TImplementation), serviceKey);

    /// <summary>Registers a concrete type as itself.</summary>
    public static IRegistration Register<[DynamicallyAccessedMembers(Ctors)] TService>(this IRegistrar registrar, object? serviceKey = null)
        where TService : class =>
        Checked(registrar).Register(typeof(TService), typeof(TService), serviceKey);

    public static IRegistration Register<TService>(this IRegistrar registrar, Func<TService> factory, object? serviceKey = null)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(factory);
        return Checked(registrar).Register(typeof(TService), (_, _) => factory(), serviceKey);
    }

    public static IRegistration Register<TService>(this IRegistrar registrar, Func<IContainer, TService> factory, object? serviceKey = null)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(factory);
        return Checked(registrar).Register(typeof(TService), (c, _) => factory(c), serviceKey);
    }

    /// <summary>Registers an existing instance. The container never disposes it.</summary>
    public static IRegistration RegisterInstance<TService>(this IRegistrar registrar, TService instance, object? serviceKey = null)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(instance);
        return Checked(registrar).Register(typeof(TService), (_, _) => instance, serviceKey).WithLifetime(Lifetimes.Instance);
    }

    public static IRegistration RegisterSingleton<TService, [DynamicallyAccessedMembers(Ctors)] TImplementation>(this IRegistrar registrar, bool eagerLoad = false)
        where TService : class where TImplementation : class, TService =>
        registrar.Register<TService, TImplementation>().AsSingleton(eagerLoad);

    public static IRegistration RegisterSingleton<TService>(this IRegistrar registrar, Func<IContainer, TService> factory, bool eagerLoad = false)
        where TService : class =>
        registrar.Register(factory).AsSingleton(eagerLoad);

    public static IRegistration RegisterScoped<TService, [DynamicallyAccessedMembers(Ctors)] TImplementation>(this IRegistrar registrar)
        where TService : class where TImplementation : class, TService =>
        registrar.Register<TService, TImplementation>().AsScoped();

    public static IRegistration RegisterScoped<TService>(this IRegistrar registrar, Func<IContainer, TService> factory)
        where TService : class =>
        registrar.Register(factory).AsScoped();

    public static IRegistration RegisterPerThread<TService, [DynamicallyAccessedMembers(Ctors)] TImplementation>(this IRegistrar registrar)
        where TService : class where TImplementation : class, TService =>
        registrar.Register<TService, TImplementation>().AsPerThread();

    public static IRegistration RegisterPerThread<TService>(this IRegistrar registrar, Func<IContainer, TService> factory)
        where TService : class =>
        registrar.Register(factory).AsPerThread();

    public static IRegistration RegisterPooled<TService, [DynamicallyAccessedMembers(Ctors)] TImplementation>(this IRegistrar registrar, int? poolSize = null)
        where TService : class where TImplementation : class, TService =>
        registrar.Register<TService, TImplementation>().AsPooled(poolSize);

    public static IRegistration RegisterPooled<TService>(this IRegistrar registrar, Func<IContainer, TService> factory, int? poolSize = null)
        where TService : class =>
        registrar.Register(factory).AsPooled(poolSize);

    /// <summary>Adds an implementation to the set resolved by <c>ResolveAll&lt;TService&gt;()</c> or an <c>IEnumerable&lt;TService&gt;</c> dependency.</summary>
    public static IRegistration RegisterEnumerable<TService, [DynamicallyAccessedMembers(Ctors)] TImplementation>(this IRegistrar registrar, object? serviceKey = null)
        where TService : class where TImplementation : class, TService =>
        Checked(registrar).RegisterEnumerable(typeof(TService), typeof(TImplementation), serviceKey);

    public static IRegistration RegisterEnumerable<TService>(this IRegistrar registrar, Func<IContainer, TService> factory, object? serviceKey = null)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(factory);
        return Checked(registrar).RegisterEnumerable(typeof(TService), (c, _) => factory(c), serviceKey);
    }

    /// <summary>Applies a module's registrations.</summary>
    public static IRegistrar RegisterModule<TModule>(this IRegistrar registrar) where TModule : IModule, new() =>
        registrar.RegisterModule(new TModule());

    public static IRegistrar RegisterModule(this IRegistrar registrar, IModule module)
    {
        ArgumentNullException.ThrowIfNull(module);
        module.Register(Checked(registrar));
        return registrar;
    }

    private static IRegistrar Checked(IRegistrar registrar)
    {
        ArgumentNullException.ThrowIfNull(registrar);
        return registrar;
    }
}

/// <summary>Fluent lifetime configuration for a registration.</summary>
public static class RegistrationExtensions
{
    /// <summary>A new instance on every resolve (the default).</summary>
    public static IRegistration AsMultiple(this IRegistration registration) =>
        Checked(registration).WithLifetime(Lifetimes.Multiple);

    public static IRegistration AsSingleton(this IRegistration registration, bool eagerLoad = false) =>
        Checked(registration).WithLifetime(Lifetimes.Singleton, eagerLoad);

    public static IRegistration AsScoped(this IRegistration registration) =>
        Checked(registration).WithLifetime(Lifetimes.Scoped);

    public static IRegistration AsPerThread(this IRegistration registration) =>
        Checked(registration).WithLifetime(Lifetimes.PerThread);

    public static IRegistration AsPooled(this IRegistration registration, int? poolSize = null) =>
        poolSize is { } size
            ? Checked(registration).WithLifetime(Lifetimes.Pooled, size)
            : Checked(registration).WithLifetime(Lifetimes.Pooled);

    private static IRegistration Checked(IRegistration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        return registration;
    }
}

/// <summary>Strongly typed resolution helpers.</summary>
public static class ContainerExtensions
{
    public static T Resolve<T>(this IContainer container, object? serviceKey = null) where T : class
    {
        ArgumentNullException.ThrowIfNull(container);
        return (T)container.Resolve(typeof(T), serviceKey);
    }

    public static bool TryResolve<T>(this IContainer container, [NotNullWhen(true)] out T? instance, object? serviceKey = null) where T : class
    {
        ArgumentNullException.ThrowIfNull(container);
        instance = container.TryResolve(typeof(T), out var o, serviceKey) ? (T)o : null;
        return instance is not null;
    }

    public static bool CanResolve<T>(this IContainer container, object? serviceKey = null) where T : class
    {
        ArgumentNullException.ThrowIfNull(container);
        return container.CanResolve(typeof(T), serviceKey);
    }

    public static IReadOnlyList<T> ResolveAll<T>(this IContainer container, object? serviceKey = null) where T : class
    {
        ArgumentNullException.ThrowIfNull(container);
        return [.. container.ResolveAll(typeof(T), serviceKey).Cast<T>()];
    }

    public static bool Unregister<T>(this IContainer container, object? serviceKey = null) where T : class
    {
        ArgumentNullException.ThrowIfNull(container);
        return container.Unregister(typeof(T), serviceKey);
    }
}

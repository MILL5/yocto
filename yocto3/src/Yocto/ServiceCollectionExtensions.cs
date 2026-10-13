using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Yocto;

/// <summary>Microsoft.Extensions.DependencyInjection integration.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds every descriptor in <paramref name="services"/> to <paramref name="registrar"/>, preserving order and lifetimes.</summary>
    [UnconditionalSuppressMessage("Trimming", "IL2072",
        Justification = "ServiceDescriptor.ImplementationType is annotated with PublicConstructors by Microsoft.Extensions.DependencyInjection.")]
    public static IRegistrar Populate(this IRegistrar registrar, IEnumerable<ServiceDescriptor> services)
    {
        ArgumentNullException.ThrowIfNull(registrar);
        ArgumentNullException.ThrowIfNull(services);

        foreach (var d in services)
        {
            var key = d.ServiceKey;
            IRegistration registration;
            if (d.IsKeyedService)
            {
                registration = d.KeyedImplementationInstance is { } keyedInstance
                    ? registrar.RegisterEnumerable(d.ServiceType, (_, _) => keyedInstance, key).WithLifetime(Lifetimes.Instance)
                    : d.KeyedImplementationFactory is { } keyedFactory
                        ? registrar.RegisterEnumerable(d.ServiceType, (c, k) => keyedFactory(c, k)!, key)
                        : registrar.RegisterEnumerable(d.ServiceType, d.KeyedImplementationType!, key);
            }
            else
            {
                registration = d.ImplementationInstance is { } instance
                    ? registrar.RegisterEnumerable(d.ServiceType, (_, _) => instance).WithLifetime(Lifetimes.Instance)
                    : d.ImplementationFactory is { } factory
                        ? registrar.RegisterEnumerable(d.ServiceType, (c, _) => factory(c))
                        : registrar.RegisterEnumerable(d.ServiceType, d.ImplementationType!);
            }

            if (registration.Lifetime != Lifetimes.Instance)
            {
                _ = d.Lifetime switch
                {
                    ServiceLifetime.Singleton => registration.AsSingleton(),
                    ServiceLifetime.Scoped => registration.AsScoped(),
                    _ => registration,
                };
            }
        }
        return registrar;
    }

    /// <summary>Builds a yocto <see cref="Container"/> from <paramref name="services"/>.</summary>
    public static Container BuildYoctoServiceProvider(this IServiceCollection services, bool validateOnBuild = false)
    {
        var container = new Container();
        container.Populate(services);
        if (validateOnBuild)
            container.Validate();
        return container;
    }
}

/// <summary>
/// Plugs yocto into the .NET Generic Host / ASP.NET Core:
/// <c>builder.Host.UseServiceProviderFactory(new YoctoServiceProviderFactory());</c>
/// Use <c>builder.Host.ConfigureContainer&lt;IContainer&gt;(c =&gt; ...)</c> for yocto-specific registrations.
/// </summary>
public sealed class YoctoServiceProviderFactory(bool validateOnBuild = false) : IServiceProviderFactory<IContainer>
{
    public IContainer CreateBuilder(IServiceCollection services)
    {
        var container = new Container();
        container.Populate(services);
        return container;
    }

    public IServiceProvider CreateServiceProvider(IContainer containerBuilder)
    {
        ArgumentNullException.ThrowIfNull(containerBuilder);
        if (validateOnBuild)
            containerBuilder.Validate();
        return containerBuilder;
    }
}

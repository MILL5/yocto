using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Yocto;

/// <summary>
/// Constructor injection. On first use, picks the public constructor with the most parameters that can all be
/// satisfied; every other satisfiable constructor's parameters must be a subset of it, otherwise it is ambiguous.
/// </summary>
internal sealed class ConstructorActivator
{
    private readonly Registration _registration;

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    private readonly Type _implementationType;

    private Plan? _plan;

    public ConstructorActivator(Registration registration,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType)
    {
        _registration = registration;
        _implementationType = implementationType;
    }

    public object Create(IContainer scope)
    {
        var plan = _plan ??= SelectConstructor(scope);
        var args = new object?[plan.Dependencies.Length];
        for (var i = 0; i < args.Length; i++)
        {
            var d = plan.Dependencies[i];
            args[i] = d.IsServiceKey ? ServiceKeyFor(d)
                : scope.TryResolve(d.Type, out var value, d.Key) ? value
                : d.HasDefault ? d.Default
                : throw new InvalidOperationException($"Unable to resolve [{d.Type.Name}] while activating [{_implementationType.Name}].");
        }
        return plan.Invoker.Invoke(args.AsSpan())!;
    }

    private object? ServiceKeyFor(Dependency d)
    {
        var key = _registration.ServiceKey;
        return key is null ? (d.Type.IsValueType ? throw KeyMismatch(d) : null)
            : d.Type.IsInstanceOfType(key) ? key
            : throw KeyMismatch(d);
    }

    private InvalidOperationException KeyMismatch(Dependency d) =>
        new($"The service key [{_registration.ServiceKey ?? "null"}] cannot be injected as [{d.Type.Name}] into [{_implementationType.Name}].");

    /// <summary>The dependencies of the selected constructor, for validation.</summary>
    public IReadOnlyList<Dependency> GetDependencies(IContainer scope) => (_plan ??= SelectConstructor(scope)).Dependencies;

    private Plan SelectConstructor(IContainer scope)
    {
        var satisfiable = _implementationType.GetConstructors()
            .Select(c => (Ctor: c, Deps: c.GetParameters().Select(Describe).ToArray()))
            .Where(c => c.Deps.All(d => d.IsServiceKey || d.HasDefault || scope.CanResolve(d.Type, d.Key)))
            .OrderByDescending(c => c.Deps.Length)
            .ToArray();

        if (satisfiable.Length == 0)
        {
            throw new InvalidOperationException(
                $"Could not find a constructor for [{_implementationType.Name}] whose parameters can all be resolved.");
        }

        var best = satisfiable[0];
        var bestTypes = best.Deps.Select(d => d.Type).ToHashSet();
        foreach (var other in satisfiable.Skip(1))
        {
            if (other.Deps.Length == best.Deps.Length || !other.Deps.All(d => bestTypes.Contains(d.Type)))
            {
                throw new InvalidOperationException(
                    $"Found more than one constructor for [{_implementationType.Name}] that could be used; none is a superset of the others.");
            }
        }

        return new Plan(ConstructorInvoker.Create(best.Ctor), best.Deps);
    }

    private Dependency Describe(ParameterInfo p)
    {
        var isServiceKey = p.GetCustomAttribute<ServiceKeyAttribute>() is not null;
        object? key = null;
        if (p.GetCustomAttribute<FromKeyedServicesAttribute>() is { } keyed)
        {
            key = keyed.LookupMode switch
            {
                ServiceKeyLookupMode.InheritKey => _registration.ServiceKey,
                ServiceKeyLookupMode.NullKey => null,
                _ => keyed.Key,
            };
        }
        return new Dependency(p.ParameterType, key, isServiceKey, p.HasDefaultValue, p.HasDefaultValue ? p.DefaultValue : null);
    }

    internal sealed record Dependency(Type Type, object? Key, bool IsServiceKey, bool HasDefault, object? Default);

    private sealed record Plan(ConstructorInvoker Invoker, Dependency[] Dependencies);
}

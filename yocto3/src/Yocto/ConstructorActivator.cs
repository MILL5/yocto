using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace Yocto;

/// <summary>
/// Constructor injection. Picks the public constructor with the most parameters that can all be satisfied; every other
/// satisfiable constructor's parameters must be a subset of it, otherwise it is ambiguous.
/// </summary>
/// <remarks>
/// The choice is made against the registration's owner, so it does not depend on which container resolves first, and is
/// made again after registrations visible to the owner change. If the owner can satisfy no constructor (a dependency only
/// child containers register), the choice is made against the resolving container and cached per container.
/// </remarks>
internal sealed class ConstructorActivator
{
    private readonly Registration _registration;

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    private readonly Type _implementationType;

    private readonly ConditionalWeakTable<Container, Plan> _scopePlans = [];
    private Candidate[]? _candidates;
    private Plan? _ownerPlan;

    public ConstructorActivator(Registration registration,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType)
    {
        _registration = registration;
        _implementationType = implementationType;
    }

    public object Create(IContainer scope)
    {
        var plan = GetPlan(scope);
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
    public IReadOnlyList<Dependency> GetDependencies(IContainer scope) => GetPlan(scope).Dependencies;

    private Plan GetPlan(IContainer scope)
    {
        var owner = _registration.Owner;
        var stamp = owner.RegistrationStamp;
        if (Volatile.Read(ref _ownerPlan) is { } cached && cached.Stamp == stamp)
            return cached;
        if (SelectConstructor(owner, stamp) is { } plan)
        {
            Volatile.Write(ref _ownerPlan, plan);
            return plan;
        }

        if (scope is Container container && !ReferenceEquals(container, owner))
        {
            var scopeStamp = container.RegistrationStamp;
            if (_scopePlans.TryGetValue(container, out var scoped) && scoped.Stamp == scopeStamp)
                return scoped;
            if (SelectConstructor(container, scopeStamp) is { } scopePlan)
            {
                _scopePlans.AddOrUpdate(container, scopePlan);
                return scopePlan;
            }
        }

        throw new InvalidOperationException(
            $"Could not find a constructor for [{_implementationType.Name}] whose parameters can all be resolved.");
    }

    /// <summary>Returns <c>null</c> when no constructor can be satisfied from <paramref name="scope"/>.</summary>
    private Plan? SelectConstructor(Container scope, long stamp)
    {
        var satisfiable = (_candidates ??= [.. _implementationType.GetConstructors()
                .Select(c => new Candidate(c, [.. c.GetParameters().Select(Describe)]))])
            .Where(c => c.Deps.All(d => d.IsServiceKey || d.HasDefault || scope.CanResolve(d.Type, d.Key)))
            .OrderByDescending(c => c.Deps.Length)
            .ToArray();

        if (satisfiable.Length == 0)
            return null;

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

        return new Plan(best.Invoker, best.Deps, stamp);
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

    private sealed record Plan(ConstructorInvoker Invoker, Dependency[] Dependencies, long Stamp);

    /// <summary>A public constructor and its parameters, described once per implementation type.</summary>
    private sealed class Candidate(ConstructorInfo ctor, Dependency[] deps)
    {
        private ConstructorInvoker? _invoker;

        public Dependency[] Deps => deps;

        public ConstructorInvoker Invoker => _invoker ??= ConstructorInvoker.Create(ctor);
    }
}

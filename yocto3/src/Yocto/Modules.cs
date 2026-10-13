using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Yocto;

/// <summary>A reusable group of registrations, applied with <c>RegisterModule</c>.</summary>
public interface IModule
{
    void Register(IRegistrar registrar);
}

/// <summary>Assembly-level auto registration.</summary>
public static class AutoRegistration
{
    private const string ConventionTypeName = "AssemblyRegistration";
    private const string ConventionMethodName = "Initialize";

    /// <summary>
    /// Applies every public <see cref="IModule"/> with a parameterless constructor in <paramref name="assembly"/>, then
    /// every public static <c>AssemblyRegistration.Initialize(IContainer)</c> method. Returns the number applied.
    /// </summary>
    [RequiresUnreferencedCode("Scans the assembly's exported types with reflection.")]
    public static int RegisterAssembly(this IContainer container, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(assembly);

        var applied = 0;
        foreach (var type in assembly.GetExportedTypes())
        {
            if (type is { IsClass: true, IsAbstract: false } && typeof(IModule).IsAssignableFrom(type)
                && type.GetConstructor(Type.EmptyTypes) is { } ctor)
            {
                container.RegisterModule((IModule)ctor.Invoke(null));
                applied++;
            }
            else if (type.Name == ConventionTypeName
                && type.GetMethod(ConventionMethodName, BindingFlags.Public | BindingFlags.Static, [typeof(IContainer)]) is { } initialize)
            {
                initialize.Invoke(null, [container]);
                applied++;
            }
        }
        return applied;
    }
}

using System.Windows.Markup;

namespace Yocto.Wpf;

/// <summary>
/// Resolves a service from <see cref="Application.Current"/> (or <see cref="Container"/>) in XAML:
/// <c>DataContext="{yocto:Resolve {x:Type vm:MainViewModel}}"</c>.
/// </summary>
[MarkupExtensionReturnType(typeof(object))]
public sealed class ResolveExtension : MarkupExtension
{
    public ResolveExtension()
    {
    }

    public ResolveExtension(Type serviceType) => ServiceType = serviceType;

    [ConstructorArgument("serviceType")]
    public Type? ServiceType { get; set; }

    /// <summary>Optional service key for keyed registrations.</summary>
    public object? Key { get; set; }

    /// <summary>The container to resolve from. Defaults to <see cref="Application.Current"/>.</summary>
    public IContainer? Container { get; set; }

    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        var serviceType = ServiceType ?? throw new InvalidOperationException($"{nameof(ServiceType)} must be set.");
        var container = Container ?? Application.Current;
        return container.TryResolve(serviceType, out var instance, Key) ? instance : null;
    }
}

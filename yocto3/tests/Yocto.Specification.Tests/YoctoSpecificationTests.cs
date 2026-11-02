using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Specification;

namespace Yocto.Specification.Tests;

/// <summary>Runs Microsoft's conformance suite for IServiceProvider implementations against yocto.</summary>
public sealed class YoctoSpecificationTests : DependencyInjectionSpecificationTests
{
    protected override IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection) =>
        serviceCollection.BuildYoctoServiceProvider();
}

/// <summary>Microsoft's keyed-services conformance suite.</summary>
public sealed class YoctoKeyedSpecificationTests : KeyedDependencyInjectionSpecificationTests
{
    protected override IServiceProvider CreateServiceProvider(IServiceCollection collection) =>
        collection.BuildYoctoServiceProvider();
}

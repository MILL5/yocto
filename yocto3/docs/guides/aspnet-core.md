---
title: Use yocto with ASP.NET Core and the .NET Generic Host
description: Replace the default ASP.NET Core service provider with yocto on .NET 10 using YoctoServiceProviderFactory, ConfigureContainer, and BuildYoctoServiceProvider.
breadcrumb: ASP.NET Core and Generic Host
last_reviewed: 2026-10-13
faq:
  - q: Is yocto compatible with Microsoft.Extensions.DependencyInjection?
    a: Yes. yocto passes all 143 tests in Microsoft.Extensions.DependencyInjection.Specification.Tests 10.0 — the conformance suite for third-party containers — including keyed services, open generics, IEnumerable ordering, scopes, and disposal.
---

# ASP.NET Core and Generic Host

## Web apps

```csharp
using Yocto;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();                     // regular IServiceCollection registrations still work
builder.Host.UseServiceProviderFactory(new YoctoServiceProviderFactory(validateOnBuild: true));
builder.Host.ConfigureContainer<IContainer>(c =>
{
    c.RegisterPooled<IEncoder, Encoder>(poolSize: 8);  // yocto-only features
    c.RegisterModule<BillingModule>();
});

var app = builder.Build();
```

Each HTTP request runs in a yocto scope, so `AsScoped()` services are per-request.

## Console and worker apps

```csharp
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.ConfigureContainer(new YoctoServiceProviderFactory(), c => c.Register<IClock, SystemClock>().AsSingleton());
using var host = builder.Build();
await host.RunAsync();
```

## Without a host

```csharp
var services = new ServiceCollection().AddLogging().AddSingleton<IClock, SystemClock>();
await using Container container = services.BuildYoctoServiceProvider(validateOnBuild: true);
```

`container.Populate(services)` adds `IServiceCollection` descriptors to any container or `ContainerBuilder`, preserving order, keys, and lifetimes (`Transient` → Multiple).

{% include faq.html heading="ASP.NET Core FAQ" %}

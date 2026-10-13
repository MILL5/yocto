<img src="https://raw.githubusercontent.com/MILL5/yocto/main/yocto3/docs/assets/yocto.png" alt="yocto logo" width="96" align="right" />

# yocto — the tiny IoC container for .NET 10

[![NuGet](https://img.shields.io/nuget/v/yocto.svg?label=yocto)](https://www.nuget.org/packages/yocto)
[![NuGet downloads](https://img.shields.io/nuget/dt/yocto.svg)](https://www.nuget.org/packages/yocto)
[![CI](https://github.com/MILL5/yocto/actions/workflows/ci.yml/badge.svg)](https://github.com/MILL5/yocto/actions/workflows/ci.yml)
[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4.svg)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![License: Apache 2.0](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](https://github.com/MILL5/yocto/blob/main/LICENSE)

**yocto is a free, open-source inversion-of-control (IoC) and dependency injection container for .NET 10.** It is a drop-in `IServiceProvider` for ASP.NET Core and Microsoft.Extensions.DependencyInjection: it passes all 143 tests in Microsoft's DI specification suite. On top of that it adds child containers, per-thread and pooled lifetimes, modules, and build-time validation, in about 1,200 lines of C#.

📖 **Documentation:** https://mill5.github.io/yocto/

## Install

```bash
dotnet add package yocto
dotnet add package yocto.Wpf   # optional: XAML markup extension
```

## Quick start

```csharp
using Yocto;

using var container = new Container();

container.RegisterSingleton<IClock, SystemClock>();
container.Register<IGreeter, Greeter>();                 // new instance per resolve (default)
container.Register<IGreeter, LoudGreeter>("loud");       // keyed
container.Register(typeof(IRepository<>), typeof(Repository<>)).AsScoped();  // open generic
container.Validate();                                    // fail fast on missing, circular, or captive dependencies

var greeter = container.Resolve<IGreeter>();             // Greeter(IClock clock) is constructor-injected

using var scope = container.CreateChildContainer();     // scoped services: one per child container
var orders = scope.Resolve<IRepository<Order>>();
```

### ASP.NET Core

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new YoctoServiceProviderFactory(validateOnBuild: true));
builder.Host.ConfigureContainer<IContainer>(c => c.RegisterPooled<IEncoder, Encoder>(poolSize: 8));
```

## Features

| Feature | API |
|---|---|
| Constructor injection (greediest satisfiable constructor) | `Register<TService, TImpl>()` |
| Factories and existing instances | `Register<T>(c => …)`, `RegisterInstance(obj)` |
| Lifetimes | `AsMultiple()` (default), `AsSingleton(eagerLoad)`, `AsScoped()`, `AsPerThread()`, `AsPooled(size)` |
| Custom lifetimes | `Lifetimes.Register(name, ILifetimeFactory)`, `WithLifetime(name)` |
| Keyed services | `Register<T, V>(key)`, `Resolve<T>(key)`, `[FromKeyedServices]`, `[ServiceKey]` |
| Open generics | `Register(typeof(IRepo<>), typeof(Repo<>))` |
| Multiple implementations | `RegisterEnumerable<T, V>()`, `ResolveAll<T>()`, inject `IEnumerable<T>` |
| Child containers and scopes | `CreateChildContainer()`, `IServiceScopeFactory` |
| Disposal | `Dispose()` / `DisposeAsync()`, reverse creation order, transients tracked weakly |
| Validation | `Validate()`, `ContainerBuilder { ValidateOnBuild = true }` |
| Modules | `IModule`, `RegisterModule<T>()`, `RegisterAssembly(assembly)` |
| Ambient container | `Application.Current`, `Application.Push()` / `Pop()` |
| ASP.NET Core / Generic Host | `YoctoServiceProviderFactory`, `BuildYoctoServiceProvider()`, `Populate(services)` |
| WPF / XAML | `{yocto:Resolve {x:Type vm:MainViewModel}}` (yocto.Wpf) |
| Native AOT and trimming | `IsAotCompatible`, `[DynamicallyAccessedMembers]` throughout |

## FAQ

**What is yocto?**
A tiny, open-source IoC/DI container for .NET 10 from MILL5. It is named after the SI prefix for 10⁻²⁴.

**Can it replace the ASP.NET Core container?**
Yes. Use `YoctoServiceProviderFactory`. yocto passes Microsoft.Extensions.DependencyInjection.Specification.Tests 10.0, including keyed services, open generics, scopes, and disposal order.

**How does it compare to Autofac or the built-in container?**
The built-in container is enough for many apps. Choose yocto when you also want child containers that override registrations, pooled or per-thread lifetimes, modules, or a container small enough to audit. Autofac offers more, such as decorators and interception, at a much larger size.

**Does it support Native AOT?**
Yes. The library builds with `IsAotCompatible` and trimming annotations. The two exceptions: `RegisterAssembly` uses reflection, and open generics over value types need runtime code generation.

**Which .NET versions?**
yocto 3 targets .NET 10 (LTS). yocto 1.x/2.x for .NET Framework, .NET Standard, and Xamarin are no longer maintained. See the [migration guide](https://mill5.github.io/yocto/guides/migrating-to-v3/).

More answers: https://mill5.github.io/yocto/faq/

## Building from source

```bash
cd yocto3
dotnet build -c Release
dotnet test                       # yocto tests + Microsoft's DI specification suite
dotnet run --project samples/Yocto.Sample
```

## Part of the MILL5 open-source family

[Pineapple](https://mill5.github.io/pineapple/) · [FastSearch](https://mill5.github.io/FastSearch/) · [M5.BloomFilter](https://github.com/MILL5/M5.BloomFilter) · [Polygon.Net](https://github.com/MILL5/Polygon.Net) · [Cut & Move](https://github.com/ChiefInnovator/cutandmove) · [MacEdgeLight](https://github.com/ChiefInnovator/macedgelight)

## Contributing

Issues and pull requests are welcome at https://github.com/MILL5/yocto. Please include tests; `dotnet test` must pass, including the specification suite.

## License

This project is licensed under the [Apache License 2.0](https://github.com/MILL5/yocto/blob/main/LICENSE). © MILL5, LLC.

## Credits

Created by **[Richard Crane](https://mvp.microsoft.com/en-US/MVP/profile/10ce0bc0-7536-43f6-b28c-e9601a4a0d0d)**, Microsoft MVP and founder of **[MILL5](https://mill5.com)**.

- **MILL5:** [mill5.com](https://mill5.com)
- **Microsoft MVP profile:** [mvp.microsoft.com/…/Richard-Crane](https://mvp.microsoft.com/en-US/MVP/profile/10ce0bc0-7536-43f6-b28c-e9601a4a0d0d)
- **Podcast — _Inventing Fire with AI_:** [inventingfirewith.ai](https://inventingfirewith.ai)
- **Support:** [rich@mill5.com](mailto:rich@mill5.com)

---

Powered by [MILL5](https://www.mill5.com)
Explore [Richard Crane on Microsoft MVP](https://mvp.microsoft.com/en-US/MVP/profile/10ce0bc0-7536-43f6-b28c-e9601a4a0d0d)
Listen to the [Inventing Fire with AI](https://inventingfirewith.ai) podcast for insights on technology and innovation.
For support, contact: [rich@mill5.com](mailto:rich@mill5.com)

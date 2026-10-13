---
title: Getting started with yocto
description: Install yocto from NuGet, register services, resolve them with constructor injection, and plug yocto into ASP.NET Core on .NET 10.
---

# Getting started

## 1. Install

```bash
dotnet add package yocto
```

yocto 3 targets **.NET 10**. Its only dependency is `Microsoft.Extensions.DependencyInjection.Abstractions`.

## 2. Register and resolve

```csharp
using Yocto;

using var container = new Container();

container.Register<IAnimal, Dog>();                 // new instance per resolve (default)
container.RegisterSingleton<IClock, SystemClock>(); // one shared instance
container.Register<IPerson, Person>();              // Person(IAnimal pet) is constructor-injected

IPerson person = container.Resolve<IPerson>();
bool known = container.CanResolve<IPerson>();
if (container.TryResolve<IPerson>(out var maybe)) { /* ... */ }
```

yocto picks the public constructor with the most parameters it can satisfy. Parameters with default values are optional.

## 3. Choose a lifetime

```csharp
container.Register<IRepository, SqlRepository>().AsScoped();   // one per container/scope
container.Register<IParser, Parser>().AsPerThread();            // one per thread
container.Register<IEncoder, Encoder>().AsPooled(poolSize: 4);  // round-robin pool
container.RegisterInstance<IConfig>(config);                    // yours; never disposed by yocto
```

See [Lifetimes]({{ '/guides/lifetimes/' | relative_url }}) for details.

## 4. Validate at startup

```csharp
container.Validate(); // throws AggregateException listing every broken registration
```

## 5. Use it in ASP.NET Core

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new YoctoServiceProviderFactory(validateOnBuild: true));
builder.Host.ConfigureContainer<IContainer>(c => c.RegisterPooled<IEncoder, Encoder>(poolSize: 8));
```

See [ASP.NET Core and Generic Host]({{ '/guides/aspnet-core/' | relative_url }}).

## 6. Dispose

Disposing a container disposes its child containers and every `IDisposable`/`IAsyncDisposable` instance it created, newest first. Prefer `await using` when services implement `IAsyncDisposable`.

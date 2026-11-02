---
title: Migrating to yocto 3
description: Upgrade from yocto 1.x or 2.x to yocto 3 on .NET 10 — namespace, API, lifetime, and disposal changes.
breadcrumb: Migrating to yocto 3
last_reviewed: 2026-10-13
---

# Migrating to yocto 3

yocto 3 is a ground-up rewrite for .NET 10. Every 1.x/2.x feature is still there; the API is modernized.

| yocto 1.x / 2.x | yocto 3 |
|---|---|
| `namespace yocto` | `namespace Yocto` |
| .NET Framework 4.0/4.7.2, .NET Standard 1.0/1.1/2.0, Xamarin iOS | .NET 10 |
| `Application.Current.GetChildContainer()` | `container.CreateChildContainer()` (returns `IContainer`) |
| `IChildContainer` | `IContainer` (every container is disposable and has `Parent`) |
| `new Container()` was internal; use `Application.Current` | `new Container()` |
| Register<T, V>() needed dependencies registered first | Constructors are chosen at resolve time from current registrations; registration order doesn't matter |
| Ambiguous constructor = any two satisfiable | Greediest satisfiable constructor wins, if it's a superset of the others |
| `Instancing.MultiInstance`, `SingletonInstance`, … | `Lifetimes.Multiple`, `Singleton`, `Scoped`, `PerThread`, `Pooled`, `Instance` |
| `registration.Register(lifetime, values)` | `registration.WithLifetime(lifetime, options)` |
| `Lifetimes.RegisterLifetimeFactory` / `GetLifetimeFactory` | `Lifetimes.Register` / `Lifetimes.Get` |
| `ILifetimeFactory.GetInstanceFactory(container, interfaceType, implementationType, factory, values)` | `ILifetimeFactory.CreateInstanceFactory(LifetimeContext, options)` |
| `IInstanceFactory.Create<T>()` | `IInstanceFactory.GetInstance(IContainer scope)` |
| `Register<T>().AsEnumerable()` | `RegisterEnumerable<T, V>()`; `ResolveAll<T>()` or inject `IEnumerable<T>` |
| `RegisterInstance(x)` disposed `x` | `RegisterInstance(x)` never disposes `x` |
| `AutoRegistration.Register(assembly, container)` | `container.RegisterAssembly(assembly)` (also applies `IModule`s) |
| `Application.Push()`/`Pop()` (instance methods) | `Application.Push()`/`Pop()` (static) |
| `{yocto:ResolveResource MainViewModel}` (by name) | `{yocto:Resolve {x:Type vm:MainViewModel}}` (by type) in `yocto.Wpf` |
| Generic `Exception` on failures | `InvalidOperationException`; `ObjectDisposedException` after dispose |
| `yocto.owin` (never implemented) | Removed — use `YoctoServiceProviderFactory` with ASP.NET Core |

New in 3.0: scoped lifetime, keyed services, open generics, `IServiceProvider`/ASP.NET Core integration, `IAsyncDisposable`, `Validate()`, modules, and Native AOT annotations.

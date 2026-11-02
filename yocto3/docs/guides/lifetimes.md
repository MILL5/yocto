---
next:
  url: /guides/aspnet-core/
  title: ASP.NET Core and Generic Host
title: yocto lifetimes — transient, singleton, scoped, per-thread, pooled
description: How yocto manages object lifetimes on .NET 10 — Multiple (transient), Singleton, Scoped, PerThread, Pooled, and Instance — plus custom lifetimes and disposal rules.
breadcrumb: Lifetimes
last_reviewed: 2026-10-13
faq:
  - q: What is the default lifetime in yocto?
    a: Multiple — a new instance on every resolve, equivalent to Microsoft's Transient. Disposable instances are tracked weakly and disposed with the container that resolved them.
  - q: What is a pooled lifetime?
    a: A fixed-size pool of instances handed out round-robin. The pool grows lazily up to its size (default max(ProcessorCount, 8), minimum 2) and is thread-safe. Use it for expensive, thread-safe objects you want to spread load across.
---

# Lifetimes

Every registration starts as **Multiple**. Change it fluently:

```csharp
container.Register<IService, Service>().AsSingleton();
container.Register<IService, Service>().AsSingleton(eagerLoad: true); // create now
container.Register<IService, Service>().AsScoped();
container.Register<IService, Service>().AsPerThread();
container.Register<IService, Service>().AsPooled(poolSize: 4);
container.RegisterInstance<IService>(existing);
```

…or with shortcuts: `RegisterSingleton`, `RegisterScoped`, `RegisterPerThread`, `RegisterPooled`.

| Lifetime | Constant | Instances | Owned and disposed by |
|---|---|---|---|
| Multiple (default) | `Lifetimes.Multiple` | New on every resolve | The resolving container (tracked weakly) |
| Singleton | `Lifetimes.Singleton` | One per registration | The registering container |
| Scoped | `Lifetimes.Scoped` | One per resolving container | The resolving container |
| Per-thread | `Lifetimes.PerThread` | One per thread | The registering container |
| Pooled | `Lifetimes.Pooled` | Fixed pool, round-robin | The registering container |
| Instance | `Lifetimes.Instance` | The object you supplied | You — never disposed |

Singletons, per-thread, and pooled services resolve their dependencies from the **registering** container. Multiple and scoped services resolve theirs from the **resolving** container, so a child container's overrides apply.

## Disposal

A container disposes its children first, then every instance it owns in **reverse creation order**. `DisposeAsync` prefers `IAsyncDisposable`. Disposal is best effort: one throwing `Dispose` doesn't stop the rest. Replacing or removing a singleton, per-thread, or pooled registration disposes its instances immediately.

## Custom lifetimes

```csharp
public sealed class PerTenantLifetime : ILifetimeFactory
{
    public IInstanceFactory CreateInstanceFactory(LifetimeContext context, IReadOnlyList<object?> options) =>
        new PerTenantFactory(context);
}

Lifetimes.Register("PerTenant", new PerTenantLifetime());
container.Register<ICache, Cache>().WithLifetime("PerTenant");
```

`LifetimeContext.Activate(scope)` builds a new instance; `TrackForDisposal` and `Untrack` hand ownership to a container.

{% include faq.html heading="Lifetime FAQ" %}

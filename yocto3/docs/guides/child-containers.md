---
title: Child containers, scopes, ContainerBuilder, and Application.Current in yocto
description: Create child containers and scopes with yocto on .NET 10, override registrations, dispose hierarchies, build containers with ContainerBuilder, and use the ambient Application container.
breadcrumb: Child containers and scopes
last_reviewed: 2026-10-13
---

# Child containers and scopes

Every yocto container is also a scope.

```csharp
using var root = new Container();
root.Register<IClock, SystemClock>().AsSingleton();
root.Register<IUnitOfWork, UnitOfWork>().AsScoped();

using (var child = root.CreateChildContainer())
{
    child.Register<IClock, FrozenClock>();       // override just for this child
    var uow = child.Resolve<IUnitOfWork>();      // one per child
}                                                // child and its instances are disposed here
```

- A child falls back to its parent for anything it doesn't register.
- Disposing a parent disposes all of its children.
- `IServiceScopeFactory.CreateScope()` creates flat scopes under the container that provided the factory, as ASP.NET Core expects.

## ContainerBuilder

```csharp
using var builder = new ContainerBuilder(parent: null) { ValidateOnBuild = true };
builder.Register<IClock, SystemClock>();
builder.RegisterModule<BillingModule>();
IContainer container = builder.Build();   // once; later calls throw
```

## Application.Current

An optional ambient container for places that can't use constructor injection, such as XAML:

```csharp
Application.Current.Register<IClock, SystemClock>();
var scope = Application.Push();   // child of Current, now Current
// ...
Application.Pop();                // disposes it and restores the parent
```

Prefer constructor injection everywhere else.

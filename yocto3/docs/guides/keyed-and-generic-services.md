---
next:
  url: /guides/child-containers/
  title: Child containers and scopes
title: Keyed services, open generics, and IEnumerable in yocto
description: Register and resolve keyed services, open generic types like IRepository<T>, and multiple implementations as IEnumerable<T> with the yocto container for .NET 10.
breadcrumb: Keyed and generic services
last_reviewed: 2026-10-13
---

# Keyed and generic services

## Keyed services

```csharp
container.Register<IStorage, BlobStorage>("blob");
container.Register<IStorage, DiskStorage>("disk");

var blob = container.Resolve<IStorage>("blob");

public sealed class Uploader([FromKeyedServices("blob")] IStorage storage);
public sealed class Named([ServiceKey] string key);   // receives the key it was resolved with
```

Registering with `KeyedService.AnyKey` provides a fallback for any key, following Microsoft's semantics.

## Open generics

```csharp
container.Register(typeof(IRepository<>), typeof(Repository<>)).AsScoped();
var orders = container.Resolve<IRepository<Order>>();
```

A closed registration (`IRepository<Order>`) wins over the open one. Generic constraints are honored: an open generic that can't close over a type is skipped.

## Many implementations

`Register` replaces; `RegisterEnumerable` appends:

```csharp
container.RegisterEnumerable<IValidator, NotEmptyValidator>();
container.RegisterEnumerable<IValidator, MaxLengthValidator>();

IReadOnlyList<IValidator> all = container.ResolveAll<IValidator>();   // registration order
public sealed class Form(IEnumerable<IValidator> validators);          // injected the same way
```

A single `Resolve<IValidator>()` returns the last registration.

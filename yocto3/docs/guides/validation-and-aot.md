---
title: Validate yocto registrations and publish with Native AOT
description: Detect missing, circular, and captive dependencies at startup with yocto's Validate, and publish trimmed or Native AOT .NET 10 apps that use the yocto IoC container.
breadcrumb: Validation and Native AOT
last_reviewed: 2026-10-13
---

# Validation and Native AOT

## Validate

```csharp
container.Validate();
```

`Validate` walks every registration's constructor without creating anything and throws an `AggregateException` with one `InvalidOperationException` per problem:

- **Missing dependency** — no constructor can be satisfied.
- **Ambiguous constructors** — two usable constructors, neither a superset of the other.
- **Circular dependency** — `A → B → A`, with the full chain in the message.
- **Captive dependency** — a singleton, per-thread, or pooled service that depends, directly or through transients, on a scoped service.

Factory registrations can't be inspected and are skipped. Use `ContainerBuilder { ValidateOnBuild = true }` or `new YoctoServiceProviderFactory(validateOnBuild: true)` to validate automatically.

At runtime, circular dependencies are also detected and reported instead of overflowing the stack.

## Native AOT and trimming

yocto is built with `IsAotCompatible`, and its public API carries `[DynamicallyAccessedMembers]` annotations, so the trimmer keeps the constructors you register:

```csharp
container.Register<IGreeter, Greeter>();   // Greeter's public constructors are preserved
```

```xml
<PublishAot>true</PublishAot>
```

Constructors are invoked with `ConstructorInvoker` — no `Reflection.Emit`. Caveats:

- `RegisterAssembly` needs unreferenced code; use `RegisterModule` instead.
- Open generics closed over **value types** need runtime code generation and throw under Native AOT. Reference-type arguments work.

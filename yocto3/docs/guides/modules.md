---
title: yocto modules and assembly registration
description: Group registrations into IModule classes and register whole assemblies with yocto's AssemblyRegistration convention on .NET 10.
breadcrumb: Modules
last_reviewed: 2026-10-13
---

# Modules and assembly registration

## Modules

```csharp
public sealed class BillingModule : IModule
{
    public void Register(IRegistrar registrar)
    {
        registrar.Register<IInvoiceService, InvoiceService>().AsScoped();
        registrar.RegisterSingleton<ITaxTable, TaxTable>();
    }
}

container.RegisterModule<BillingModule>();
```

`IRegistrar` is implemented by both `IContainer` and `ContainerBuilder`.

## Assembly registration

```csharp
container.RegisterAssembly(typeof(BillingModule).Assembly);
```

This applies every public `IModule` with a parameterless constructor, plus every public static `AssemblyRegistration.Initialize(IContainer)` method (the yocto 1.x/2.x convention), and returns how many it applied. It uses reflection and is marked `RequiresUnreferencedCode`; in trimmed or Native AOT apps, call `RegisterModule` explicitly.

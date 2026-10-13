---
title: yocto IoC container — frequently asked questions
description: Answers about yocto, the tiny dependency injection container for .NET 10 — install, ASP.NET Core support, lifetimes, Native AOT, license, and comparisons with Autofac and Microsoft.Extensions.DependencyInjection.
faq:
  - q: What is yocto?
    a: yocto is a free, open-source (Apache 2.0) inversion-of-control (IoC) and dependency injection container for .NET 10 from MILL5. The name comes from the SI prefix for 10⁻²⁴ — it is deliberately tiny, about 1,200 lines of C#.
  - q: How do I install yocto?
    a: "Run `dotnet add package yocto`, or add `<PackageReference Include=\"yocto\" Version=\"3.*\" />` to your project file. The WPF/XAML markup extension ships separately as `yocto.Wpf`."
  - q: Can yocto replace the built-in ASP.NET Core container?
    a: "Yes. `builder.Host.UseServiceProviderFactory(new YoctoServiceProviderFactory())` makes yocto the app's IServiceProvider. yocto passes all 143 tests in Microsoft.Extensions.DependencyInjection.Specification.Tests 10.0, the suite Microsoft publishes for third-party containers, including keyed services, open generics, scopes, and disposal order."
  - q: Which lifetimes does yocto support?
    a: Multiple (transient, the default), Singleton (optionally eager), Scoped (one per container), PerThread, Pooled (fixed-size round-robin pool), and Instance (an externally owned object that yocto never disposes). You can add your own with Lifetimes.Register.
  - q: Does yocto support keyed services?
    a: "Yes. Pass a service key when registering and resolving, or use `[FromKeyedServices(\"key\")]` and `[ServiceKey]` on constructor parameters. KeyedService.AnyKey follows Microsoft's semantics."
  - q: Does yocto support Native AOT and trimming?
    a: Yes. The library is built with IsAotCompatible and has trimming annotations throughout, so the compiler warns you instead of failing at runtime. Assembly scanning (RegisterAssembly) is marked RequiresUnreferencedCode; prefer modules in trimmed apps.
  - q: How is yocto different from Autofac?
    a: Autofac is a large, mature container with decorators, relationship types, and interception. yocto is a small, auditable container that covers the common needs — scopes, child containers, modules, keyed and open generic services — plus per-thread and pooled lifetimes, and stays compatible with Microsoft's DI abstractions.
  - q: Which .NET versions does yocto support?
    a: yocto 3 targets .NET 10 (LTS). yocto 1.x and 2.x, which targeted .NET Framework, .NET Standard 1.x/2.0, and Xamarin, are no longer maintained.
  - q: Is yocto thread-safe?
    a: Yes. Registration and resolution are thread-safe. Singletons are created once, pools grow safely under contention, and scoped instances are created once per container.
  - q: What license is yocto released under?
    a: The Apache License 2.0. You can use it in commercial and closed-source software; keep the license notice.
  - q: Who built yocto?
    a: yocto was built by Richard Crane, a Microsoft Most Valuable Professional (MVP) and founder of MILL5, a Boston-based Microsoft Gold Partner specializing in cloud, AI, IoT, data analytics and modern application development.
  - q: What other open-source projects does MILL5 publish?
    a: MILL5's open-source family includes Pineapple (Guava-inspired core utilities for .NET), FastSearch (fast substring search for .NET), M5.BloomFilter, Polygon.Net, and the macOS utilities Cut & Move and MacEdgeLight.
  - q: Where do I report bugs or request features?
    a: Open an issue at https://github.com/MILL5/yocto/issues or email rich@mill5.com. Pull requests are welcome.
---

# Frequently asked questions

{% include faq.html heading="yocto FAQ" %}

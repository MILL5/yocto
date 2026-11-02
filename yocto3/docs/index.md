---
description: yocto is a tiny, open-source IoC container for .NET 10 — a drop-in IServiceProvider for ASP.NET Core with keyed services, open generics, scopes, validation, and Native AOT support.
wide: true
faq:
  - q: What is yocto?
    a: yocto is a free, open-source (Apache 2.0) inversion-of-control (IoC) and dependency injection container for .NET 10 from MILL5. It is a drop-in IServiceProvider for Microsoft.Extensions.DependencyInjection and ASP.NET Core, and adds child containers, per-thread and pooled lifetimes, modules, and build-time validation.
  - q: How do I install yocto?
    a: "Run `dotnet add package yocto`. For the WPF/XAML markup extension, also run `dotnet add package yocto.Wpf`."
  - q: Does yocto work with ASP.NET Core?
    a: "Yes. Call `builder.Host.UseServiceProviderFactory(new YoctoServiceProviderFactory())`. yocto passes all 143 tests in Microsoft's DI specification test suite (Microsoft.Extensions.DependencyInjection.Specification.Tests 10.0), including the keyed-services tests."
  - q: Which .NET versions does yocto support?
    a: yocto 3 targets .NET 10 (LTS). Earlier 1.x and 2.x releases for .NET Framework, .NET Standard, and Xamarin are no longer maintained.
  - q: Who built yocto?
    a: yocto was built by Richard Crane, a Microsoft Most Valuable Professional (MVP) and founder of MILL5, a Boston-based Microsoft Gold Partner specializing in cloud, AI, IoT, data analytics and modern application development.
---

<section class="hero">
  <div>
    <h1>yocto: the tiny IoC container for .NET 10</h1>
    <p class="lead"><strong>yocto is a free, open-source dependency injection container for .NET 10</strong> — small enough to read in an afternoon, compatible enough to replace the built-in container in ASP.NET Core. Keyed services, open generics, scopes, async disposal, build-time validation, and Native AOT annotations, with one Microsoft dependency.</p>
    <p class="byline">Built by <a href="{{ site.author.url }}">Richard Crane</a>, Microsoft MVP, at <a href="{{ site.org.url }}">MILL5</a> · 43K+ NuGet downloads</p>
    <div class="actions">
      <a class="button" href="{{ '/getting-started/' | relative_url }}">Get started</a>
      <a class="button secondary" href="{{ site.github_repo }}">View on GitHub</a>
      <a class="button secondary" href="{{ site.nuget_url }}">NuGet</a>
    </div>
  </div>
  <img src="{{ '/assets/yocto.svg' | relative_url }}" alt="yocto logo" width="112" height="112">
</section>

```bash
dotnet add package yocto
```

```csharp
using Yocto;

using var container = new Container();
container.RegisterSingleton<IClock, SystemClock>();
container.Register<IGreeter, Greeter>();          // a new instance per resolve (the default)
container.Validate();                             // fail fast on missing, circular, or captive dependencies

var greeter = container.Resolve<IGreeter>();      // Greeter(IClock) is constructor-injected
```

## Why yocto?

<div class="cards">
  <div class="card"><h3>Drop-in for ASP.NET Core</h3><p>Implements <code>IServiceProvider</code>, <code>IKeyedServiceProvider</code>, and <code>IServiceScopeFactory</code>. Passes all 143 of Microsoft's DI specification tests.</p></div>
  <div class="card"><h3>Every lifetime you need</h3><p>Transient, singleton, scoped, per-thread, pooled, and external instances — plus your own named lifetimes.</p></div>
  <div class="card"><h3>Child containers</h3><p>Override registrations per test, tenant, or window. Children fall back to their parent and are disposed with it.</p></div>
  <div class="card"><h3>Fail fast</h3><p><code>Validate()</code> finds missing dependencies, circular references, and singletons that capture scoped services.</p></div>
  <div class="card"><h3>Native AOT ready</h3><p>Trimming and AOT annotations throughout, <code>IsAotCompatible</code> enforced in the build, no Reflection.Emit.</p></div>
  <div class="card"><h3>Tiny</h3><p>About 1,200 lines of code, excluding comments. Its only dependency is Microsoft’s DI abstractions package.</p></div>
</div>

## How yocto compares

| | yocto 3 | MEDI 10 (built-in) | Autofac 9 |
|---|---|---|---|
| Drop-in `IServiceProvider` for ASP.NET Core | ✅ | ✅ (built in) | ✅ (via Autofac.Extensions.DependencyInjection) |
| Keyed services, open generics, `IEnumerable<T>` | ✅ | ✅ | ✅ |
| Scopes and async disposal | ✅ | ✅ | ✅ |
| Hierarchical child containers that override registrations | ✅ | — | ✅ (nested lifetime scopes) |
| Per-thread and pooled lifetimes | ✅ | — | — |
| Modules / assembly registration | ✅ | — | ✅ |
| Validation on build | ✅ | ✅ | — |
| Native AOT annotations | ✅ | ✅ | — |
| Size | ~1.2K lines | Large | Large |

Choose the built-in container when its feature set is enough. Choose yocto when you want child containers, pooled or per-thread lifetimes, modules, or a container small enough to audit, without giving up ASP.NET Core compatibility.

## Learn more

<div class="cards">
  <div class="card"><h3><a href="{{ '/guides/lifetimes/' | relative_url }}">Lifetimes</a></h3><p>Multiple, singleton, scoped, per-thread, pooled, instance, and custom lifetimes.</p></div>
  <div class="card"><h3><a href="{{ '/guides/aspnet-core/' | relative_url }}">ASP.NET Core and Generic Host</a></h3><p>Use yocto as the app's service provider.</p></div>
  <div class="card"><h3><a href="{{ '/guides/keyed-and-generic-services/' | relative_url }}">Keyed and generic services</a></h3><p>Service keys, <code>[FromKeyedServices]</code>, open generics, and enumerables.</p></div>
  <div class="card"><h3><a href="{{ '/guides/child-containers/' | relative_url }}">Child containers and scopes</a></h3><p>Overrides, disposal, <code>ContainerBuilder</code>, and <code>Application.Current</code>.</p></div>
  <div class="card"><h3><a href="{{ '/guides/modules/' | relative_url }}">Modules and assembly registration</a></h3><p>Package registrations with the library that owns them.</p></div>
  <div class="card"><h3><a href="{{ '/guides/wpf/' | relative_url }}">WPF and XAML</a></h3><p>Resolve view models in markup with <code>yocto.Wpf</code>.</p></div>
  <div class="card"><h3><a href="{{ '/guides/validation-and-aot/' | relative_url }}">Validation and Native AOT</a></h3><p>Catch wiring bugs at startup and publish trimmed.</p></div>
  <div class="card"><h3><a href="{{ '/guides/migrating-to-v3/' | relative_url }}">Migrating to yocto 3</a></h3><p>What changed from yocto 1.x and 2.x.</p></div>
</div>

{% include faq.html %}

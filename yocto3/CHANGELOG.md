# Changelog

## 3.0.0 — 2026-10-13

A ground-up rewrite for .NET 10. The 1.x/2.x codebases (.NET Framework 4.0/4.7.2, .NET Standard 1.0/1.1/2.0, Xamarin iOS, OWIN) were removed.

### Added
- Drop-in `IServiceProvider` for Microsoft.Extensions.DependencyInjection and ASP.NET Core: `YoctoServiceProviderFactory`, `BuildYoctoServiceProvider`, `Populate`. Passes all 143 tests in Microsoft.Extensions.DependencyInjection.Specification.Tests 10.0.
- Scoped lifetime (`AsScoped`, `RegisterScoped`) and `IServiceScopeFactory`.
- Keyed services, including `[FromKeyedServices]`, `[ServiceKey]`, and `KeyedService.AnyKey`.
- Open generic registrations.
- `IEnumerable<T>` constructor injection.
- `IAsyncDisposable` support and `DisposeAsync`. Instances are disposed in reverse creation order.
- `Validate()` and `ValidateOnBuild` to detect missing, ambiguous, circular, and captive dependencies.
- Runtime circular-dependency detection.
- `IModule` and `RegisterModule`.
- `Unregister` and `IRegistration.Remove`.
- Native AOT and trimming annotations (`IsAotCompatible`).
- `yocto.Wpf` package with `{yocto:Resolve {x:Type …}}`.

### Changed
- The namespace is now `Yocto` (was `yocto`).
- `new Container()` is public. Child containers are `IContainer`.
- The greediest constructor satisfiable from the registration's container wins, and is selected again after registrations change. Registration order no longer matters.
- Lifetimes are configured with `WithLifetime(name, options)`. Constants moved from `Instancing` to `Lifetimes`. `ILifetimeFactory` and `IInstanceFactory` were redesigned around `LifetimeContext`.
- `RegisterEnumerable` replaces `AsEnumerable`. Enumerables are stored per container instead of in a static dictionary.
- `RegisterInstance` no longer disposes the instance.
- `Application.Push`/`Pop` are static, and `Application.Current` follows them.
- `RegisterAssembly` passes the target container to `AssemblyRegistration.Initialize` (2.x always passed `Application.Current`).
- Failures throw `InvalidOperationException` instead of `Exception`.

### Removed
- Targets other than .NET 10, and their compatibility shims.
- `yocto.owin` (an unimplemented stub).
- `ResolveResourceExtension` (name-based lookup). Use `yocto.Wpf`'s `ResolveExtension`.

## 2.0.10-alpha — 2019

Last 2.x prerelease (.NET Standard 2.0, .NET Framework 4.0/4.7.2, XAML `ResolveResourceExtension`).

## Welcome to Yocto, an extremely small IoC container

### Overview ###

Yocto has been designed to be an extremely small, only a few 100 lines of code, extremely high quality and extremely high value Inversion of Control container.

Here are the core principals:

* **High Quality** - follow development best practices such as *preconditions*, *unit tests*, *code coverage*, etc.
* **Less Is More** - most projects do not need all the ceremony of large IoC frameworks, we prefer small framework, with a limited set of features.
* **Deployment** - published to NuGet using the new cross-platform .NET Class Library project type
* **Consumption** - built as a portable class library for use with Windows 8/10, Xamarin iOS/Android, ASP.NET Core, and .NET Framework.

### Key Features ###

* Simple API - Register, RegisterSingleton, Resolve, CanResolve, TryResolve
* Constructor Injection - constructors are selected automatically
* Lifetime Management - includes singleton, multi-instance, and extensible
* Child Containers - support for child containers, automatic "bubbling" of resolving to parent containers, and automatic disposal of singleton objects which support IDisposable
* Eager Type Factory Resolution - resolve type factories needed to construct instances at registration time
* Type Safety - use of generics for type safety

### Quality Bar ###

* # of Lines - 188 yocto, 121 yocto.tests
* Test Coverage - 96.22% overall, 97.37% yocto, 94.82% yocto.tests
* Best Practices - use of interfaces, preconditions, unit tests, code coverage, etc.
* Last Published May 7, 2016

### Why ###

You might be asking "Why another IoC container?"  I have had the opportunity to use many difference IoC containers on many different platforms. There is a need for a simple, yet powerful IoC container that is of high quality and value. I have used other containers like Autofac, TinyIoC, Ninject, Unity, and MvvmCross just to name a few.  Each of them have had tons of issues that we have specifically tried to address.

* Multipurpose IoC container that can be used for all types of applications such as desktop, web, and mobile.
* Proper support for child containers including disposing of singleton objects that support IDisposable.
* We do not like zombie objects aka partially constructed objects.  For this reason we do not support property injection.

## Welcome to Yocto, an extremely small IoC container

### Overview ###

Welcome to Yocto! An extremely small, low on ceremony, high on value Inversion of Control container. Yocto has been designed to be very small, a few 100 lines of code, but extremely high qualify.

Here are the core principals:

* **High Quality** - follow development best practices such as *preconditions*, *unit tests*, *code coverage*, etc.
* **Less Is More** - most projects do not need all the ceremony of large IoC frameworks, we prefer small framework, with a limited set of features.
* **Deployment** - while not yet implemented, we will publish Yocto to NuGet for use by all.
* **Consumption** - built as a portable class library for use with Windows 8/10, Xamarin iOS/Android, ASP.NET Core, and .NET Framework.

### Key Features ###

* Simple API - Register, Resolve, CanResolve
* Constructor Injection - constructors are selected automatically
* Lifetime Management - includes singleton and multi-instance
* Child Containers - support for child containers, automatic "bubbling" of resolving to parent containers, and automatic disposal of singleton objects which support IDisposable
* Eager Type Factory Resolution - resolve type factories needed to construct instances at registration time


### Quality Bar ###

* # of Lines - 147 yocto, 78 yocto.tests
* Test Coverage - 95.1% overall, 94.8% yocto, 95.57% yocto.tests
* Last Published May 7, 2016


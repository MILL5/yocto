## Welcome to Yocto, an extremely small IoC container

### Overview ###

Welcome to Yocto! An extremely small, low on ceremony, high on value Inversion of Control Container. Yocto has been designed to be very small, a few 100 lines of code, but extremely high qualify.

Here are the core principals:

* **High Quality** - Follow development best practices such as *preconditions*, *unit tests*, *code coverage*, etc.
* **Less Is More** - Most projects do not need all the ceremony of large IoC frameworks, we prefer small framework, with a limited set of features.
* **Deployment** - While not yet implemented, we will publish Yocto to NuGet for use by all.
* **Consumption** - built as a portable class library for use with Windows 8/10, Xamarin iOS/Android, ASP.NET Core, and .NET Framework.

### Key Features ###

* Simple API - Register, Resolve
* Constructor Injection - constructors are selected automatically
* Lifetime Management - includes singleton and multi-instance
* Child Containers - support for child containers, automatic "bubbling" of resolving to parent containers, and automatic disposal of singleton objects which support IDisposable
* Eager Type Factory Resolution - resolve type factories (not type) needed to construct instances at registration time

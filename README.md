## Welcome to Yocto, an extremely small IoC container

### Overview ###

Yocto has been designed to be an extremely small, only a few 100 lines of code, extremely high quality and extremely in high value Inversion of Control container.

Here are the core principals:

* **High Quality** - follow development best practices such as *preconditions*, *unit tests*, *code coverage*, etc.
* **Less Is More** - most projects do not need all the ceremony of large IoC frameworks, we prefer small framework, with a limited set of features.
* **Deployment** - published to NuGet using the new cross-platform .NET Class Library project type
* **Consumption** - built as a portable class library for use with Windows 8/10, Xamarin iOS/Android, ASP.NET Core, and .NET Framework.

### Key Features ###

* Simple API - Register, Resolve, CanResolve, TryResolve
* Type Safety - use of generics for type safety
* Constructor Injection - constructors are selected automatically
* Eager Type Factory Resolution - resolve type factories needed to construct instances at registration time
* Extensible API - RegisterSingleton, RegisterPerThread
* Lifetime Management - includes singleton, multi-instance, per thread
* Fluent API - AsSingleton, AsMultiInstance, AsInstancePerThread
* Child Containers - support for child containers, automatic "bubbling" of resolution to parent containers
* Memory Management - automatic disposal of singleton or per thread objects which support IDisposable when container is disposed
* Assembly Registration - new support for registering types for an assembly

### Quality Bar ###

* # of Lines - 255 yocto, 214 yocto.tests
* Test Coverage - 96.59% overall, 97.51% yocto, 95.59% yocto.tests
* Best Practices - use of interfaces, preconditions, unit tests, code coverage, etc.
* Last Published May 13, 2016

### Why ###

You might be asking "Why another IoC container?"  I have had the opportunity to use many difference IoC containers on many different platforms. There is a need for a simple, yet powerful IoC container that is of high quality and value. I have used other containers like Autofac, TinyIoC, Ninject, Unity, and MvvmCross just to name a few.  Each of them have had tons of issues that we have specifically tried to address with yocto.

#### Simple API ####

We support Register, Resolve, CanResolve, and TryResolve as the basic operations supported by the container.  This allows the developer to do interface development by registering interface types, resolving concrete implementation types, and creating multiple instances of these types (aka multiinstance) when needed.  This simple API is enough to use dependency injection and inversion of control in your application.

#### Type Safety ####

We support generics when registering types.  Of course we use constraints to ensure interface-based development.  So when you register an implementation type for a particular interface, it must implement that interface.

#### Constructor Injection ####

There are two ways commonly used to construct an object using dependency injection, Constructor or Property injection.  With constructor injection you get a fully constructed object instance assuming the constructor completes and all dependencies were passed to the constructor.  With property injection you construct an object instance and then you set dependencies.  It is difficult to determine whether or not an object is fully constructed when you use property injection.  If you forget to register a dependency, then the object can get constructed and returned to the user without all its dependencies being set.  We call these partially constructed objects (aka zombie objects).



* Multipurpose IoC container that can be used for all types of applications such as desktop, web, and mobile.
* Proper support for child containers including disposing of singleton objects that support IDisposable.
* We do not like zombie objects aka partially constructed objects.  For this reason we do not support property injection.



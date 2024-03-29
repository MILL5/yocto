## Welcome to Yocto, an extremely small IoC container

### Overview ###

Yocto has been designed to be an extremely small, only a few 100 lines of code, extremely high quality and extremely high in value Inversion of Control container.

Here are the core principals:

* **High Quality** - follow development best practices such as *preconditions*, *unit tests*, *code coverage*, etc.
* **Less Is More** - most projects do not need all the ceremony of large IoC frameworks, we prefer a small framework, with a limited set of features
* **Deployment** - published to NuGet using the new cross-platform .NET Class Library project type
* **Consumption** - built as a cross platform library for .NET to target multiple platforms (i.e. Windows 8/10, Xamarin iOS/Android, ASP.NET Core, .NET Core, and .NET Framework)

### Key Features ###

* Simple API - Register, Resolve, CanResolve, TryResolve
* Type Safety - use of generics for type safety
* Constructor Injection - constructors are selected automatically
* Eager Type Factory Resolution - resolve type factories needed to construct instances at registration time
* Extensible API - RegisterSingleton, RegisterPerThread, RegisterPooled, RegisterEnumerable
* Lifetime Management - includes singleton, multi-instance, per thread, and pooled (round-robin)
* Singleton Support - we support both lazy and eager loading of singletons
* Enumerable Support - Register multiple implementations and retrieve as enumerable
* Fluent API - AsSingleton, AsMultiple, AsPerThread, AsPooled, AsEnumerable
* Child Containers - support for child containers, automatic "bubbling" of resolution to parent containers
* Memory Management - automatic disposal of singleton, per thread or leaked multiinstance objects which support IDisposable when container is disposed
* Assembly Registration - new support for registering types for an assembly
* Custom Factories - **NEW** custom factories with Func<T>
* Register Instances - **NEW** register an instance of your type

### Quality Bar ###

* # of Lines - 499
* Code Coverage - 100%
* Best Practices - use of interfaces, preconditions, unit tests, code coverage, etc.
* Last Published July 2, 2016

### Why ###

You might be asking "Why another IoC container?"  I have had the opportunity to use many difference IoC containers on many different platforms. There is a need for a simple, yet powerful IoC container that is of high quality and value. I have used other containers like Autofac, TinyIoC, Ninject, Unity, and MvvmCross just to name a few.  Each of them have had tons of issues that we have specifically tried to address with yocto.

#### Simple API ####

We support Register, Resolve, CanResolve, and TryResolve as the basic operations supported by the container.  This allows the developer to do interface development by registering interface types, resolving concrete implementation types, and creating multiple instances of these types (aka multiinstance) when needed.  This simple API is enough to use dependency injection and inversion of control in your application.

#### Type Safety ####

We support generics when registering types.  Of course we use constraints to ensure interface-based development.  So when you register an implementation type for a particular interface, it must implement that interface.

#### Constructor Injection ####

There are two ways commonly used to construct an object using dependency injection: Constructor or Property injection.  **Yocto only supports constructor injection.**

With constructor injection you get a fully constructed object instance assuming the constructor completes and all dependencies were passed to the constructor.  This can easily be enforced by an IoC container (i.e. yocto).

With property injection you construct an object instance and then you set dependencies by calling property setters.  It is difficult to determine whether or not an object is fully constructed when you use property injection.  If you forget to register a dependency, then the object can get constructed and returned to the user without all of its dependencies being set.  We call these objects "partially constructed objects" (aka zombie objects).  We like zombies when playing video games, but not in our code.

#### Eager Type Factory Resolution ####

Many containers do not resolve types until resolution (i.e. Resolve).  This allows flexibility in type registration.  You can register your types without their dependencies being registered.  This can leads to problems when your application tries to resolve a type whose dependencies are missing.  Over the years, we have seen this problem occur many times in many different applications.  We want to avoid this issue.

#### Extensible API ####

This has always been a goal for us.  We realized very quickly that we could use our own extensibility to refactor yocto to be extremely simple, yet powerful.  For example, we extended our own framework with Singleton, PerThread, and Pooled instancing.  For us, that means the core framework has less code.  Also, we are dogfooding our own API and have fixed several bugs by doing so.

#### Lifetime Management ####

We support four lifetimes currently, Multi-instance, Singleton, PerThread, and Pooled.  This combined with child containers gives you extensive lifetime management features.

#### Singleton Support ####

We support both lazy and eager loading for singletons.  Lazy loading is the default.

#### Enumerable Support ####

We now support registering multiple implementations for a common interface using RegisterEnumerable or AsEnumerable.  This was challenging since we strive to maintain complete type safety.  All implementations are registered as multi-instance; however, you can use the fluent-based APIs to change the registration of the implementation.  That means you can have an enumerable of implementations where each implementation has their own custom lifetimes.

#### Fluent API ####

We support a simple fluent-based API for registration with AsMultiple, AsSingleton, AsPerThread, AsPooled and AsEnumerable.  We definitely want to expand on this feature.

#### Child Containers ####

A child container is great for supporting custom lifetime management within your application.  For example, an application that requires a user to login has a custom lifetime from login to logout.  Creating a child container after a user logs in and disposing it after a user logs out is a great way to track types and instances.

#### Memory Management ####

Singleton, PerThread, and Pooled instance lifetimes are managed by the container.  When the container is destroyed, so are the Singleton, PerThread and Pooled instances.  This includes calling Dispose() on those objects that support IDisposable.

We now do instance tracking for Multi-instance objects that support IDisposable.  We use weak references to allow instances to be cleaned up.  However, if an instance was not cleaned up (i.e. resource leak) and the container is being disposed, we will call Dispose() for you.

Our internal Registration class now holds a weak reference to the container.  We expose references of the Registration class as IRegistration.  This interface is the core of our fluent-based API.  The consumer of IRegistration instances previously could keep a container instance alive.  That is no longer the case.  We take advantage of this with our Enumerable support.

#### Assembly Registration ####

Create a static class in your application called AssemblyRegistration with an Initialize method that takes a single parameter of IContainer.  This is a way for an assembly to be completely self contained, register implementations and manage lifetime sematics.

#### Custom Factories ####

Now you can provide a custom factory using Func<T> with the Register, RegisterSingleton, RegisterPerThread, RegisterPooled methods.

#### Register Instances ####
Already have an instance and want to register that instance?  Now you can!  The RegisterInstance method takes an instance.

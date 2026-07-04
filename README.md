# **Yocto**
*An extremely small IoC container*

## ✨ Overview

Yocto is a lightweight Inversion of Control (IoC) container designed to provide high-quality dependency injection with minimal ceremony. Built with only a few hundred lines of code, it emphasizes simplicity and performance.

### Core Principles:
- **High Quality:** Adheres to development best practices such as *preconditions*, *unit tests*, *code coverage*, and more.
- **Less Is More:** Focuses on a small framework with essential features, avoiding the overhead of larger IoC frameworks.
- **Deployment:** Available on NuGet, designed for cross-platform .NET Class Library projects.
- **Consumption:** Supports various platforms including Windows 8/10, Xamarin iOS/Android, ASP.NET Core, .NET Core, and .NET Framework.

## 🚀 Getting Started

### Prerequisites
- .NET SDK (version 5.0 or higher)
- NuGet package manager

### Installation
To install Yocto, simply use the following command in your terminal:
```bash
dotnet add package yocto
```

### Setup
1. Create a new .NET project or open an existing one.
2. Add Yocto as a dependency using the installation command above.
3. Use the following basic setup in your application:

```csharp
using Yocto;

public class Program
{
    public static void Main(string[] args)
    {
        var container = new Container();

        // Register services
        container.Register<IMyService, MyService>();

        // Resolve services
        var myServiceInstance = container.Resolve<IMyService>();
    }
}
```

### Basic Usage
Yocto provides a straightforward API for registering and resolving services:
- **Register:** Register a service type.
- **Resolve:** Retrieve an instance of a service.
- **CanResolve:** Check if a service can be resolved.
- **TryResolve:** Attempt to resolve a service, returning null if it cannot be resolved.

## 🏗️ Architecture

### Tech Stack
- **Language:** C#
- **Framework:** .NET Standard for cross-platform compatibility.

### Key Features
- **Simple API:** Core operations include Register, Resolve, CanResolve, and TryResolve.
- **Type Safety:** Utilizes generics to ensure type-safe registrations.
- **Constructor Injection:** Supports only constructor injection to ensure fully constructed instances.
- **Eager Type Factory Resolution:** Resolves dependencies at the time of registration to prevent runtime errors.
- **Extensible API:** Includes various lifetime management strategies like Singleton, PerThread, and Pooled instances.
- **Memory Management:** Automatically disposes of instances that support IDisposable when the container is disposed.
- **Custom Factories:** Allows for custom factory methods using Func<T>.

### Quality Assurance
- **Code Coverage:** 100%
- **Best Practices:** Adheres to interface-based development, preconditions, and comprehensive unit testing.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit issues, fork the repository, and create pull requests. For any inquiries, you can contact us at [sales@mill5.com](mailto:sales@mill5.com).

## 📄 License

This project is licensed under the Apache License 2.0. See the [LICENSE](LICENSE) file for details.

---

*Powered by [MILL5](https://www.mill5.com)*


---

<sub>Powered by [RepoSpotlight](https://repospotlight.ai)</sub>

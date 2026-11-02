---
next:
  url: /guides/validation-and-aot/
  title: Validation and Native AOT
title: Resolve view models in WPF XAML with yocto.Wpf
description: Use the yocto.Wpf ResolveExtension markup extension to resolve view models and services from the yocto IoC container in WPF XAML on .NET 10.
breadcrumb: WPF and XAML
last_reviewed: 2026-10-13
---

# WPF and XAML

```bash
dotnet add package yocto.Wpf
```

Register at startup:

```csharp
public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        Yocto.Application.Current.RegisterModule<ViewModelsModule>();
        base.OnStartup(e);
    }
}
```

Resolve in XAML:

```xml
<Window xmlns:yocto="https://github.com/MILL5/yocto"
        xmlns:vm="clr-namespace:MyApp.ViewModels"
        DataContext="{yocto:Resolve {x:Type vm:MainViewModel}}">
```

`ResolveExtension` also accepts `Key` for keyed registrations and `Container` to resolve from a container other than `Application.Current`. It returns `null` if the service isn't registered, so the designer keeps working.

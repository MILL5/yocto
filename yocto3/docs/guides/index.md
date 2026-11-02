---
title: yocto guides
description: Guides for yocto, the tiny IoC container for .NET 10 — lifetimes, ASP.NET Core, keyed and generic services, child containers, modules, WPF, validation, Native AOT, and migration.
---

# Guides

## All guides

<div class="cards">
  <div class="card"><h3><a href="{{ '/guides/lifetimes/' | relative_url }}">Lifetimes</a></h3><p>Multiple, singleton, scoped, per-thread, pooled, instance, and custom.</p></div>
  <div class="card"><h3><a href="{{ '/guides/aspnet-core/' | relative_url }}">ASP.NET Core and Generic Host</a></h3><p>Use yocto as the app's <code>IServiceProvider</code>.</p></div>
  <div class="card"><h3><a href="{{ '/guides/keyed-and-generic-services/' | relative_url }}">Keyed and generic services</a></h3><p>Keys, open generics, and <code>IEnumerable&lt;T&gt;</code>.</p></div>
  <div class="card"><h3><a href="{{ '/guides/child-containers/' | relative_url }}">Child containers and scopes</a></h3><p>Overrides, disposal, builders, and the ambient container.</p></div>
  <div class="card"><h3><a href="{{ '/guides/modules/' | relative_url }}">Modules and assembly registration</a></h3><p>Package registrations with the library that owns them.</p></div>
  <div class="card"><h3><a href="{{ '/guides/wpf/' | relative_url }}">WPF and XAML</a></h3><p>Resolve view models in markup with <code>yocto.Wpf</code>.</p></div>
  <div class="card"><h3><a href="{{ '/guides/validation-and-aot/' | relative_url }}">Validation and Native AOT</a></h3><p>Catch wiring bugs at startup; publish trimmed.</p></div>
  <div class="card"><h3><a href="{{ '/guides/migrating-to-v3/' | relative_url }}">Migrating to yocto 3</a></h3><p>What changed from 1.x and 2.x.</p></div>
</div>

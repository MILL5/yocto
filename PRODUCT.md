# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Users

.NET developers in general: anyone building on .NET 10 who needs dependency injection — ASP.NET Core and Generic Host apps, libraries, desktop (WPF/XAML) apps, and test suites. Within that audience the site also serves existing yocto 1.x/2.x users moving to 3.0, but no single sub-group takes priority over the others.

Their job on the site: decide whether yocto fits, get it installed and wired in quickly, and look up specific behavior (lifetimes, scopes, keyed services, validation, AOT) while coding.

## Product Purpose

yocto is a free, open-source (Apache 2.0) IoC / dependency injection container for .NET 10 from MILL5. The site (mill5.github.io/yocto, Jekyll on GitHub Pages, source in `yocto3/docs/`) explains what yocto is, how it compares, and how to use it. Success: a developer can go from landing to a working `Container` or `YoctoServiceProviderFactory` setup without leaving the site, and can find any guide in one click.

## Positioning

Two co-equal claims; neither leads:

- **Tiny and auditable, yet a drop-in.** About 1,200 lines of C# (excluding comments) with one dependency (Microsoft's DI abstractions), small enough to read in an afternoon, while acting as a drop-in `IServiceProvider` that runs Microsoft's DI specification suite.
- **More than the built-in container.** Hierarchical child containers that override registrations, per-thread and pooled lifetimes, custom named lifetimes, modules and assembly registration — on top of MEDI compatibility.

## Operating Context

- Readers arrive from NuGet, GitHub, search, and LLM tools (the site publishes `llms.txt` and FAQ structured data).
- Reading happens next to an IDE; code samples get copied directly. Code blocks must be accurate and copyable.
- Light and dark color schemes are both supported (`color-scheme: light dark`).
- Content lives in Markdown under `yocto3/docs/` with one shared layout (`_layouts/default.html`) and one stylesheet (`assets/css/site.css`). Deployed by `.github/workflows/pages.yml` from `main`.

## Capabilities and Constraints

- Static Jekyll site; no client-side JavaScript framework. Keep it that way unless a need is confirmed.
- Pages: home, getting started, guides index plus eight guides (lifetimes, ASP.NET Core, keyed and generic services, child containers, modules, validation and AOT, WPF, migrating to v3), FAQ, changelog, 404.
- Terminology: "container", "child container", "scope", "registration", "lifetime" (Multiple/transient, Singleton, Scoped, PerThread, Pooled, Instance), "module".
- The comparison table on the home page must stay factually accurate against MEDI 10 and Autofac 9.

## Brand Commitments

- **Attribution:** "Built by Richard Crane, Microsoft MVP, at MILL5" byline and the footer credits to MILL5 and the Microsoft MVP profile stay.
- **Logo:** the existing yocto mark (`yocto3/docs/assets/yocto.png`, `yocto.svg`) stays.
- **Podcast:** the "Inventing Fire with AI" footer link stays.
- **Download count:** the "43K+ NuGet downloads" claim in the hero byline stays; the owner keeps the number current.
- Name is always lowercase "yocto".

## Evidence on Hand

- NuGet download count (43K+), maintained by the owner.
- Microsoft DI specification test suite results (`yocto3/tests/Yocto.Specification.Tests`). Any "passes all N tests" claim must match a green run.
- Logo assets and OG image in `yocto3/docs/assets/`.
- No testimonials, customer logos, case studies, or benchmarks exist. Do not invent them.

## Product Principles

1. **Accuracy over persuasion.** Every claim on the site (spec-suite results, line counts, comparison table) must be true today.
2. **Code first.** A developer should see working C# before reading prose about it.
3. **Small stays small.** The site should feel like the library: compact, readable, nothing ornamental.
4. **One click to any answer.** Guides and FAQ are reachable from every page.

## Accessibility & Inclusion

Inferred from the existing site, not yet confirmed by the owner: WCAG 2.2 AA — keyboard-reachable navigation with visible focus, skip link, sufficient contrast in both light and dark schemes, and no information conveyed by color alone.

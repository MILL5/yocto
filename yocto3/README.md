# yocto 3 — source

This folder is the yocto codebase: one .NET 10 solution. For the product overview, see the [repository README](../README.md) or the docs at https://mill5.github.io/yocto/.

## Layout

| Path | What |
|---|---|
| `src/Yocto` | The `yocto` NuGet package (net10.0) |
| `src/Yocto.Wpf` | The `yocto.Wpf` NuGet package: XAML `ResolveExtension` (net10.0-windows) |
| `tests/Yocto.Tests` | yocto behavior tests (MSTest) |
| `tests/Yocto.Specification.Tests` | Microsoft.Extensions.DependencyInjection.Specification.Tests run against yocto (xUnit) |
| `samples/Yocto.Sample` | Console sample; publishes with Native AOT |
| `docs` | GitHub Pages site (Jekyll), `llms.txt`, structured data |

## Build and test

```bash
dotnet build -c Release          # warnings are errors; AOT/trim analyzers on
dotnet test -c Release
dotnet run --project samples/Yocto.Sample
dotnet pack -c Release -o artifacts
```

`yocto.Wpf` builds on macOS and Linux through `EnableWindowsTargeting`, but runs only on Windows.

## Docs site

```bash
cd docs
cp ../CHANGELOG.md _includes/CHANGELOG.md   # generated include; the Pages workflow does the same
bundle install
bundle exec jekyll serve --baseurl /yocto
```

`.github/workflows/pages.yml` deploys the site on pushes to `main` that touch `yocto3/docs` or `yocto3/CHANGELOG.md`.

## Releasing

1. Bump `<Version>` in `Directory.Build.props` and add a `CHANGELOG.md` entry.
2. Update `version:` in `docs/_config.yml`.
3. Tag `vX.Y.Z` on `main`. `.github/workflows/release.yml` tests, packs, pushes to NuGet (Trusted Publishing), and creates a GitHub release.

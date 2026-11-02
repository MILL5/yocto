# Goldfish Review Report — Stage 9 (Implementation Readiness Test)

**Target:** `/Users/rich/.openclaw/projects/yocto/docs/designs/yocto3-release-hardening.md` (lines 318–407)

## Reviewer Status

| Reviewer | Status |
| --- | --- |
| `egm-implementation-readiness` | completed |
| `egm-codebase-details` | completed |
| `egm-style` | completed |

**Totals:** 🔴 0 · 🟡 14 · 🟢 11

Verdict: this range cannot yet be implemented without guessing. The design holds up. The gaps are in the regression tests (two of them cannot fail on the baseline), in decisions about the activator plan cache that are left open, and in the verification steps.

Where several reviewers reported the same issue, the finding is merged under one ID and the other lenses are listed.

---

## 🔴 Blockers

None.

---

## 🟡 Major

### egm-implementation-readiness-1-1: The concurrent Unregister/Register test cannot fail on the current code
* **Lenses:** implementation-readiness, codebase-details, style
* **Affected lines:** 368–370, 398
* **Issue:**
  * `Register` calls `Add(..., replace: true)`, which writes `[registration]` without reading the existing array (`Container.cs:140`). A racing `Unregister` therefore always ends in a valid serial outcome.
  * The resurrection bug only happens on the append path. `RegisterEnumerable` writes `[.. existing, registration]` and can bring back the removed entry.
  * "Many iterations" is not a number, and the pass/fail rule for a nondeterministic test against the baseline is undefined.
* **Suggested fix:**
  * Pre-register `RegisterEnumerable<IAnimal, Dog>()`, then race `Unregister<IAnimal>()` against `RegisterEnumerable<IAnimal, Cat>()`.
  * Assert that `ResolveAll<IAnimal>()` is either empty or exactly `[Cat]`, and never contains `Dog`.
  * Fix the iteration count (for example 10,000) and the synchronization method (a `Barrier` or `Parallel.Invoke`).
  * Define "fails on baseline" for this test, for example "fails in at least 1 of N runs".

### egm-codebase-details-1-1: `EnumerableCache_IsInvalidatedByParentRegistration` passes on the current code and is underspecified
* **Lenses:** codebase-details, implementation-readiness
* **Affected lines:** 365–367, 398
* **Issue:**
  * The global `s_version` already invalidates every cache (`Container.cs:142`, `:223-224`), so this test contradicts line 398 ("each new test fails…").
  * With the default `Multiple` lifetime nothing is cached (`Container.cs:229`), so the test would pass vacuously.
  * `FindAll` stops at the first container that has matches (`:263-271`), so the child must have no registrations of its own for `T`.
* **Suggested fix:**
  * Label it as a guard test that must pass both before and after the change, and exclude it from line 398.
  * Require `AsSingleton()` registrations, and require that the child has no registrations for `T`.

### egm-implementation-readiness-1-2: The baseline commit is wrong and the regression-proof procedure is undefined
* **Lenses:** implementation-readiness, codebase-details, style
* **Affected lines:** 398–399
* **Issue:**
  * The `feature/yocto3` tip is `59fcb74`, not `48437a5`.
  * "Current tip" goes stale as soon as the fix is committed.
  * The new tests do not exist at either commit, so "run against commit X" has no procedure.
* **Suggested fix:**
  * Name `59fcb74` as "the base before this change".
  * Spell out the procedure: apply only the test and `Fakes.cs` changes on top of the base, run `dotnet test yocto3/tests/Yocto.Tests -c Release --filter "Name~<test>"`, and expect a failure.

### egm-implementation-readiness-1-3: The plan cache's type and order of operations are unspecified
* **Lenses:** implementation-readiness, codebase-details, style
* **Affected lines:** 350–356 (field order also conflicts with line 214)
* **Issue:**
  * "Cached record `(Container Scope, long Stamp, Plan Plan)`" reads as a `ValueTuple`. `Volatile.Write` cannot publish a multi-field struct atomically, so reads could tear.
  * The text does not say to read the stamp before `SelectConstructor`. If it is read after, a stale plan can be stamped as current and never rebuilt. `ResolveEnumerable` already reads first (`Container.cs:223`).
* **Suggested fix:**
  * Specify `private sealed record PlanEntry(Container Scope, long Stamp, Plan Plan);` with a field `private PlanEntry? _entry;`.
  * Specify the order: `Volatile.Read` the entry, then read `RegistrationStamp`, then select, then `Volatile.Write`.
  * Align the field order at line 214.

### egm-implementation-readiness-1-4: There is no rule for a scope that is not a `Container`
* **Lenses:** implementation-readiness, codebase-details
* **Affected lines:** 351–353
* **Issue:** `Create` and `GetDependencies` take `IContainer` (`ConstructorActivator.cs:27`), and the public `LifetimeContext.Activate(IContainer)` (`Lifetimes.cs:34`) lets a custom lifetime pass any `IContainer`. `RegistrationScope` and `RegistrationStamp` exist only on `Container`.
* **Suggested fix:** Pick one rule:
  * throw `ArgumentException`, matching `LifetimeContext.AsContainer` (`Lifetimes.cs:46-47`); or
  * select a constructor each time without caching.

### egm-implementation-readiness-1-5: The plan cache keeps a strong reference to a scope container
* **Lenses:** implementation-readiness
* **Affected lines:** 350–356
* **Issue:** Caching `Container Scope` on an activator owned by a parent registration keeps a disposed child container alive until another scope resolves that registration.
* **Suggested fix:** Either store a `WeakReference<Container>` and treat a dead target as a cache miss, or state explicitly that the retention is accepted.

### egm-codebase-details-1-2: The new fakes duplicate existing ones, and the CUJ tests have no way to assert which constructor ran
* **Lenses:** codebase-details, implementation-readiness, style
* **Affected lines:** 368, 376–377, 379–383
* **Issue:**
  * `Fakes.cs:46-52` already has `Greedy` with constructors `()`, `(IAnimal)` and `(IAnimal, IPerson)`, plus an `int Constructor` property.
  * "If equivalent fakes do not already exist" leaves the decision to the implementer.
  * The proposed `ReportService` has no way to report which constructor was used.
  * CUJ 4 uses `IPlugin` and `PluginB`, which the fakes list never adds.
* **Suggested fix:** Drop item 6 and map each scenario to existing fakes:
  * CUJ 2: register `Greedy` and `IAnimal`, then expect `Constructor == 1`. Register `IPerson` without re-registering `Greedy`, then expect 2.
  * CUJ 3: the root registers `Greedy` and `IAnimal`, and a child registers `IPerson` → `Person`. Resolving from the child gives 2, then resolving from the root gives 1.
  * CUJ 4: use `IAnimal`, `Dog` and `Cat`.

### egm-codebase-details-1-3: The constructor-reselection tests do not state a lifetime
* **Lenses:** codebase-details, implementation-readiness
* **Affected lines:** 376–377
* **Issue:**
  * Singleton, PerThread, Pooled and Instance registrations always activate against `context.Owner` (`InstanceFactories.cs:64`, `:176`).
  * A singleton activates only once (`:91`), which would hide reselection.
  * CUJ 3 can only be reproduced with `Multiple` (`:41`) or `Scoped` (`:163`).
* **Suggested fix:** Require the default `Multiple` lifetime in both tests.

### egm-codebase-details-1-4: An existing test that works around the bug is not addressed
* **Lenses:** codebase-details
* **Affected lines:** 376
* **Issue:** `Constructor_GreediestResolvableWins` (`RegistrationTests.cs:116-125`) re-registers `Greedy` at line 123 only to clear the cached plan, which hides the bug.
* **Suggested fix:** State whether to remove line 123 so the test covers CUJ 2, or whether the new test replaces it.

### egm-codebase-details-1-5: The `Release` change is left as an either/or
* **Lenses:** codebase-details, implementation-readiness, style
* **Affected lines:** 335–336 (also 204–206)
* **Issue:**
  * "Becomes non-static or calls `r.Owner.OnRegistrationsChanged()` per distinct owner" leaves the design choice to the implementer.
  * Every caller passes registrations owned by `this`:
    * `Unregister` (`:76-78`)
    * `Remove` (`:97`)
    * `Add` (`:144`)
    * `Dispose` and `DisposeAsync` (`:442`, `:453`, via `:477`)
* **Suggested fix:**
  * Specify `private void Release(Registration[] registrations)` as an instance method that calls `this.OnRegistrationsChanged()`.
  * Make the same change at lines 204–206.
  * The style reviewer preferred the per-owner form. The code evidence above supports the instance-method form.

### egm-codebase-details-1-6: The CHANGELOG instruction would leave two bullets that contradict each other
* **Lenses:** codebase-details, implementation-readiness, style
* **Affected lines:** 388–389
* **Issue:**
  * `CHANGELOG.md:24` already says "Constructors are selected on first resolve…", so adding a bullet would contradict it.
  * The entry covers only one of the three fixes.
* **Suggested fix:**
  * Rewrite line 24, for example: "The greediest satisfiable constructor wins; selection is repeated when registrations visible to the resolving container change."
  * Either list the per-container enumerable caching and the `Unregister` thread-safety fix, or say why they are left out.

### egm-codebase-details-1-7: Changed code paths have no regression tests
* **Lenses:** codebase-details
* **Affected lines:** 358–377
* **Issue:** Three changed paths have no listed test:
  * `WithLifetime` now bumps the owner's version (`Registration.cs:120`).
  * `Validate()` from a child followed by a resolve from the parent. `Verify` calls `GetDependencies(this)` with the child (`Container.cs:533`).
  * Disposing an unrelated container. `Dispose` → `Release` bumps the version even with no registrations (`:442`, `:150`), which is a second trigger for the cross-container cache bug.
* **Suggested fix:** Add a test case for each to item 4 or item 5.

### egm-implementation-readiness-1-6: The verification plan does not cover the performance requirement
* **Lenses:** implementation-readiness
* **Affected lines:** 391–401 (requirement at line 290)
* **Issue:** The Non-Functional Requirements allow at most a 5% resolve regression. Nothing in the verification plan measures it, and no benchmark harness exists.
* **Suggested fix:** Add a verification step that names:
  * the benchmark: a cached singleton `Resolve<T>` in a root container and in a MEDI scope;
  * the baseline commit;
  * the BenchmarkDotNet command;
  * the pass threshold.

### egm-style-1-1: It is unclear which operations increment `_version`, and how
* **Lenses:** style, implementation-readiness
* **Affected lines:** 327–328, 334–339
* **Issue:**
  * Only `Add` says it calls `OnRegistrationsChanged()`.
  * `Unregister` mentions only the lock, and `Remove` and `WithLifetime` do not appear.
  * The increment primitive is not stated here, although line 286 requires `Interlocked`.
* **Suggested fix:** Add one bullet: "Every registration change (`Add`, `Remove`, `Unregister`, `WithLifetime`) increments the owning container's `_version` via `Interlocked.Increment`, through `Release` or directly."

---

## 🟢 Nits

### egm-codebase-details-1-8: The `Add` bullet describes existing behavior
* **Affected lines:** 334
* **Issue:**
  * `Add` already calls `OnRegistrationsChanged()` (`Container.cs:142`).
  * On replace it is called again inside `Release` (`:150`).
* **Suggested fix:** Reword as "call site unchanged; now resolves to the instance method". Note the harmless double bump on replace.

### egm-codebase-details-1-9: `-warnaserror` is redundant
* **Affected lines:** 393–394
* **Issue:**
  * `Directory.Build.props:6` already sets `TreatWarningsAsErrors=true`.
  * The build of `Yocto.Wpf` on macOS relies on `EnableWindowsTargeting` (`Yocto.Wpf.csproj:6`).
* **Suggested fix:** Drop the flag, or mark it as belt-and-braces. Note the `EnableWindowsTargeting` dependency.

### egm-implementation-readiness-1-7: Verification bullets lack exact commands and pass criteria
* **Lenses:** implementation-readiness, style
* **Affected lines:** 393–397
* **Issue:**
  * The 50-run loop has no command.
  * The build and unit-test bullets have no pass criteria.
  * The stability bullet omits the parallelization condition from the Goal (lines 64–65).
  * The stability bullet starts with a lowercase "run".
* **Suggested fix:**
  * Give the command: build once, then `for i in $(seq 50); do dotnet test yocto3/tests/Yocto.Specification.Tests -c Release --no-build || exit 1; done`.
  * Add "zero warnings" and "all tests pass".
  * State that the run uses the test framework's default parallelization.

### egm-style-1-2: Two names are used for one concept
* **Affected lines:** 331 (vs 233)
* **Issue:**
  * The prose says "registration-bearing container" and the code says `RegistrationScope`.
  * "Scope" also clashes with MEDI scopes.
* **Suggested fix:** Use one term in both places, and consider `RegistrationOwner`.

### egm-style-1-3: "Stamp" used as a verb is too terse
* **Affected lines:** 337
* **Suggested fix:** "store `RegistrationStamp` with each cached array and compare against it on lookup, replacing `s_version`."

### egm-style-1-4: "Compares reference and stamp" is ambiguous
* **Affected lines:** 353
* **Suggested fix:** "compares the cached `Scope` by reference with the resolved `RegistrationScope`, and the cached `Stamp` with its current `RegistrationStamp`."

### egm-style-1-5: The bullets mix sentence forms
* **Affected lines:** 326–339
* **Suggested fix:** Use the form "`Member`: imperative action" throughout.

### egm-style-1-6: "Reselect" is spelled several ways
* **Affected lines:** 354, 376, 389 (also 220)
* **Suggested fix:** Use "reselect" everywhere.

### egm-style-1-7: The relationship between containers A and B is unstated
* **Affected lines:** 364
* **Suggested fix:** "a separately constructed container B (neither ancestor nor descendant of A)".

### egm-style-1-8: "The binary runs" is not measurable
* **Affected lines:** 401
* **Suggested fix:**
  * Use "the published binary exits with code 0".
  * State that AOT comes from `PublishAot` in the project file, or give the required `-r <RID>`.

### egm-style-1-9: The provenance comment and the footer repeat each other
* **Affected lines:** 405–407
* **Issue:** "EGM" is never spelled out.
* **Suggested fix:** Keep one of the two and spell out the acronym, unless the template requires both.

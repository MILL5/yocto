# Stage 6 — Comprehension Report

Reviewer: egm-comprehension (completed)
Target: docs/designs/yocto3-release-hardening.md (FeatureRange 1..317, DetailedRange 318..407)

### What this proposes

The document wants to fix three caching and concurrency bugs before yocto 3.0.0 is tagged on `main`. First, the static enumerable-cache version counter becomes a `_version` field on each container. A cache is then stamped with the sum of `_version` over the container and its ancestors, so a registration in an unrelated container no longer throws away another container's cached `IEnumerable<T>` arrays. Second, `ConstructorActivator` keeps a single cached plan tagged with the container it was built for and that container's stamp. It picks a constructor again when either differs, so registering a dependency late, or resolving from a child first, no longer locks in the wrong constructor. Third, `Unregister` takes `_lock` like `Add` and `Remove` already do. Each fix needs a regression test, and the MEDI specification suite must pass 50 consecutive runs in parallel.

### How the existing system works, per this document

A `ContainerBuilder` builds a root `Container`. The container stores registrations by `(ServiceType, Key)` and caches derived matches, enumerable arrays, scoped instances and owned disposables. Child containers, and MEDI scopes created through `IServiceScopeFactory`, check their parent when a lookup misses. Each registration has an `IInstanceFactory` chosen by lifetime name (Multiple, Singleton, Scoped, PerThread, Pooled or Instance). It builds objects through a `ConstructorActivator`, which picks the greediest constructor it can satisfy on the first resolve and caches that plan forever. Today one static counter, bumped by any registration change in any container, invalidates every enumerable cache in the process. That makes the specification suite fail now and then when tests run in parallel. Separately, `Unregister` removes entries without holding the lock that `Register` uses.

### Findings

**C1: The tests contradict "every new test fails on current code"** — 🟡 Major — lines 75-76, 365-370, 398-399
`EnumerableCache_IsInvalidatedByParentRegistration` passes today (the global `s_version` already discards every cache); it is a guard test, not a regression test. `Unregister_ConcurrentWithRegister_IsLinearizable` is timing-dependent and fails only sometimes.
*Fix:* Split tests into "regression (fail before fix)" and "guard (must keep passing)". Give the race test an iteration count and expected pre-fix failure rate.

**C2: The commit called "the current tip" is wrong** — 🟡 Major — lines 398-399
The tip of `feature/yocto3` is `59fcb74`, not `48437a5`.
*Fix:* Say "any commit where `Container.s_version` is static".

**C3: The `Container.Release` change is left undecided** — 🟡 Major — lines 203-206, 335-336
"Instance method **or** takes the owner from each registration" — pick one; define what Release does to instances and caches.

**C4: "Registration-bearing container" is introduced without saying why it is safe** — 🟡 Major — lines 214-216, 230-234, 331-333, 352-354
State the invariant (zero-version containers cannot change plan inputs), which container's stamp is stored, and define or replace "scope root".

**C5: Unclear which problem causes the named failing test** — 🟢 Nit — lines 17-21, 33-39
State that it fails because of Problem 1 and what it asserts.

**C6: Another process-wide counter goes unmentioned** — 🟢 Nit — lines 176-178
Say the registration sequence number only orders registrations and is never used for cache freshness.

**C7: Siblings share one cached plan, which Goal 2 doesn't address** — 🟢 Nit — lines 66-68, 224-226
Narrow Goal 2 or acknowledge sibling churn.

**C8: Current behavior described as a defect, but the CHANGELOG documents it** — 🟢 Nit — lines 44-51, 277-279, 303-304, 388-389
CHANGELOG line 24 says "Constructors are selected on first resolve". Say this design reverses it before release.

**C9: Some project-specific terms are never defined** — 🟢 Nit
`Application.Current`/Push/Pop, "Multiple", "Plan", `Populate`/modules, "today's guarantee", "session scratch directory".

**C10: Cache-entry tuple written in two orders** — 🟢 Nit — lines 214, 351

### Verdict

Both sections can be paraphrased, so there are no Blockers. C1–C4 should be fixed before approval.

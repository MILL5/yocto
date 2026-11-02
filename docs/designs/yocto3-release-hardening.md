# yocto 3.0 Release Hardening

*   **Author:** Richard Crane (ChiefInnovator)
*   **Status:** Approved
*   **Date:** 2026-11-02

--------------------------------------------------------------------------------

## Overview

yocto 3 is a ground-up rewrite of the yocto IoC container for .NET 10. It lives
under `yocto3/` on the `feature/yocto3` branch and has not shipped. Its
CHANGELOG and docs say it passes all 143 tests of Microsoft's DI specification
suite (`yocto3/tests/Yocto.Specification.Tests`).

Running that suite repeatedly shows the claim holds only some of the time: the
keyed test `ResolveWithAnyKeyQuery_Constructor_Duplicates` failed in 3 of 4
consecutive runs on a developer laptop. It asserts that two resolves of an
all-singleton `IEnumerable<T>` return the same array, and it fails because of
Problem 1 below. Two more defects come from the same root: yocto lets
registrations change after resolution starts, and some of its cached state does
not track those changes correctly.

This document specifies the changes required before 3.0.0 is tagged on `main`.

## The Problem

MEDI's service provider is immutable once built. yocto's is not: `Register`,
`RegisterEnumerable`, `Unregister`, `IRegistration.Remove`, and `WithLifetime`
all work after resolution has started. The design rule this document adopts is:

> A resolve behaves as it would on a freshly built container that has the same
> registrations.

Three defects break that rule.

1.  **The enumerable cache uses one process-wide version counter.**
    `Container.s_version` is `static`. A registration change in any container
    in the process discards the cached `IEnumerable<T>` arrays of every other
    container. When xUnit runs test classes in parallel, a registration in one
    test invalidates another test's cache between its two resolves, and
    `Assert.Same` fails. Applications that build several containers at once
    (test hosts, one container per tenant) get correct values but lose array
    identity and allocate new arrays.
2.  **Constructor selection depends on which container resolves first, and is
    never revisited.** `ConstructorActivator` picks the greediest satisfiable
    constructor on the first resolve and caches that plan on the registration
    forever. Consequences:
    *   If a child container resolves a root-owned registration first, the plan
        can require a dependency only the child has. Later resolves from the
        root throw "Unable to resolve".
    *   If a dependency is unregistered after the first resolve, the plan still
        requires it and resolves throw, although a smaller constructor would
        work.
    *   If a dependency is registered after the first resolve, the greedier
        constructor is never used. `CHANGELOG.md` currently documents this
        ("Constructors are selected on first resolve"); this design reverses
        that documented behavior before 3.0.0 ships, because it violates the
        rule above.
3.  **`Unregister` races with `RegisterEnumerable` and `Remove`.** Those two
    operations read the stored array under `_lock`, build a new array from it,
    and write it back. `Unregister` removes the entry without taking `_lock`.
    If `Unregister` runs between the read and the write-back, the write-back
    restores the unregistered registrations, including ones whose instances
    `Unregister` has already disposed. (`Register` is not affected: it writes a
    one-element array regardless of what it read.)

## Goals

*   The MEDI specification suite passes 50 of 50 consecutive runs with xUnit's
    default parallelization.
*   A registration change in one container never invalidates the enumerable
    cache or constructor-plan cache that belongs to an unrelated container
    (one that is neither an ancestor nor a descendant).
*   Constructor selection follows the rule in The Problem: after any
    registration change visible to the registration's owner, the next
    activation selects against the current registrations.
*   Resolving first from a child container never causes a later resolve from
    the parent to fail.
*   After concurrent `Unregister` and `RegisterEnumerable` on the same service,
    the container holds exactly the registrations that one of the two orderings
    (unregister then register, or register then unregister) would produce.
*   Each defect has a test in `Yocto.Tests`. Tests that can fail
    deterministically on the old code do so; the concurrency test is a guard
    test (see Verification Plan).

## Non-Goals

*   Making the container immutable after build.
*   Changing disposal error handling (see Open Questions).
*   Detecting scoped services resolved from the root container. yocto treats
    every container as a scope by design.
*   Running scoped activation outside `_scopedLock`. `GetOrAddScoped` holds the
    lock while activating, as MEDI does per scope.
*   Changing pooled semantics. `PooledInstanceFactory` hands out instances
    round-robin without checkout, as 2.x did.
*   Performance work beyond the regression budget in Non-Functional
    Requirements.
*   Changes to `Yocto.Wpf`, the docs site, or packaging.

## Background: the parts of yocto 3 this design changes

```text
 root Container ── _registrations: (ServiceType, Key) → Registration[]
   │               _enumerables:   (ServiceType, Key) → (version, cached array)
   │
   └─ child Container (CreateChildContainer or IServiceScopeFactory.CreateScope)
        lookup miss → falls back to parent, then grandparent, …

 Registration ── Owner: the container it was registered in
              ── ConstructorActivator: cached constructor plan
              ── IInstanceFactory: one per lifetime
```

*   **Registrations** are stored per container, keyed by service type and key.
    `Register` replaces the stored array; `RegisterEnumerable` appends to it.
*   **Lookup** walks from the resolving container up through its ancestors and
    uses the first container that has a match.
*   **Activation container.** Singleton, PerThread, and Pooled instances are
    activated against the registration's owner. Multiple (a new instance per
    resolve, MEDI's "transient") and Scoped instances are activated against the
    resolving container. Activation resolves each constructor parameter from
    that container.
*   **Constructor plan.** A plan is the chosen constructor plus a description
    of each parameter (type, service key, default value).
*   **Registration sequence.** Each registration also gets a process-wide
    sequence number. It only orders registrations ("last registered wins");
    it is never used to decide whether a cache is fresh, so it is not part of
    Problem 1.

## Technical Plan (Architecture)

### Change 1: Per-container registration versions

Each container keeps its own version counter. A registration change increments
the counter of the container where the change happened, and of no other
container. The changes that increment it are: adding a registration
(`Register`, `RegisterEnumerable`), removing registrations (`Unregister`,
`IRegistration.Remove`, disposal of a container that still has registrations),
and changing a registration's lifetime (`WithLifetime`).

A container's lookups depend on its own registrations and those of every
ancestor. Its **registration stamp** is therefore the sum of the version
counters of the container and all its ancestors. Counters only increase, so
the stamp increases whenever any counter in the chain changes and never returns
to an earlier value. Descendants and unrelated containers do not contribute.

The enumerable cache stores the stamp it was built at and is reused only while
the container's current stamp is equal. Computing the stamp reads one counter
per level of the parent chain without locking. Typical depth is 1 (root plus
one scope); ASP.NET Core request scopes are always depth 1 because
`IServiceScopeFactory` creates them as direct children of the root.

### Change 2: Owner-first constructor selection, revisited on change

Constructor selection uses the registrations visible to the **registration's
owner**, not the container that happens to resolve first. This makes the plan
the same no matter which container resolves first, and matches how Singleton,
PerThread, and Pooled registrations already activate.

The cached plan records the owner's registration stamp at the time of
selection. On each activation, if the owner's current stamp differs, selection
runs again against the current registrations. Selection repeats only after a
registration change in the owner or one of its ancestors.

**Fallback for child-only dependencies.** A root-owned registration may depend
on a service that only child containers register, for example a per-request
`IRequestContext` registered in each child. The owner can satisfy no
constructor for it, but the child can, and this works today. When owner
selection finds no satisfiable constructor, selection runs against the
resolving container instead. That plan is cached per resolving container, keyed
weakly so a disposed child is not kept alive, and stamped with that container's
registration stamp.

**Behavior changes**, recorded in the CHANGELOG:

*   A dependency registered after the first resolve is picked up on the next
    activation.
*   A child's registrations no longer widen the constructor chosen for a
    parent-owned registration, unless the parent can satisfy no constructor
    (fallback above). Today this widening depends on resolve order.
*   A constructor ambiguity ("none is a superset of the others") is reported
    when it first exists in the current registrations, which can now be after
    the first resolve. This is the same error a freshly built container would
    report.

**Concurrency contract.** The cached plan and its stamp are one immutable
object published atomically. Two threads may both select at the same time;
both results are valid for the stamp they record, and the last write wins.

**Cost.** The unchanged path (no registration change since the last selection)
adds one stamp computation per activation. Re-selection repeats the
satisfiability check per parameter, but not reflection over constructors and
parameter attributes, which is computed once per implementation type.

### Change 3: `Unregister` under the registration lock

`Unregister` takes `_lock` around its removal, as `Add` and `Remove` already
do. Disposal of the removed registrations' instances stays outside the lock,
matching `Add` and `Remove`. A lock-free removal would not help: the
read-then-write-back in `RegisterEnumerable` and `Remove` would still restore a
stale array.

## Alternatives Considered

### Alternative A: Disable xUnit parallelization in the specification tests

*   **Pros**: One-line change; the suite becomes green.
*   **Cons**: The defect remains for any process that builds containers
    concurrently.
*   **Reason for Rejection**: Fixes the symptom, not the cause.

### Alternative B: Cache enumerables only in "built" containers

*   **Pros**: Matches MEDI; no version tracking.
*   **Cons**: yocto has no built state; `new Container()` is public and
    mutable. Adding a freeze flag changes the public model and still needs a
    rule for children seeing parent changes.
*   **Reason for Rejection**: Larger API change than the defect warrants.

### Alternative C: Parents notify children of changes

*   **Pros**: No parent-chain walk on cache reads.
*   **Cons**: Each parent must hold a list of live descendants, including every
    short-lived request scope, add and remove them under a lock, and lock each
    one during notification.
*   **Reason for Rejection**: The version-sum stamp gives the same result with
    lock-free reads and no subscription bookkeeping.

### Alternative D: Keep "selected on first resolve" and only fix the child-first failure

*   **Pros**: Smallest change; keeps the current CHANGELOG wording.
*   **Cons**: Plans still go stale after `Unregister` (resolves throw although a
    smaller constructor works) and after late registrations. Violates the
    fresh-container rule.
*   **Reason for Rejection**: Owner decision (2026-11-02) to adopt the
    fresh-container rule for 3.0.0.

### Alternative E: Cache the plan per resolving container

*   **Pros**: A child's registrations can widen a parent-owned registration's
    constructor.
*   **Cons**: Requests that alternate containers re-select every time; the
    cache must either hold strong references to disposed children or use weak
    references on the hot path; the plan depends on resolve order.
*   **Reason for Rejection**: More cost and more failure modes than owner-first
    selection, which only uses the resolving container as a fallback.

## Non-Functional Requirements

*   **Thread safety**: Version counters are incremented atomically and read
    without locks. A resolve that races a registration change sees either the
    old or the new registrations, never a mix.
*   **Performance**: Multiple-lifetime resolves and cached `IEnumerable<T>`
    resolves, from a root and from a depth-2 child, must not regress by more
    than 5% against `59fcb74`. Measured with a throwaway BenchmarkDotNet
    harness outside the repository; results go in the pull request.
*   **Compatibility**: No public API changes. Behavior changes are listed in
    Change 2 and recorded in the CHANGELOG.
*   **Rollout**: Ships in 3.0.0. No released 3.x exists.

## Open Questions

*   **Disposal exceptions**: Disposal swallows exceptions from individual
    instances; MEDI lets them propagate. Should 3.0 collect them and throw one
    `AggregateException` after disposing everything? Not changed here.

## Risks

*   **Ambiguity surfaces later**: see Change 2 behavior changes. Mitigation:
    `Validate()` reports it at startup when called.
*   **Race test is probabilistic**: it guards the lock but may pass on the old
    code. Mitigation: the lock change is reviewed by inspection.
*   **Stamp walk on deep hierarchies**: cost grows with depth. Mitigation:
    benchmark at depth 2; typical depth is 1.

## Additional Resources

*   [`yocto3/CHANGELOG.md`](../../yocto3/CHANGELOG.md)
*   [`yocto3/src/Yocto/Container.cs`](../../yocto3/src/Yocto/Container.cs)
*   [`yocto3/src/Yocto/ConstructorActivator.cs`](../../yocto3/src/Yocto/ConstructorActivator.cs)
*   [`yocto3/tests/Yocto.Specification.Tests`](../../yocto3/tests/Yocto.Specification.Tests)
*   Review reports: [`reviews/yocto3-release-hardening/`](reviews/yocto3-release-hardening/)

--------------------------------------------------------------------------------

## Detailed Implementation Plan

### Target Files to Create/Modify

#### 1. `yocto3/src/Yocto/Container.cs` (Modify)

*   Remove `private static long s_version`. Add `private long _version`.
*   `internal static void OnRegistrationsChanged()` becomes
    `internal void OnRegistrationsChanged()`:
    `Interlocked.Increment(ref _version)`.
*   Add `internal long RegistrationStamp`: loop `c = this` up through `_parent`,
    summing `Volatile.Read(ref c._version)`.
*   `Add`: call `OnRegistrationsChanged()` (instance) where it calls the static
    one today.
*   `Release(Registration[])` becomes a private instance method. If the array
    is non-empty it calls `OnRegistrationsChanged()` once, then for each
    registration calls `ReleaseDerived` on `this` and `Release()` on the
    registration. All callers (`Add`, `Unregister`, `Remove`, `Dispose`,
    `DisposeAsync`) pass registrations owned by `this`.
*   `ResolveEnumerable`: read `RegistrationStamp` instead of `s_version`.
*   `Unregister`: perform `_registrations.TryRemove` inside `lock (_lock)`;
    call `Release` after the lock.

#### 2. `yocto3/src/Yocto/Registration.cs` (Modify)

*   `WithLifetime`: replace `Yocto.Container.OnRegistrationsChanged()` with
    `Owner.OnRegistrationsChanged()`.

#### 3. `yocto3/src/Yocto/ConstructorActivator.cs` (Modify)

*   Compute constructor descriptions once, lazily: an array of
    `(ConstructorInfo, Dependency[])` for the implementation type.
*   Cache `ConstructorInvoker` per constructor lazily (created only when that
    constructor is selected).
*   Replace `Plan? _plan` with `Plan? _ownerPlan`, where `Plan` gains a
    `long Stamp` field. `Plan` remains an immutable sealed record; publish with
    `Volatile.Write`, read with `Volatile.Read`.
*   Add `ConditionalWeakTable<Container, Plan> _scopePlans` for the fallback.
*   `GetPlan(IContainer scope)`:
    1.  `owner = _registration.Owner`; `stamp = owner.RegistrationStamp`.
    2.  If `_ownerPlan` is non-null and its `Stamp == stamp`, return it.
    3.  Try selection against `owner`. On success, publish and return it.
    4.  If no constructor is satisfiable from `owner` and `scope` is a
        `Container` other than `owner`: look up `_scopePlans[scope]`; if its
        stamp equals `scope.RegistrationStamp`, return it; otherwise select
        against `scope`, store with `AddOrUpdate`, return it.
    5.  Otherwise throw the existing "Could not find a constructor" error.
    Ambiguity errors from step 3 are thrown, not converted to fallback.
*   `Create(scope)` and `GetDependencies(scope)` both call `GetPlan(scope)`.

#### 4. `yocto3/tests/Yocto.Tests/RegistrationTests.cs` (Modify)

*   `Constructor_GreediestResolvableWins`: delete the second `c.Register<Greedy>()`
    (the workaround that cleared the stale plan), so the test asserts that
    registering `IAnimal` after the first resolve yields `Constructor == 1`.
*   Add `Constructor_IsReselected_AfterDependencyUnregistered`: register
    `Greedy`, `IAnimal → Dog`, `IPerson → Person`; resolve (2);
    `Unregister<IPerson>()`; resolve (1).
*   Add `ChildFirstResolve_DoesNotBreakParent`: root registers `Greedy` and
    `IAnimal → Dog`; child registers `IPerson → Person`. Resolve from child,
    then root; both return `Constructor == 1`, neither throws.
*   Add `ChildOnlyDependency_FallsBackToResolvingContainer`: root registers
    `Holder` (needs `Disposable`); child registers `Disposable`. Resolve from
    child succeeds; `CanResolve<Holder>()` from root stays resolvable as
    registered, and resolving `Holder` from root throws.
*   Add `Validate_FromChild_DoesNotBreakParentResolve`: same setup as
    `ChildFirstResolve_DoesNotBreakParent`, but call `child.Validate()` first.
*   All use the default (Multiple) lifetime.

#### 5. `yocto3/tests/Yocto.Tests/ContainerTests.cs` (Modify)

*   Add `EnumerableCache_IsNotInvalidatedByUnrelatedContainer`: in container A,
    register two `IAnimal` singletons with `RegisterEnumerable`; resolve
    `IEnumerable<IAnimal>` twice around (a) a registration in a separate
    `new Container()` B, (b) `WithLifetime` on a registration in B,
    (c) disposing B. Assert the same array instance each time. Fails on the old
    code.
*   Add `EnumerableCache_SeesParentRegistration` (guard test): resolve from a
    child, `RegisterEnumerable` a new singleton in the parent, resolve again;
    assert the new instance is included.
*   Add `Unregister_ConcurrentWithRegisterEnumerable_IsLinearizable` (guard
    test): 2,000 iterations; each registers `IAnimal → Dog` via
    `RegisterEnumerable`, then runs `Unregister<IAnimal>()` and
    `RegisterEnumerable<IAnimal, Cat>()` in parallel; assert the resolved
    `IEnumerable<IAnimal>` is either `[Cat]` or empty.

#### 6. `yocto3/CHANGELOG.md` (Modify)

*   Replace "Constructors are selected on first resolve: the greediest
    satisfiable constructor wins. Registration order no longer matters." with:
    "The greediest constructor satisfiable from the registration's container
    wins, and is selected again after registrations change. Registration order
    no longer matters."

### Verification Plan

*   **Build**: `dotnet build yocto3/Yocto.slnx -c Release` (warnings are errors
    via `Directory.Build.props`). Pass: exit code 0.
*   **Unit tests**: `dotnet test yocto3/Yocto.slnx -c Release`. Pass: 0 failed.
*   **Specification stability**: run
    `dotnet test yocto3/tests/Yocto.Specification.Tests -c Release` 50 times in
    a shell loop. Pass: 0 runs with failures.
*   **Regression proof**: with only the new tests applied on top of `59fcb74`,
    `EnumerableCache_IsNotInvalidatedByUnrelatedContainer`,
    `Constructor_GreediestResolvableWins`,
    `Constructor_IsReselected_AfterDependencyUnregistered`, and
    `ChildFirstResolve_DoesNotBreakParent` fail. Guard tests may pass.
*   **Native AOT**: `dotnet publish yocto3/samples/Yocto.Sample -c Release -o aot`
    produces no warnings, and `./aot/Yocto.Sample` exits with code 0.
*   **Performance**: the benchmark in Non-Functional Requirements, numbers in
    the pull request.

--------------------------------------------------------------------------------

*Drafted with EGM*

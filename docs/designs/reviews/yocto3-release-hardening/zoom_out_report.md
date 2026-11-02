# Goldfish Review Report

**Target:** `/Users/rich/.openclaw/projects/yocto/docs/designs/yocto3-release-hardening.md` (`Lines 1-407`)

## Reviewer Status

| Reviewer | Status |
| --- | --- |
| `egm-zoom-out` | completed |

**Reviewer verdict:** The design is mostly sound. Change 1 (per-container versions) is the right size for the flaky spec test's actual cause. Change 2 is more complex than it needs to be. Change 3 describes the race wrongly, so its planned regression test would not fail on the current code.

**Severity breakdown:** 🔴 0 Blocker · 🟡 3 Major · 🟢 3 Nit

## Prioritized Findings (Blockers -> Major -> Nits)

### ZO-1: Change 2 should select constructors against the registration's owner, not the resolving container - Severity: 🟡 Major
* **Reviewer Lens:** `egm-zoom-out`
* **Affected Scope:** `Lines 212-237` (Change 2), `131-138` (CUJ 3), `331-333`, `347-356` (implementation plan)
* **Discrepancy Description**: The plan cache is keyed on the resolving container, which then needs a "nearest container with a non-zero `_version`" rule to keep request scopes fast. Requests that alternate between containers re-select the constructor every time, and this hits the Multiple (transient) lifetime, the busiest activation path. Lines 227-228 say the cost is negligible because scoped and singleton lifetimes activate once. But Singleton, PerThread and Pooled already activate against `context.Owner` (`InstanceFactories.cs` lines 64 and 176), so that reasoning is backwards. CUJ 3 also has a hidden risk: a registration owned by the parent picks up a dependency that exists only in the child, which is the captive dependency that `Validate()` flags.
* **Rationale & Recommendation**: Select the constructor against `Registration.Owner` and cache `(Stamp, Plan)`, where the stamp is `Owner.RegistrationStamp` from Change 1. This drops `RegistrationScope`, the non-zero-version rule, the scope reference check and the alternation cost, and it matches what the parent-owned lifetimes already do. CUJ 2 is still fixed, and CUJ 3 is fixed with neither resolve throwing. Change CUJ 3's expected result so the child instance uses `(IStore)`, or explain why the child's extra registrations should change the shape of a registration owned by the parent.

### ZO-2: Change 3 describes the race wrongly, so CUJ 4 and its regression test won't catch it - Severity: 🟡 Major
* **Reviewer Lens:** `egm-zoom-out`
* **Affected Scope:** `Lines 52-56`, `140-148` (CUJ 4), `368-370`, `398-399`
* **Discrepancy Description**: `Register` replaces the stored array, so `Add` writes `[registration]` whatever it read (`Container.cs` line 140). When it races `Unregister`, the final state always matches one of the two serial orders, and nothing comes back from the dead. The worst outcome is a second `Release` of the old array. The resurrection the doc describes only happens with `RegisterEnumerable` (it writes `[.. existing, registration]`) or `IRegistration.Remove` (it writes `remaining`). Both read the array under `_lock`, while `Unregister` removes it without the lock. So the planned `Unregister_ConcurrentWithRegister_IsLinearizable` test, written with `Register`, passes on the current code. That breaks the doc's own goal at line 75 and its verification step at lines 398-399. A test that only loops many times is also unreliable as regression proof.
* **Rationale & Recommendation**: Restate the race in terms of `RegisterEnumerable` and `Remove`, and rewrite CUJ 4 and the test to match. Either say plainly that the test only fails some of the time, or drop the "fails on current code" requirement for this change, since the fix is a one-line lock that matches the surrounding code.

### ZO-3: The performance requirement measures a path the design doesn't change - Severity: 🟡 Major
* **Reviewer Lens:** `egm-zoom-out`
* **Affected Scope:** `Lines 290-293`
* **Discrepancy Description**: Resolving a cached singleton returns `_instance.Value` (`InstanceFactories.cs` line 96) and never reaches the stamp or plan code. The paths that do change are `IEnumerable<T>` resolves (the stamp walk up the parent chain) and transient activation (the plan stamp check). Because the 5% gate is on the wrong path, a slowdown would go unmeasured. A benchmark harness that isn't checked in also can't be rerun.
* **Rationale & Recommendation**: Set the gate on transient resolves and cached-enumerable resolves, measured from a root container and from a container one or two levels down. Check in the benchmark harness so the gate can be rerun.

### ZO-4: CUJ 2 changes documented behavior; it isn't a defect fix - Severity: 🟢 Nit
* **Reviewer Lens:** `egm-zoom-out`
* **Affected Scope:** `Lines 44-47`, `273-280` (Alternative D)
* **Discrepancy Description**: `CHANGELOG.md` line 24 says "Constructors are selected on first resolve", and the current code does exactly that. Alternative D quotes the changelog as if it already promised re-selection.
* **Rationale & Recommendation**: Describe re-selection after late registrations as a deliberate change to the 3.0 behavior. The planned changelog edit at lines 385-389 already implies this. Keep "a child-first resolve breaks the parent" as the actual defect.

### ZO-5: Pick one approach for `Release` - Severity: 🟢 Nit
* **Reviewer Lens:** `egm-zoom-out`
* **Affected Scope:** `Lines 204-206`, `335-336`
* **Discrepancy Description**: The doc leaves two options open. Every caller of `Release` (`Add`, `Unregister`, `Remove`, `Dispose`) passes registrations owned by the same container, and derived registrations keep the source's `Owner` (`Registration.cs` line 100). So the "per distinct owner" option adds nothing.
* **Rationale & Recommendation**: Make `Release` an instance method that bumps `this._version` once.

### ZO-6: Two "Open Questions" are already decided - Severity: 🟢 Nit
* **Reviewer Lens:** `egm-zoom-out`
* **Affected Scope:** `Lines 99-108`, `312-314`
* **Discrepancy Description**: The scoped-lock and pooled-semantics items already say "recorded, not fixed" and "no code change", so they aren't open. The `Lazy<object>` entry under Future Improvements designs a fix for a problem the doc says MEDI shares.
* **Rationale & Recommendation**: Move both items to Non-Goals and keep only the disposal-exceptions question open.

## Notes

* The reviewer found no reuse gaps: the design adds no new utilities, and the stamp is built on the existing parent-chain walk.
* Change 1 and Alternatives A-C need no changes.

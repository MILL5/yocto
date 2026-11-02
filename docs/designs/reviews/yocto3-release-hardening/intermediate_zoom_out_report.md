# Goldfish Review Report — Stage 8 (Intermediate Zoom Out)

**Target:** `/Users/rich/.openclaw/projects/yocto/docs/designs/yocto3-release-hardening.md` (lines 1-407)

## Reviewer Status

| Reviewer | Status |
| --- | --- |
| `egm-zoom-out` | completed |

**Verdict:** Changes 1 and 3 are the right size. The main bloat risk comes from applying the earlier reports' fixes one by one. Most of the additions the Critic report asks for exist only because Change 2 keys the plan cache on the resolving container. Settle that question first. No Blockers.

## Prioritized Findings (Blockers -> Major -> Nits)

### ZO8-1 Settle Change 2's shape before applying the Critic additions - Severity: 🟡 Major
* **Reviewer Lens:** `egm-zoom-out`
* **Affected Scope:** Lines 212-237, 245-280, 284-297 (plus the Risks & Mitigations section the Critic report proposes)
* **Discrepancy Description**:
  * Taken together, the Critic report would add:
    * a Risks & Mitigations section with about 9 items
    * Alternative E (a per-container plan cache)
    * a torn-read fix for the cache entry
    * a multi-scenario benchmark that is checked in
    * a policy for ambiguity on re-selection
    * a definition of "registration-bearing container"
  * Applied together, these would roughly double the doc.
  * Five of the nine risks only exist because the plan is cached against the resolving container:
    * alternation cost
    * the sticky "non-zero `_version`" rule
    * concurrent re-selection across scopes
    * cross-scope tearing
    * the "registration-bearing container" invariant (also Comprehension C4)
  * Owned lifetimes already activate against `context.Owner` (`InstanceFactories.cs:64`, `:176`). That supports ZO-1's alternative: selecting against `Registration.Owner` removes these risks instead of mitigating each one.
* **Rationale & Recommendation**:
  * Adopt ZO-1 first.
  * Record the current resolving-container design as the rejected Alternative E, reusing the existing text.
  * Cut Risks & Mitigations down to one line each for what is left:
    * the O(depth) stamp read on enumerable cache hits
    * ambiguity on re-selection (Critic design-1-4)
    * how reliable the race test is
    * security: none

### ZO8-2 Late re-selection (CUJ 2) goes beyond "minimum set of changes" - Severity: 🟡 Major
* **Reviewer Lens:** `egm-zoom-out`
* **Affected Scope:** Lines 23-24, 44-51, 69-70, 122-129, 212-228, 385-389
* **Discrepancy Description**:
  * The actual defect is CUJ 3: a child-first resolve breaks the parent. Selecting against the owner fixes that with the existing `_plan ??=` cache and no stamp.
  * The stamp is needed only for CUJ 2. CUJ 2 is a behavior change that reverses CHANGELOG line 24 ("selected on first resolve").
  * Because of CUJ 2, the design carries the stamp, the runtime ambiguity risk, the re-selection cost analysis and the extra benchmark scenarios.
  * The doc says it specifies the "minimum set" of changes but never argues why CUJ 2 must ship in 3.0.0.
  * Owner-based selection is also a semantic change that the doc does not state: a child's registrations can no longer widen the constructor of a parent-owned Multiple or Scoped registration. Today that widening is nondeterministic, so no reliable capability is lost.
* **Rationale & Recommendation**: Pick one option.
  * (a) Keep CUJ 2. Justify it in one sentence: a plan goes stale after `Unregister` or a late registration, which the line 29 principle covers. Label it as a deliberate change to 3.0 behavior.
  * (b) Move CUJ 2 to Future Improvements. Fix CUJ 3 with owner-based selection only, and keep the CHANGELOG wording.
  * Either way, record the owner-selection semantics in Problem item 2, CUJ 3's expected result, and the CHANGELOG edit.

### ZO8-3 Checking in a benchmark harness is scope creep - Severity: 🟢 Nit
* **Reviewer Lens:** `egm-zoom-out`
* **Affected Scope:** Lines 89-90, 290-293
* **Discrepancy Description**:
  * ZO-3 and Critic design-1-3 both ask for a checked-in BenchmarkDotNet harness.
  * No benchmark project exists under `yocto3/`.
  * Adding one brings a new project into `Yocto.slnx`. That project then falls under the `-warnaserror` build and the solution-wide test run.
  * Non-Goals exclude performance work and packaging.
* **Rationale & Recommendation**:
  * Keep the harness throwaway, but specify the scenarios and a baseline commit (static `s_version`).
  * After ZO8-1, the scenarios are transient resolve and cached `IEnumerable<T>` resolve, at depth 0 and depth 2.
  * Record the numbers in the PR, not in the design.

### ZO8-4 The proposed Alternative F for Change 3 would not fix the race - Severity: 🟢 Nit
* **Reviewer Lens:** `egm-zoom-out`
* **Affected Scope:** Lines 239-243
* **Discrepancy Description**:
  * Critic structural-1-2 asks for Alternative F, a lock-free compare-and-remove using `TryRemove(KeyValuePair)`.
  * That would not fix the race. `RegisterEnumerable`/`Remove` read the array under `_lock` and write a derived array back afterwards (`Container.cs:87-95`, `:137-140`), so they can still overwrite the removal.
  * A full Pros/Cons/Rejection block would add text for a non-option.
* **Rationale & Recommendation**: Add one sentence to Change 3: "Taking `_lock` matches `Add` and `Remove`; lock-free removal cannot prevent the read-modify-write in `Add`/`Remove` from writing back a stale array."

### ZO8-5 Implementation detail has leaked into the Technical Plan - Severity: 🟢 Nit
* **Reviewer Lens:** `egm-zoom-out`
* **Affected Scope:** Lines 202-206, 208-210, 214, 286-288
* **Discrepancy Description**:
  * The feature-level sections name static vs. instance methods, `r.Owner`, the tuple's field order, `Volatile.Read` and `Interlocked.Increment`.
  * The Detailed Implementation Plan (lines 322-356) already covers these.
  * This duplication causes three earlier findings:
    * Comprehension C10 (the tuple is written in two orders)
    * C3/ZO-5 (the open `Release` choice)
    * Critic design-1-2 (tearing)
* **Rationale & Recommendation**:
  * At feature level, state only the contracts:
    * change notifications go to the owning container
    * the plan cache entry is immutable and published atomically
  * Keep type and method choices only in lines 322-356. Removing the duplicated text resolves the three findings above.

### ZO8-6 "How yocto 3 works today" describes more than the changes touch - Severity: 🟢 Nit
* **Reviewer Lens:** `egm-zoom-out`
* **Affected Scope:** Lines 154-187
* **Discrepancy Description**:
  * These parts of the section are unrelated to the three changes:
    * `Application.Current`/Push/Pop
    * `_scoped` and `_disposables`
    * the six-lifetime list
    * the Disposal and Validation bullets
  * Comprehension C9 asks to define these terms, which would add more text about things the design does not change.
  * The section also leaves out the fact Change 2 depends on: which lifetimes activate against the owner and which against the resolving container.
* **Rationale & Recommendation**:
  * Cut the section down to registrations, the parent fallback, `_enumerables` and `ConstructorActivator`.
  * State the owner-vs-resolver activation split once.
  * Drop the rest instead of defining it.

### ZO8-7 Goal 2 promises more than the design delivers - Severity: 🟢 Nit
* **Reviewer Lens:** `egm-zoom-out`
* **Affected Scope:** Lines 66-68
* **Discrepancy Description**:
  * "Never changes the observable behavior … of an unrelated container" is broader than Change 1.
  * The `Lifetimes` registry, `Application.Current`, and the process-wide registration sequence number all stay shared across containers.
  * The only test for this goal (line 362) checks the identity of the cached enumerable.
* **Rationale & Recommendation**: Narrow the goal to "never invalidates the enumerable or constructor-plan caches of an unrelated container."

## No New Issues Found In

* Change 1's sum-of-versions stamp
* Change 3's lock
* Alternatives A-D
* The verification plan, apart from items already raised in earlier stages

The design adds no new utilities, so there are no reuse gaps.

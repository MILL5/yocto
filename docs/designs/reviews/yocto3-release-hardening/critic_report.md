# Goldfish Review Report — Stage 7 (Critic Review)

**Target:** `/Users/rich/.openclaw/projects/yocto/docs/designs/yocto3-release-hardening.md` (FeatureRange, lines 1-317)

**Standard:** `feature-spec-standard.md` (egm-design, egm-structural); egm-style used its persona only.

**Context checked:** `yocto3/src/Yocto/` (Container.cs, ConstructorActivator.cs, Registration.cs, InstanceFactories.cs), `yocto3/CHANGELOG.md`

## Reviewer Status

| Reviewer | Status |
| --- | --- |
| `egm-design` | completed |
| `egm-structural` | completed |
| `egm-style` | completed |

**Totals after deduplication:** 🔴 0 Blocker · 🟡 10 Major · 🟢 14 Nit

The reviewers confirmed these source claims as accurate: `s_version` is private static (Container.cs:19), `OnRegistrationsChanged` is internal static (Container.cs:102), `Unregister` skips `_lock` while `Add` and `Remove` take it, `Release` is private static (Container.cs:148), `WithLifetime` calls the static notifier (Registration.cs:120), the plan is cached once with `_plan ??=`, and the relative links resolve. The sum-of-versions stamp in Change 1 is sound.

---

## 🔴 Blockers

None.

---

## 🟡 Major

### egm-design-1-1 — The Unregister race happens through `RegisterEnumerable`, so CUJ 4's test would pass on today's code
* **Reviewer Lens:** `egm-design`
* **Affected Scope:** Lines 52-56, 73-76, 140-148
* **Issue:** `Container.Add` with `replace: true` (`Register`) writes `[registration]` and never writes the old array back. So CUJ 4's `Register<IPlugin, PluginB>()` already ends in one of the two serial states on current code. The only side effect is a double `Release`, which is harmless. The service only comes back to life with `replace: false` (`RegisterEnumerable`, which writes `[.. existing, registration]`). In that case a singleton's `Lazy` hands out an instance that was already disposed. As written, CUJ 4 cannot produce the regression test that line 75 requires ("fails on the current code").
* **Suggested fix:** Restate the problem and CUJ 4 in terms of `RegisterEnumerable`. Say how the race test will fail reliably on current code: either a stress loop with N iterations and a stated failure rate, or a deterministic test hook.

### egm-design-1-2 — The constructor-plan cache entry can be read half-updated
* **Reviewer Lens:** `egm-design`
* **Affected Scope:** Lines 214-221, 286-289
* **Issue:** If the `(long Stamp, Container Scope, Plan Plan)` entry is a value tuple stored in a field, concurrent `Create` calls from different scopes can each write part of it. A reader could then pair one scope's stamp with another scope's plan. That produces the stale plan this change is meant to prevent, and it contradicts "never a torn one" (line 288). Today's `_plan ??=` is safe only because it is a single reference.
* **Suggested fix:** Make the entry an immutable reference type (a sealed class or record). Publish and read it with `Volatile.Write`/`Volatile.Read` or `Interlocked.CompareExchange`.

### egm-design-1-3 — Re-selection cost is understated and the benchmark doesn't measure it (merged: egm-style-1-5, egm-style-1-6)
* **Reviewer Lens:** `egm-design`, `egm-style`
* **Affected Scope:** Lines 40-42, 223-228, 290-293
* **Issue:** Each re-selection repeats `GetConstructors()`, the attribute reads in `Describe`, `CanResolve` for every parameter, and `ConstructorInvoker.Create`. "Negligible" and "bounded by the cost of the first resolve" are not measured, and it's unclear whether the bound is per resolve or in total. Take a default (Multiple-lifetime) service resolved alternately from per-tenant child containers that have their own registrations, which is the motivating multi-tenant case. It would re-select on nearly every resolve. Lines 227-228 only cover Scoped and Singleton. The benchmark measures only cached-singleton resolves, so it cannot detect a regression in any of the three changes. It is also a "throwaway ... session scratch directory" harness that nobody else can reproduce.
* **Suggested fix:** State the cost per resolve and which lifetimes pay it, and replace "negligible" with a measured figure. Benchmark Multiple-lifetime resolves, `IEnumerable<T>` resolves, and resolves that alternate between two registration-bearing containers, against a named baseline commit. Check the harness in or describe it well enough to rebuild. Consider cheaper designs, such as caching `ConstructorInvoker` per `ConstructorInfo` or a cache per container (see egm-structural-1-2).

### egm-design-1-4 — Re-selection can start throwing the ambiguous-constructor error at runtime
* **Reviewer Lens:** `egm-design`
* **Affected Scope:** Lines 66-70, 212-228
* **Issue:** `SelectConstructor` throws "Found more than one constructor ... none is a superset" for tied or incomparable satisfiable constructors. Today a plan is chosen once and that check never runs again. With Change 2, a late registration can make a second constructor satisfiable, so a service that resolved fine before can throw on its next resolve. Lines 69-70 don't cover this.
* **Suggested fix:** State the intended behavior: either throw, or keep the previous plan and report it. Add it to the risks and add a regression test.

### egm-design-1-5 — Required Risks & Mitigations section is missing (merged: egm-structural-1-1)
* **Reviewer Lens:** `egm-design`, `egm-structural`
* **Affected Scope:** Whole range. Fragments are at 92-108, 99-104, 208-210, 223-228 and 284-297.
* **Issue:** The standard requires failure modes, security implications, scalability limits and their mitigations. The doc has no such section, and its risk content is scattered across Change 1, Change 2, Non-Functional Requirements and Open Questions (the known `_scopedLock` deadlock). Several risks are never addressed:
  * concurrency of the plan cache (design-1-2)
  * re-selection cost (design-1-3)
  * new ambiguity failures (design-1-4)
  * how reliable the race test is
  * the extra O(depth) stamp walk on each `IEnumerable<T>` resolve and each activation
  * the "non-zero `_version`" heuristic sticking after a register-then-unregister
  * concurrent re-selection
  * the change in which constructor is chosen for existing callers
  * security (not even stated as "none")
* **Suggested fix:** Add `## Risks & Mitigations` after Alternatives Considered. List each item above with its mitigation, and state the security impact explicitly.

### egm-structural-1-2 — Alternatives only partly covered: none for Change 2's cache shape or for Change 3
* **Reviewer Lens:** `egm-structural` (echoed by `egm-design`)
* **Affected Scope:** Lines 212-243, 245-280
* **Issue:** Alternatives A-C cover Change 1. Alternative D covers *when* the constructor is chosen, not *how the plan is cached*. A cache per registration-bearing container (`ConditionalWeakTable<Container, Entry>` or a small dictionary) is never considered. Change 3 has no alternatives, for example an atomic compare-and-remove such as `TryRemove(KeyValuePair)` instead of the lock. The standard fails a spec that proposes a single path without showing that other viable designs were considered.
* **Suggested fix:** Add Alternative E (a plan cache per container) and Alternative F (lock-free compare-and-remove), each with the reason it was rejected. Label each alternative with the Change it applies to.

### egm-style-1-1 — The CHANGELOG quote isn't verbatim and hides a documented behavior change (merged: egm-design-1-6)
* **Reviewer Lens:** `egm-style`, `egm-design`
* **Affected Scope:** Lines 273-280, 294-295
* **Issue:** CHANGELOG.md:24 actually reads: "Constructors are selected on first resolve: the greediest satisfiable constructor wins. Registration order no longer matters." The doc puts quotation marks around a paraphrase and leaves out "selected on first resolve", which is the exact behavior Change 2 removes. The compatibility requirement says only "No public API changes", which hides a documented behavior change.
* **Suggested fix:** Quote the CHANGELOG exactly. Make the CHANGELOG update a deliverable. Change the compatibility line to say the constructor-selection timing changes.

### egm-style-1-2 — The "three caches" framing doesn't fit defect 3
* **Reviewer Lens:** `egm-style`
* **Affected Scope:** Lines 19-21, 29-31, 52-56
* **Issue:** The doc frames all three defects as caches that never re-check, but defect 3 (`Unregister` racing `Register`) is an unsynchronized mutation, not a stale cache.
* **Suggested fix:** "Two caches do not notice registration changes, and one mutation path is not synchronized." Change line 21 to match.

### egm-style-1-3 — The flaky-test failure rate and its cause can't be verified (merged: egm-design-1-8)
* **Reviewer Lens:** `egm-style`, `egm-design`
* **Affected Scope:** Lines 16-21, 64-65
* **Issue:** "About one run in three on a developer laptop" gives no run count, hardware or command. The cause (`Assert.Same` combined with the global version counter) is asserted without a failing assertion or stack trace. "50 of 50 runs" doesn't say where. If the failure has a second cause, Change 1 alone won't meet the goal.
* **Suggested fix:** "In N runs of `dotnet test ... -c Release` on <machine>, `ResolveWithAnyKeyQuery_Constructor_Duplicates` failed M times". Quote the failing assertion. State where the 50 runs happen (locally and/or in CI).

### egm-style-1-4 — The Change 2 algorithm contradicts itself
* **Reviewer Lens:** `egm-style`
* **Affected Scope:** Lines 218-221 vs 230-234
* **Issue:** Step 1 compares the cached scope to `scope` itself. Line 233 then says the comparison is against the "registration-bearing container". "Scope" means three different things here. Someone implementing the numbered steps would build the wrong comparison.
* **Suggested fix:** Define the registration-bearing container first, then rewrite step 1 against it. Use one name throughout (for example `RegistrationScope`).

---

## 🟢 Nit

### egm-design-1-7 — The child-first problem only applies to the Multiple and Scoped lifetimes
* **Affected Scope:** Lines 47-51, 131-138
* **Issue:** `OwnedInstanceFactory` and `ExternalInstanceFactory` activate from `context.Owner`, so Singleton, PerThread, Pooled and external registrations can't get a plan specific to a child. CUJ 3 never states the lifetime.
* **Suggested fix:** Scope Problem item 2 and CUJ 3 to the Multiple (default) and Scoped lifetimes.

### egm-structural-1-3 — Additional Resources and Future Improvements are H3s under the wrong H2
* **Affected Scope:** Lines 299-314
* **Issue:** Both appear in the outline as children of `## Non-Functional Requirements`.
* **Suggested fix:** Promote both to `##` or group them under `## Appendix`.

### egm-structural-1-4 — Open Questions comes before the system is explained
* **Affected Scope:** Lines 92-108 vs 150-187
* **Issue:** Open Questions refers to `GetOrAddScoped`, `_scopedLock` and `PooledInstanceFactory` before "How yocto 3 works today" introduces them.
* **Suggested fix:** Use the order Overview, Problem, Goals/Non-Goals, CUJs, Technical Plan, Alternatives, Risks, NFRs, Open Questions, Future Improvements.

### egm-structural-1-5 — Open Questions and Future Improvements repeat each other (merged: egm-style-1-16)
* **Affected Scope:** Lines 94-104, 311-314
* **Issue:** "Disposal exceptions: See Open Questions" adds nothing. The `_scopedLock` problem and its `Lazy<object>` fix are in different sections.
* **Suggested fix:** Keep each topic in one place: unresolved decisions go in Open Questions with the proposed fix, and deferred decided work goes in Future Improvements.

### egm-structural-1-6 — The `Release` design is left undecided (merged: egm-style-1-7)
* **Affected Scope:** Lines 204-206
* **Issue:** "Becomes an instance method **or** takes the owner from each registration" leaves the choice open, and "which it already has" has an unclear antecedent. `Add` (Container.cs:142-144) already bumps the version before `Release` bumps it again.
* **Suggested fix:** Choose one, for example "stays static and calls `r.Owner.OnRegistrationsChanged()` once per distinct owner". Also say whether the extra bump in `Add` is removed.

### egm-structural-1-7 — The diagram annotation collides with other lines and is never explained (merged: egm-style-1-15)
* **Affected Scope:** Lines 155-157
* **Issue:** The `Application.Current` note wraps into the `_registrations` lines. It also never appears in the prose.
* **Suggested fix:** Move it to its own block with a one-line prose bullet, or remove it.

### egm-structural-1-8 — Specification test suite reference has no link (merged: egm-style-1-17)
* **Affected Scope:** Line 307 (and the 143-test claim at line 14)
* **Suggested fix:** Link the NuGet package or the dotnet/runtime source path.

### egm-style-1-8 — The depth figure has no stated basis
* **Affected Scope:** Line 209
* **Suggested fix:** Explain where "Typical depth is 1-3" comes from (root to MEDI scope is depth 1, plus one per nested child), or remove it.

### egm-style-1-9 — Informal or filler wording
* **Affected Scope:** Lines 49, 84, 97, 249
* **Suggested fix:** "Stuck with a plan" becomes "the parent's cached plan requires a dependency it cannot resolve". Drop "real". "Surfaces bugs" becomes "reports disposal failures to the caller". "Becomes green" becomes "passes".

### egm-style-1-10 — Inconsistent constructor terminology
* **Affected Scope:** Lines 45-48
* **Suggested fix:** Use either greediest/less greedy or "more/fewer parameters" throughout, not "smaller".

### egm-style-1-11 — "Serial orders" and "current code" are imprecise
* **Affected Scope:** Lines 73-76
* **Suggested fix:** "Equal to the result of one of the two serial orders"; "fails on commit `<sha>`".

### egm-style-1-12 — The argument against Alternative A is overstated
* **Affected Scope:** Lines 250-252
* **Suggested fix:** "Hides a defect that affects any process building or mutating more than one container concurrently."

### egm-style-1-13 — The cons for Alternative C are vague
* **Affected Scope:** Lines 268-269
* **Suggested fix:** Name the specific lock-ordering hazard, or drop the line.

### egm-style-1-14 — The doc says the code makes the claim
* **Affected Scope:** Lines 12-14
* **Suggested fix:** "The 3.0 CHANGELOG describes it as a drop-in ... that passes all 143 tests."

---

*Outside range (not scored):* egm-structural noted that line 399 calls `48437a5` "the current tip", but the branch tip is `59fcb74`.

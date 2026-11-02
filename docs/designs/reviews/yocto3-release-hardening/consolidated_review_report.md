# Consolidated Goldfish Review — yocto 3.0 Release Hardening

Target: `docs/designs/yocto3-release-hardening.md` · Preset: consolidated (Stages 5–9, uninterrupted)
Reviewers: all completed (zoom-out ×2, comprehension, design, structural, style ×2, implementation-readiness, codebase-details).

Totals: 🔴 0 · 🟡 31 · 🟢 41 (before deduplication). Stage reports in this folder.

## Action items (deduplicated) and resolution

| # | Pri | Action | Source findings | Resolution |
|---|---|---|---|---|
| 1 | 🟡 | Select constructors against the registration's owner, not the resolving container | ZO-1, ZO8-1, C4, C7, IR-1-4, IR-1-5, design-1-3 | Applied. Owner decision: owner-first **with re-selection on change** (CUJ 2 kept). Added fallback to the resolving container when the owner can satisfy no constructor (a regression none of the reviewers caught). Resolver-keyed cache recorded as Alternative E. |
| 2 | 🟡 | Justify or drop late re-selection | ZO8-2, ZO-4, C8, style-1-1, CD-1-6 | Kept, justified by the explicit "fresh-container" rule; CHANGELOG reversal stated; Alternative D records the rejected minimal option. |
| 3 | 🟡 | Restate the Unregister race via `RegisterEnumerable`/`Remove`; test is a guard | ZO-2, design-1-1, IR-1-1, style-1-2 | Applied. |
| 4 | 🟡 | Plan cache must be one immutable, atomically published object | design-1-2, IR-1-3, C10 | Applied (contract in Change 2; type in plan). |
| 5 | 🟡 | Ambiguity can surface at runtime | design-1-4 | Recorded as behavior change and risk. |
| 6 | 🟡 | Performance gate measures the wrong path | ZO-3, design-1-3, IR-1-6, ZO8-3 | Gate moved to Multiple and enumerable resolves at depth 0/2; harness stays throwaway, numbers in PR. |
| 7 | 🟡 | Pick one `Release` design | C3, ZO-5, structural-1-6, CD-1-5 | Instance method, bumps once. |
| 8 | 🟡 | Separate regression vs guard tests; fix baseline commit (`59fcb74`) | C1, C2, IR-1-2, CD-1-1 | Applied. |
| 9 | 🟡 | Use existing fakes (`Greedy`), state lifetimes, remove workaround in `Constructor_GreediestResolvableWins` | CD-1-2, CD-1-3, CD-1-4 | Applied. |
| 10 | 🟡 | Test `WithLifetime`, `Validate()` from child, and dispose of unrelated container | CD-1-7 | Applied. |
| 11 | 🟡 | Say which operations bump the version | style(IR)-1-1 | Applied. |
| 12 | 🟡 | Add Risks section; alternatives for Change 2/3 | design-1-5, structural-1-2, ZO8-4 | Short Risks section; Alt E added; Change 3 gets one sentence (lock-free removal does not fix it). |
| 13 | 🟢 | Trim background to what the changes touch; state owner-vs-resolver activation split | ZO8-5, ZO8-6, C9 | Applied. |
| 14 | 🟢 | Narrow Goal 2; move decided "open questions" to Non-Goals | ZO8-7, ZO-6, structural-1-5 | Applied. |
| 15 | 🟢 | Structure, wording, links, commands, pass criteria | structural-1-3/4/7/8, style nits, IR-1-7, CD-1-8/9 | Applied. |

Contested: none.

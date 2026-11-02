Method: dual-agent (A: aebb94a20ccd68023 · B: ab2d53169033c6cb7)

# Impeccable review: yocto docs site (critique + audit)

- **Target:** `yocto3/docs` (Jekyll). Built locally with `jekyll build --baseurl ''` and served on localhost.
- **How it was checked:** rendered in Playwright Chromium at 390px and 1440px, in light and dark. The detector was run against the built HTML, the `_layouts`/`_includes` source and live URLs.
- **Modes:** the home page is a light Persuade (landing) page and every other page is Read (docs).
- **Scope:** this is a refinement of the current design, not a redesign. No site files were edited.
- **Date:** 2026-11-02, branch `feature/yocto3`, including uncommitted changes.

## Fact check

| Claim | Where | Result |
|---|---|---|
| "passes all 143 tests" | `index.md:10`, `index.md:49`, `faq.md:10`, `guides/aspnet-core.md:8`, `_includes/CHANGELOG.md:8` | **True.** `dotnet test tests/Yocto.Specification.Tests` reports Passed 143, Failed 0, Skipped 0, Total 143. |
| "About 1,200 lines of C#" / "~1.2K lines" | `index.md:54`, `index.md:69` | **Borderline.** `src/Yocto/*.cs` has **1,350** non-blank lines, or 1,231 if comment lines (`//`, `///`) are left out. The claim only holds as "lines of code, excluding comments". See F9. |
| "43K+ NuGet downloads" | `index.md:21`, `llms.txt:20` | Not checked; the owner maintains this number (PRODUCT.md). |

## Audit health score

| # | Dimension | Score | Key finding |
|---|-----------|-------|-------------|
| 1 | Accessibility | 3 | Contrast passes AA everywhere (lowest is 4.73). The FAQ `<h3>` inside `<summary>`, the h1→h3 skip on `/guides/`, and nav with no current-page cue on guide pages are the gaps. |
| 2 | Performance | 3 | Pages are about 145KB, and 84% of that is the 122KB `yocto.png` shown at 32px and 112px. An SVG of 641 bytes already exists. |
| 3 | Theming | 4 | Every colour is a custom property with a light and a dark value. Dark mode is a real theme, not an inversion. |
| 4 | Responsive | 2 | `/getting-started/` scrolls sideways at 390px (page is 506px wide). Tables are clipped with no scroll cue. Footer and breadcrumb links are 17–23px tall. |
| 5 | Implementation integrity | 3 | Small and coherent. Non-link cards have a hover state, `.callout` and `.family` are unused, and FAQ text on the wide home page runs to 152 characters per line. |
| **Total** | | **15/20** | **Good** |

**Implementation integrity verdict: pass.** One layout file, one stylesheet of about 130 lines, system fonts and no JavaScript. The code follows its own DESIGN.md: one accent colour, an amber focus ring, hairline borders and no shadows.

- Detector problems confirmed in the browser: `skipped-heading` on `/guides/`, `line-length` on `/` and `/faq/`, and `text-overflow` on `/getting-started/`.
- `flat-type-hierarchy` (×5) is a **false positive**. The scan read h2 as 16px, but the browser computes h2 at 24px and h1 at 44px.
- `design-system-color` (×2) is a false positive. The scanner saw unrendered Liquid and so fell back to black.
- `design-system-font-size` (×5, `site.css:62,80,97,108,120`) is accurate but not worth fixing. Those sizes are the 0.95rem and similar values that DESIGN.md already documents.

## Design health score (Nielsen)

| # | Heuristic | Score | Key issue |
|---|-----------|-------|-----------|
| 1 | Visibility of system status | 2 | On all 8 guide pages, no nav item shows that you are in Guides. |
| 2 | Match system / real world | 3 | "IoC" in the h1 is not spelled out until the FAQ. "Size: Large" is vague. |
| 3 | User control and freedom | 3 | Breadcrumbs and the 404 page work, but guides and getting started stop dead with no next step. |
| 4 | Consistency and standards | 3 | Cards that are not links still light up on hover. The home page lists 6 guides; the index lists 8. |
| 5 | Error prevention | 3 | `Validate()` is pushed early. There is no copy button, so readers select code by hand. |
| 6 | Recognition rather than recall | 3 | Guides are in the nav. Long guides have no list of sections on the page. |
| 7 | Flexibility and efficiency | 2 | No search, no anchor links on headings, no copy button. |
| 8 | Aesthetic and minimalist design | 3 | Clean overall, but the ✅ emoji brings in a second hue and the home page makes the same case twice (cards, then table). |
| 9 | Error recovery | 3 | The 404 page has three useful ways out. |
| 10 | Help and documentation | 3 | The FAQ and guides are task-focused. With no search, the FAQ is the only way to look things up. |
| **Total** | | **28/40** | **Good (70%)** |

## Design specificity verdict

**What the reviewer saw:** the content is clearly about yocto, but the visual language is generic, "a well-made README". DESIGN.md chooses that on purpose and it suits a developer tool. Craft is good: no stray colours, matched dark mode, and a deliberate amber focus ring.

- **Missed opportunity:** the logo is nested containers (a box in a box in a box), and that idea appears nowhere else on the site. Child containers are the feature yocto has and the built-in container lacks, yet they get one card out of six.
- **Detector:** 6 file warnings on the built site: 1 real, 5 false positives. Live-URL scans found 4 confirmed line-length or overflow problems. No overlay was injected because the review was read-only, so the browser shows no overlay.

**Overall impression:** a solid, honest docs site. The biggest gains are in wayfinding, which is the "one click to any answer" principle: highlight the current nav item, list every guide on the home page, add next-step links. Mobile overflow is the one real defect.

**What's working**
- **Focus and keyboard:** a 3px amber ring with a 2px offset, clear on the blue button and in dark mode. The skip link is the first Tab stop.
- **Dark mode:** a full second theme. Syntax colours keep their contrast (the lowest dark-mode ratio is 6.65).
- **Code first:** the home page shows a short C# sample with `Validate()` right after the hero, and getting started is numbered steps 1 to 6.

## Findings

Severity: P0 blocks the task, P1 is major, P2 is minor, P3 is polish. Totals: P0 0, P1 3, P2 5, P3 3.

### F1 [P1] Guide pages don't mark "Guides" as the current nav item
- **Location:** `yocto3/docs/_layouts/default.html:24`, `yocto3/docs/assets/css/site.css:65`
- **Category:** Accessibility / Navigation (heuristic 1, WCAG 2.4.8)
- **Problem:** `page.url == item.url` only matches `/guides/` itself. On all 8 `/guides/<slug>/` pages no nav item is highlighted and nothing is exposed to assistive technology, so readers lose their place.
- **Fix:** in `default.html:24`:
  ```liquid
  <a href="{{ item.url | relative_url }}"{% if page.url == item.url %} aria-current="page"{% elsif item.url != '/' and page.url contains item.url %} aria-current="true"{% endif %}>{{ item.title }}</a>
  ```
  In `site.css:65`:
  ```css
  .site-header nav a:hover, .site-header nav a[aria-current] { color: var(--text); }
  ```
- **Command:** `/impeccable polish`

### F2 [P1] `/getting-started/` scrolls sideways on phones
- **Location:** `yocto3/docs/getting-started.md:14`; CSS at `site.css:77`
- **Category:** Responsive (WCAG 1.4.10 Reflow)
- **Problem:** the inline code `Microsoft.Extensions.DependencyInjection.Abstractions` cannot break. At 390px the page is 506px wide, so the whole page pans. The same issue pushes the FAQ code on `/` and `/faq/` 9px past the edge when an item is opened (`index.md:10`, `faq.md:10`).
- **Fix:** add to `site.css` after line 77:
  ```css
  :not(pre) > code { overflow-wrap: anywhere; }
  ```
- **Command:** `/impeccable adapt`

### F3 [P1] The home page "Learn more" grid leaves out 2 of the 8 guides
- **Location:** `yocto3/docs/index.md:75-82`
- **Category:** Information architecture (PRODUCT.md principle 4, "One click to any answer")
- **Problem:** the Modules and WPF guides are missing. PRODUCT.md names WPF and XAML developers as part of the audience, but nothing on the home page mentions WPF. The grid also leaves 2 orphan cards on its second row at desktop width.
- **Fix:** insert after `index.md:79`. Eight cards then fill a clean 4×2 grid at 1440px.
  ```html
  <div class="card"><h3><a href="{{ '/guides/modules/' | relative_url }}">Modules and assembly registration</a></h3><p>Package registrations with the library that owns them.</p></div>
  <div class="card"><h3><a href="{{ '/guides/wpf/' | relative_url }}">WPF and XAML</a></h3><p>Resolve view models in markup with <code>yocto.Wpf</code>.</p></div>
  ```
- **Command:** `/impeccable clarify`

### F4 [P2] Cards that aren't links still react to hover; link cards are only clickable on the title
- **Location:** `yocto3/docs/assets/css/site.css:95`, `index.md:49-54`
- **Category:** Consistency (heuristic 4)
- **Problem:** the six "Why yocto?" cards turn their border blue on hover, which suggests a link that isn't there. On the guide cards, only the title text can be clicked.
- **Fix:** replace `site.css:95` and add whole-card hit areas:
  ```css
  .card:has(a):hover, .card:focus-within { border-color: var(--accent); }
  .card:has(h3 a) { position: relative; }
  .card h3 a::after { content: ""; position: absolute; inset: 0; border-radius: inherit; }
  ```
- **Command:** `/impeccable polish`

### F5 [P2] FAQ answers on the home page run to about 152 characters per line
- **Location:** `yocto3/docs/index.md:3` (`wide: true`), `index.md:84`, `site.css:58`
- **Category:** Readability (detector `line-length`, confirmed)
- **Problem:** the wide home page also widens the FAQ, so an opened answer spans 1112px.
- **Fix:** keep the grid and table wide and cap the prose. Add to `site.css` after line 58:
  ```css
  main.wrap.wide .faq { max-width: var(--prose); }
  ```
  - Optional: the 46rem column measures about 95–103 characters per line on `/faq/` and `/guides/lifetimes/`. Changing `--prose` from `46rem` to `42rem` at `site.css:19` brings that to about 85.
- **Command:** `/impeccable typeset`

### F6 [P2] FAQ questions lose heading meaning, and `/faq/` repeats its title
- **Location:** `yocto3/docs/_includes/faq.html:3,6`, `faq.md:35`, `site.css:103`
- **Category:** Accessibility (semantics)
- **Problem:** `<summary>` is exposed as a button, so the `<h3>` inside it is flattened in most screen readers. On `/faq/`, the h1 "Frequently asked questions" is followed straight away by an h2 "yocto FAQ".
- **Fix, part 1:** put the h3 outside the button so each question stays a heading. In `faq.html:6`:
  ```html
  <summary><span class="q">{{ q.q }}</span></summary>
  ```
  In `site.css:103`:
  ```css
  .faq summary .q { font-weight: 700; font-size: 1.05rem; }
  ```
  The FAQPage JSON-LD comes from front matter, so it is unaffected.
- **Fix, part 2:** in `faq.md:35`, give the include a heading that is not a duplicate, for example `heading="Using yocto"`. Alternatively, make `faq.html:3` conditional with `{% unless include.heading == "" %}` and pass `heading=""`.
- **Command:** `/impeccable harden`

### F7 [P2] `/guides/` jumps from h1 to h3
- **Location:** `yocto3/docs/guides/index.md:6-9`
- **Category:** Accessibility (detector `skipped-heading`, confirmed)
- **Problem:** the card titles are h3 with no h2 above them.
- **Fix:** insert `## All guides` between line 6 and line 8. This also gives the page an obvious entry point for screen-reader heading navigation.
- **Command:** `/impeccable harden`

### F8 [P2] Getting started and the guides end with no next step
- **Location:** `yocto3/docs/getting-started.md:63`, `yocto3/docs/_layouts/default.html:40`
- **Category:** User control / emotional journey (peak-end)
- **Problem:** getting started stops at "6. Dispose". Each guide ends on "Last reviewed…", with no route onward.
- **Fix:**
  - Append to `getting-started.md`:
    ```markdown
    ## Next

    [Lifetimes]({{ '/guides/lifetimes/' | relative_url }}) · [ASP.NET Core]({{ '/guides/aspnet-core/' | relative_url }}) · [All guides]({{ '/guides/' | relative_url }})
    ```
  - In `default.html`, insert before line 40:
    ```liquid
    {% if page.next %}<p class="next">Next: <a href="{{ page.next.url | relative_url }}">{{ page.next.title }} →</a></p>{% endif %}
    ```
  - Add `next: {url: /guides/…/, title: …}` front matter to each guide in reading order.
- **Command:** `/impeccable onboard`

### F9 [P3] The line-count claim needs a qualifier or a new number
- **Location:** `yocto3/docs/index.md:54`, `index.md:69`
- **Category:** Accuracy (PRODUCT.md principle 1)
- **Problem:** `src/Yocto/*.cs` has 1,350 non-blank lines today. "About 1,200 lines of C#" is only true if comment lines are left out (1,231).
- **Fix, either:**
  - `index.md:54`: change to "About 1,200 lines of code (excluding comments)." and leave `index.md:69` as "~1.2K lines".
  - Or use the plain count: "About 1,350 lines of C#." and "~1.35K lines".
- **Command:** `/impeccable clarify`

### F10 [P3] Small mobile tap targets and clipped tables with no scroll cue
- **Location:** `site.css:63,80,112,130-133`, `index.md:59`
- **Category:** Responsive (WCAG 2.5.8)
- **Problem:**
  - At 390px, footer links are 23px tall, and breadcrumb and footer-paragraph links are 17px.
  - The comparison table hides the Autofac column with no hint that it scrolls.
  - The 8-line lead and the wrapped nav push the C# sample below the fold, which goes against "Code first".
- **Fix:** add inside the existing `@media (max-width: 640px)` block:
  ```css
  .footer-links a, .breadcrumb a { display: inline-block; padding-block: .35rem; }
  .lead { font-size: 1.05rem; }
  .site-header nav { gap: .25rem 1rem; }
  ```
  Also in `index.md:59`, shorten the column header to `MEDI 10 (built-in)`. The full package name already appears in context.
- **Command:** `/impeccable adapt`

### F11 [P3] The 122KB PNG logo is used where a 641-byte SVG exists
- **Location:** `yocto3/docs/_layouts/default.html:19`, `index.md:28`
- **Category:** Performance
- **Problem:** `assets/yocto.png` is 512×512 and 122KB, displayed at 32px and 112px. It accounts for about 84% of every page's weight.
- **Fix:** the logo itself stays (brand commitment). Point both `<img>` tags at the same mark in SVG: `src="{{ '/assets/yocto.svg' | relative_url }}"`. Keep the PNG for OG and `llms.txt` use.
- **Command:** `/impeccable optimize`

## Patterns and systemic issues
- **Wayfinding is the weak system:** no current-page state on nested routes, an incomplete guide list on the home page, and no next-step links. F1, F3 and F8 together are the "one click to any answer" gap.
- **The `wide: true` home page widens everything,** prose included (F5). Wide should apply only to grids and tables.
- **Defined components are not used:** `.callout` (`site.css:107-108`) and `.family` (`site.css:116-120`) have no markup. Use `.callout` for the AOT caveats in `guides/validation-and-aot.md` and the "only dependency" line in getting started, rather than inventing new patterns.

## Persona red flags
- **Jordan (first-timer):**
  - "IoC" in the h1 is not spelled out on the page.
  - Getting started ends without "you're done, now…".
  - "View on GitHub" and "NuGet" carry nearly the same visual weight as "Get started".
- **Sam (keyboard / screen reader):**
  - No `aria-current` on guide pages (F1).
  - FAQ headings disappear inside `<summary>` (F6).
  - The byline links (Richard Crane, MILL5) come before "Get started" in tab order. This is acceptable; keep the byline.
- **Casey (mobile):**
  - The page scrolls sideways on getting started (F2).
  - Tap targets are 17–23px (F10).
  - Tables are clipped with no cue (F10).
- **WPF desktop developer (from PRODUCT.md):**
  - WPF is not mentioned anywhere on the home page.
  - `yocto.Wpf` only shows up once the reader reaches `/guides/`. F3 fixes this.

## Minor observations (not ticketed)
- The ✅ emoji in the comparison table renders differently on each OS and brings in green, against the One Signal Rule. A plain `✓` in `var(--text)` would match DESIGN.md. Owner's call.
- On the lifetimes guide at 390px, the table header "Owned and disposed by" wraps one word per line.
- Buttons have no hover transition, while cards use 0.15s.

## Positive findings
- Contrast passes WCAG AA for every text and background pair in both schemes. The lowest is the light-mode syntax keyword at 4.73.
- No console errors and no images missing alt text. Landmarks are complete, with labelled Primary, Footer and Breadcrumb navs. `lang` is set.
- There is no motion that needs `prefers-reduced-motion`; the only transition is a 150ms border-colour change.
- Brand commitments are all present and well placed: the MVP/MILL5 byline, the 43K+ downloads, the podcast footer link and the logo.

## Recommended actions
1. **[P1] `/impeccable polish`:** F1 nav current state and F4 card affordance.
2. **[P1] `/impeccable adapt`:** F2 inline-code wrapping and F10 mobile tap targets, lead size and table header.
3. **[P1] `/impeccable clarify`:** F3 complete the guide grid and F9 the line-count wording.
4. **[P2] `/impeccable harden`:** F6 FAQ semantics and F7 the `/guides/` h2.
5. **[P2] `/impeccable typeset`:** F5 cap FAQ width on the wide home page.
6. **[P2] `/impeccable onboard`:** F8 next-step links.
7. **[P3] `/impeccable optimize`:** F11 SVG logo.
8. `/impeccable polish`: final pass.

## Questions for Rich
1. **Line count (F9):** qualify as "lines of code (excluding comments)" or change to ~1,350?
2. **Wayfinding scope:** only the small fixes (F1, F3, F8), or also a sticky guide list in the empty right column on desktop guide pages? That would be a larger change.
3. **Comparison table:** keep ✅ as it is, or switch to a plain ✓ in ink colour to follow the One Signal Rule?

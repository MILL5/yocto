---
name: yocto
description: The tiny IoC container for .NET 10 — documentation site (mill5.github.io/yocto)
colors:
  signal-blue: "#2563eb"
  signal-blue-deep: "#1d4ed8"
  signal-blue-wash: "#e8f0ff"
  accent-ink: "#ffffff"
  focus-amber: "#9a6a00"
  paper: "#fbfcfe"
  surface-white: "#ffffff"
  ink: "#111827"
  slate-muted: "#5b6475"
  hairline: "#e3e8f0"
  code-wash: "#f2f5fa"
  syntax-keyword: "#2563eb"
  syntax-string: "#0f766e"
  syntax-type: "#a1580b"
  syntax-number: "#b4235a"
  night-paper: "#0b1220"
  night-surface: "#111a2e"
  night-ink: "#e6edf7"
  night-muted: "#9aa8c0"
  night-hairline: "#22304a"
  night-signal-blue: "#60a5fa"
  night-signal-blue-hover: "#93c5fd"
  night-signal-wash: "#172554"
  night-code-wash: "#16213a"
  night-focus-amber: "#f2b51c"
  night-syntax-string: "#6fd3c4"
  night-syntax-type: "#f2c25c"
  night-syntax-number: "#ff8fb4"
typography:
  display:
    fontFamily: "ui-sans-serif, system-ui, -apple-system, \"Segoe UI\", Roboto, sans-serif"
    fontSize: "clamp(2rem, 4vw, 2.75rem)"
    fontWeight: 700
    lineHeight: 1.25
    letterSpacing: "-0.01em"
  headline:
    fontFamily: "ui-sans-serif, system-ui, -apple-system, \"Segoe UI\", Roboto, sans-serif"
    fontSize: "1.5rem"
    fontWeight: 700
    lineHeight: 1.25
  title:
    fontFamily: "ui-sans-serif, system-ui, -apple-system, \"Segoe UI\", Roboto, sans-serif"
    fontSize: "1.05rem"
    fontWeight: 700
    lineHeight: 1.25
  lead:
    fontFamily: "ui-sans-serif, system-ui, -apple-system, \"Segoe UI\", Roboto, sans-serif"
    fontSize: "1.2rem"
    fontWeight: 400
    lineHeight: 1.6
  body:
    fontFamily: "ui-sans-serif, system-ui, -apple-system, \"Segoe UI\", Roboto, sans-serif"
    fontSize: "1rem"
    fontWeight: 400
    lineHeight: 1.6
  label:
    fontFamily: "ui-sans-serif, system-ui, -apple-system, \"Segoe UI\", Roboto, sans-serif"
    fontSize: "0.875rem"
    fontWeight: 400
    lineHeight: 1.6
  code:
    fontFamily: "ui-monospace, \"SF Mono\", \"Cascadia Code\", Menlo, Consolas, monospace"
    fontSize: "0.9em"
    lineHeight: 1.45
rounded:
  inline: "4px"
  base: "10px"
spacing:
  xs: "0.5rem"
  sm: "0.75rem"
  md: "1rem"
  gutter: "1.25rem"
  lg: "2rem"
  xl: "4rem"
  prose: "46rem"
  max: "72rem"
components:
  button-primary:
    backgroundColor: "{colors.signal-blue}"
    textColor: "{colors.accent-ink}"
    rounded: "{rounded.base}"
    padding: "0.65rem 1.1rem"
  button-primary-hover:
    backgroundColor: "{colors.signal-blue-deep}"
  button-secondary:
    backgroundColor: "transparent"
    textColor: "{colors.ink}"
    rounded: "{rounded.base}"
    padding: "0.65rem 1.1rem"
  button-secondary-hover:
    backgroundColor: "{colors.signal-blue-wash}"
  card:
    backgroundColor: "{colors.surface-white}"
    rounded: "{rounded.base}"
    padding: "1.1rem 1.2rem"
  code-block:
    backgroundColor: "{colors.code-wash}"
    textColor: "{colors.ink}"
    rounded: "{rounded.base}"
    padding: "1rem"
  code-inline:
    backgroundColor: "{colors.code-wash}"
    rounded: "{rounded.inline}"
    padding: "0.1em 0.35em"
  site-header:
    backgroundColor: "{colors.surface-white}"
    height: "4rem"
  callout:
    backgroundColor: "{colors.surface-white}"
    rounded: "{rounded.base}"
    padding: "0.75rem 1rem"
---

# Design System: yocto

<!-- Recorded from the shipped site (yocto3/docs: _layouts/default.html, assets/css/site.css, _includes, Markdown pages, assets/yocto.svg) on 2026-11-02. No owner interview was held: the North Star name, color names, and philosophy lines below are inferences from code and PRODUCT.md, marked "(inferred)". Token values are extracted verbatim and are normative. -->

## Overview

**Creative North Star: "The Readable Afternoon"** (inferred; from PRODUCT.md's "small enough to read in an afternoon")

The site is a plain, quiet technical reading surface that mirrors the library it documents: one stylesheet of about 130 lines, one layout, system fonts, no JavaScript, no imagery beyond the logo. Everything sits on a near-white paper (or deep navy at night) with a single blue accent that marks what is actionable: links, the primary button, card hover, the FAQ toggle glyph. Hierarchy comes from size, weight, and the muted/ink text contrast, not from color blocks or decoration.

Density is relaxed and prose-led. Content runs in a 46rem reading column inside a 72rem frame; only the home page breaks out to full frame width for its card grid and comparison table. Code blocks are first-class citizens, given a tinted wash, a hairline border, and the same 10px corner as cards and buttons, so C# reads as part of the page rather than a pasted inset.

Both color schemes are equal: every token has a light and a dark value switched by `prefers-color-scheme`, with no manual toggle. (inferred) The visual stance is "a well-made README": competent, calm, and out of the way of the code.

**Key Characteristics:**
- System UI sans for everything except code; system monospace for code.
- One accent hue (blue), used for interaction only; amber reserved for keyboard focus.
- Flat: hairline borders and tonal surfaces, zero shadows.
- One corner radius (10px) for every container-like object; 4px for inline code.
- Light and dark schemes driven entirely by CSS custom properties.

## Colors

A cool, near-monochrome slate palette with one saturated blue for interaction and an amber focus ring that deliberately breaks the hue family.

### Primary
- **Signal Blue** (`signal-blue`; dark `night-signal-blue`): links, primary button fill, card hover border, FAQ +/– glyph, callout label, caret, native `accent-color`. The only chromatic UI color on the page.
- **Signal Blue Deep** (`signal-blue-deep`; dark `night-signal-blue-hover`): primary button hover. In dark mode the hover goes lighter, not darker.
- **Signal Wash** (`signal-blue-wash`; dark `night-signal-wash`): secondary button hover fill and text selection background.
- **Accent Ink** (`accent-ink`; dark `night-paper`): text on Signal Blue fills.

### Tertiary
- **Focus Amber** (`focus-amber`; dark `night-focus-amber`): the 3px keyboard focus outline on links and FAQ summaries. Chosen off-hue so focus never reads as just another link.

### Neutral
- **Paper** (`paper`; dark `night-paper`): page background.
- **Surface White** (`surface-white`; dark `night-surface`): header bar, cards, table header cells, callouts, skip link.
- **Ink** (`ink`; dark `night-ink`): body text, headings, brand wordmark, footer link text.
- **Slate Muted** (`slate-muted`; dark `night-muted`): lead paragraph, nav links at rest, card body, breadcrumb, meta line, footer, code comments.
- **Hairline** (`hairline`; dark `night-hairline`): every border and divider, plus scrollbar thumb.
- **Code Wash** (`code-wash`; dark `night-code-wash`): code block and inline code background.

### Syntax
Rouge highlighting uses four hues, each with a dark-mode counterpart: keyword (`syntax-keyword`, same as Signal Blue), string (`syntax-string`, teal), type/function name (`syntax-type`, amber-brown), number (`syntax-number`, raspberry). Comments use Slate Muted in italic.

### Named Rules
**The One Signal Rule.** Blue means "you can act on this." It never fills a section background, decorates a heading, or appears as an illustration color.

**The Amber Focus Rule.** Keyboard focus is always the 3px amber outline at 2px offset, never blue, so it stays distinguishable from links.

**The Paired Scheme Rule.** No color is introduced without both a light and a dark value in the `:root` custom-property blocks.

## Typography

**Display Font:** system UI sans (`ui-sans-serif, system-ui, -apple-system, "Segoe UI", Roboto, sans-serif`)
**Body Font:** same stack
**Label/Mono Font:** system monospace (`ui-monospace, "SF Mono", "Cascadia Code", Menlo, Consolas, monospace`)

**Character:** Native, zero-download type that looks like the reader's own OS and IDE. (inferred) It signals "tool documentation," not marketing.

### Hierarchy
- **Display** (h1, fluid clamp): one per page; page titles and the home hero headline. Slight negative tracking.
- **Headline** (h2, browser default ~1.5rem): section breaks, with generous top margin (2.5rem + 0.5rem padding).
- **Title** (h3 / card and FAQ headings, 1.05rem bold): card titles, FAQ questions, family items; plain h3 in prose uses browser default size with 1.75rem top margin.
- **Lead** (1.2rem, Slate Muted): the single intro paragraph under the home h1, capped at prose width.
- **Body** (1rem / 1.6): prose, capped at 46rem (roughly 75–80 characters).
- **Label** (0.875rem, Slate Muted): breadcrumb and "Last reviewed" meta line. Footer is 0.9rem; tables and card body 0.95rem.
- **Callout tag** (0.75rem, 700, 0.06em tracking, uppercase, Signal Blue): the only uppercase text in the system.
- **Code** (0.9em monospace, 1.45 line height in blocks).

### Named Rules
**The No Webfont Rule.** Type is the system stack only; no font files are loaded.

**The Single Uppercase Rule.** Uppercase tracking is reserved for the callout "Note" tag.

## Layout

A centered frame (`.wrap`, max 72rem, 1.25rem side gutters) holds header, main, and footer. Inside `main`, the article is capped at the 46rem prose width; pages with `wide: true` (the home page) release the article to the full frame. `main` has 2rem top and 4rem bottom padding.

Grids are intrinsic, not breakpoint-driven: cards use `repeat(auto-fill, minmax(15rem, 1fr))` with 1rem gaps; the family grid uses `minmax(17rem, 1fr)` with 2rem column gaps. The header and button rows are wrapping flex rows (nav gap 1.25rem, actions gap 0.75rem). Tables are `display: block` with horizontal scroll so wide comparison tables never break the column.

There is one breakpoint, 640px: the hero's two-column grid (text + 112px logo) collapses to one column and the logo is hidden.

Spacing rhythm is rem-based and loose rather than a strict scale: 0.5 / 0.75 / 1 / 1.25 / 1.5 / 2 / 2.5 / 3 / 4rem recur.

## Elevation & Depth

The system is completely flat. There is no `box-shadow` anywhere. Depth is conveyed by tonal layering (Surface White over Paper; Code Wash for code) and 1px Hairline borders. The header separates from content with a bottom hairline, the footer with a top hairline.

### Named Rules
**The Hairline Rule.** Separation is a 1px Hairline border or a surface tone change, never a shadow.

**The Border-Is-State Rule.** The only hover "lift" is a border-color change to Signal Blue (cards, secondary buttons), transitioned at 0.15s ease-out on cards.

## Shapes

One soft radius (10px, `rounded.base`) is used for every block object: buttons, cards, code blocks, callouts. Inline code uses a tighter 4px (`rounded.inline`). Tables, the header, and the footer are square, ruled by hairlines rather than boxed. The logo echoes the language: a rounded-square gradient tile (blue to cyan) with two nested rounded-square outlines and a center dot.

## Components

### Buttons
Compact, solid, and few. (inferred character)
- **Shape:** gently rounded (10px).
- **Primary:** Signal Blue fill, Accent Ink text, 600 weight, 0.65rem × 1.1rem padding, no border. One per action row ("Get started").
- **Hover:** fill shifts to Signal Blue Deep (light) or the lighter hover blue (dark). No transition is declared.
- **Secondary:** transparent with Ink text and a 1px Hairline border; hover adds Signal Wash fill and a Signal Blue border. Used for outbound links (GitHub, NuGet).
- **Focus:** buttons are links and inherit the amber focus outline.

### Cards / Containers
- **Corner Style:** 10px.
- **Background:** Surface White on Paper.
- **Shadow Strategy:** none (see Elevation & Depth).
- **Border:** 1px Hairline; turns Signal Blue on hover or `:focus-within`, 0.15s ease-out.
- **Internal Padding:** 1.1rem × 1.2rem. Title 1.05rem bold, body 0.95rem Slate Muted, links undecorated.
- Used in auto-fill grids on the home page ("Why yocto?") and the guides index.

### Code Blocks
- Code Wash background, 1px Hairline border, 10px radius, 1rem padding, horizontal scroll, Rouge highlighting via the syntax tokens. Inline code: Code Wash pill, 4px radius, no border.

### Navigation
- **Header:** Surface White bar, bottom hairline, min-height 4rem. Brand left (32px logo + bold 1.15rem lowercase "yocto" in Ink); nav right, wrapping.
- **Nav links:** Slate Muted, 500 weight, no underline; hover and `aria-current="page"` turn Ink. No underline or pill for the active state.
- **Breadcrumb:** 0.875rem Slate Muted text line with "›" separators on guide pages.
- **Footer:** top hairline, 0.9rem Slate Muted; a wrapping row of Ink 500-weight links (hover Signal Blue), then copyright and MILL5/MVP attribution lines.
- **Skip link:** off-screen until focused, then a Surface White chip at top-left.

### FAQ Disclosure (signature)
Native `<details>` rows separated by bottom hairlines, 0.75rem vertical padding. The summary is an inline 1.05rem h3 preceded by a bold Signal Blue "+" that becomes "–" when open; the native marker is hidden. The first item renders open.

### Tables
Full-width, collapsed, 0.95rem, 0.6rem × 0.75rem cells ruled by bottom hairlines only; header cells on Surface White. Used for the yocto / MEDI / Autofac comparison.

### Callout and Family Grid (defined, currently unused)
The stylesheet defines a `callout` (Surface White box, hairline border, 10px radius, uppercase blue "Note" tag) and a `family` grid (hairline-topped items in a 17rem auto-fill grid). No current page uses either; reuse them rather than inventing new note or related-project patterns.

## Do's and Don'ts

### Do:
- **Do** take every color from the `:root` custom properties and add both light and dark values for anything new.
- **Do** use Signal Blue only for links and interactive affordances; keep focus as the 3px amber outline at 2px offset.
- **Do** keep reading content inside the 46rem prose column; use `wide: true` only for grid- or table-heavy pages.
- **Do** use the 10px radius and a 1px Hairline border for any new block container.
- **Do** keep the system font stacks; code in the system monospace stack.
- **Do** keep the yocto logo (`assets/yocto.svg` / `yocto.png`) and the lowercase "yocto" wordmark as shipped.

### Don't:
- **Don't** add box-shadows, gradients (outside the logo), or background imagery; depth is hairlines and surface tone.
- **Don't** load webfonts or icon fonts.
- **Don't** introduce a second accent hue for UI; syntax colors stay inside code blocks.
- **Don't** add client-side JavaScript for presentation (theme toggles, animated reveals); the dark scheme follows the OS.
- **Don't** use uppercase tracking outside the callout tag.

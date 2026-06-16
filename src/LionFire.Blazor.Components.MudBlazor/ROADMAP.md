# ROADMAP — Eliminating `Radzen.Blazor` and `Orleans.Runtime`

> Scope: decouple `LionFire.Blazor.Components.MudBlazor` (the "heavy" MudBlazor
> component library) from the `Radzen.Blazor` package and from the
> `Orleans.Runtime` transitive closure, so the library can multi-target
> `net9.0;net10.0` and be consumed cleanly by net10 apps (e.g. axi-web).
>
> Status of this document: **research + analysis** produced from static source
> inspection. The full `/src/Core` graph does **not** compile on the machine
> where this was authored (see [§7 Open Questions](#7-open-questions--unknowns)),
> so every claim below is grounded in `grep`/file inspection rather than a clean
> build. Verify before executing the larger steps.

---

## 1. Goal & motivation

`LionFire.Blazor.Components.MudBlazor` is the project's main MudBlazor component
library (PropertyGrid / Inspector, async collection views, editors, notifications,
view-type providers). It is **net9.0-only** and drags in a heavy transitive graph.
Two dependencies are the worst offenders for downstream consumers:

- **`Radzen.Blazor`** — a whole second Blazor component framework (pinned at
  `6.1.6` in `/src/Core/Directory.Packages.props`). The library is otherwise
  **MudBlazor-first**; Radzen is a tiny legacy island. Carrying an entire rival
  UI kit for one component is pure weight (CSS/JS assets, package surface,
  version-churn exposure).
- **`Orleans.Runtime`** — pulled transitively through the
  `LionFire.Orleans.Structures.Abstractions` project reference (which references
  `LionFire.DynamicData.Orleans` → `Microsoft.Orleans.Sdk`). A distributed-actor
  runtime has **no business** in a Blazor presentation library's closure, yet it
  pins the net version, bloats restore, and couples the UI tier to the clustering
  stack.

Why this hurts specifically:

- **net version pinning.** Both the library and the Orleans projects target
  `net9.0`. A net10 consumer (axi-web) cannot take the library cleanly while it
  and its closure are net9-only.
- **transitive bloat for web consumers.** Web hosts that only want a PropertyGrid
  or a collection view inherit ReactiveUI + DynamicData + Radzen + Orleans +
  Roslyn (`Microsoft.CodeAnalysis.*`) and a 10-project LionFire graph.
- **fragility.** Every one of those is an independent breaking-change / CVE /
  net-bump surface the UI library is hostage to.

**Precedent already set (first step taken):** a lean tier was extracted into the
sibling project **`LionFire.Blazor.Components.MudBlazor.Lean`**
(`../LionFire.Blazor.Components.MudBlazor.Lean/`). It multi-targets
`net9.0;net10.0`, depends on **only** `MudBlazor` + `Microsoft.AspNetCore.Components.Web`,
and carries the components whose bodies need nothing heavier — `InlineSelector`
(+ its size enum/CSS), and (per the heavy csproj's comment) `AutoRefreshButton`
and `EnumerableViewComponent`. The heavy project `ProjectReference`s the lean one
and keeps the shared root namespace `LionFire.Blazor.Components.MudBlazor_` (note
trailing underscore) so existing references resolve unchanged. **That extraction
is the proven decoupling model this roadmap extends inward** toward the
reactive/MVVM tier.

---

## 2. Current dependency map

### 2.1 Direct dependencies of the heavy library

From `LionFire.Blazor.Components.MudBlazor.csproj` (`net9.0`,
`RootNamespace=LionFire.Blazor.Components.MudBlazor_`):

| Kind | Reference | Notes |
|---|---|---|
| Package | `Microsoft.AspNetCore.Components.Web` | essential |
| Package | `MudBlazor` | essential — the library's reason to exist |
| Package | **`Radzen.Blazor`** | **target for removal** (see §3) |
| Package | `ReactiveUI.Blazor` | essential to the reactive/MVVM tier |
| Package | `ReactiveUI.SourceGenerators` | analyzer (`PrivateAssets=all`) — `[Reactive]` |
| Package | `System.ComponentModel`, `System.ComponentModel.Annotations` | light |
| Package | `ObservableCollections` | collection views |
| Package | `System.Reactive.Async` | async reactive |
| Package | `Microsoft.CodeAnalysis.Common` | **see §3.3 — usings vestigial; pkg likely droppable** |
| Package | `Microsoft.CodeAnalysis.Workspaces.Common` | **likely droppable with the above** |
| ProjectRef | `...MudBlazor.Lean` | the lean tier (kept; heavy depends on lean) |
| ProjectRef | `LionFire.Base` | |
| ProjectRef | `LionFire.Blazor.Components` | heavily used (~29 files) |
| ProjectRef | `LionFire.Data.Async.Abstractions` | `LionFire.Data.Async.*` ns used |
| ProjectRef | `LionFire.Data.Async.Mvvm` | |
| ProjectRef | `LionFire.Data.Async.UI` | |
| ProjectRef | `LionFire.Inspection` | used (~14 files) — PropertyGrid/Inspector |
| ProjectRef | `LionFire.Mvvm` | used (~7 files) |
| ProjectRef | **`LionFire.Orleans.Structures.Abstractions`** | **target for removal** (see §4) |
| ProjectRef | `LionFire.UI.Abstractions` | |
| ProjectRef | `LionFire.UI.Entities` | |

`_Imports.razor` adds `@using Radzen.Blazor` (and Mud/ReactiveUI/LionFire usings).
`GlobalUsings.cs` pulls `ReactiveUI`, `DynamicData`, `MudBlazor`, etc. (no Radzen,
no Orleans there).

### 2.2 How each heavy dependency enters the closure

| Heavy dep | Entry point | Genuinely consumed? |
|---|---|---|
| `Radzen.Blazor` | direct `PackageReference` + `@using Radzen.Blazor` in `_Imports.razor` | **One** live tag: `<RadzenProfileMenu>` in `Notifications/NotificationsDropdown.razor`. Everything else is commented-out (`@* ... *@`). |
| `Orleans.Runtime` | `LionFire.Orleans.Structures.Abstractions` → `LionFire.DynamicData.Orleans` → `Microsoft.Orleans.Sdk` (and `Microsoft.Orleans.Core.Abstractions`) | **No.** Only two vestigial `using Orleans.Runtime;` lines; **zero** Orleans types used; **zero** types from `LionFire.Orleans.Structures.Abstractions` referenced. |
| `Microsoft.CodeAnalysis.*` (Roslyn) | direct `PackageReference` ×2 | **No.** 5 vestigial usings (`...VisualBasic.Syntax` ×4, `...CSharp.Syntax` ×1); **zero** Roslyn types (`SyntaxNode`, `Compilation`, `Workspace`, `ISymbol`, …) used. |

**Legend of usage classification used below:** *vestigial* = a `using`/`@using`/
tag with no corresponding type usage in the file body (compile-blocking only
because the namespace must resolve); *essential* = a type from the dependency is
actually consumed.

---

## 3. Radzen elimination

### 3.1 Findings (verified by grep over `.razor` + `.cs`)

- The **only non-comment Radzen reference** in the entire library is the
  `<RadzenProfileMenu>` element wrapping the notification dropdown in
  `Notifications/NotificationsDropdown.razor` (open tag line 5, close tag line 71).
  Its `<ChildContent>` is **entirely MudBlazor** (`MudTabs`, `MudTabPanel`,
  `MudList`, `MudLink`, `MudIcon`). The Radzen element is a thin outer shell.
- Every other Radzen token in the codebase is **inside a Razor comment**
  (`@*<RadzenCard …>*@`, `@*<RadzenProfileMenuItem …>*@` in `NotificationsDropdown.razor`
  and `NotificationsDropdownItem.razor`) — i.e. dead text, not compiled.
- `_Imports.razor` line 4 carries `@using Radzen.Blazor` (the only `@using`).
- **`NotificationsDropdown` has zero consumers** anywhere under `/src/Core/src`
  (grep for the component name outside its own folder returns nothing). It is, in
  practice, an orphan component.

**Net assessment: Radzen is a removable legacy island, not load-bearing.** There
is no `RadzenDataGrid`, no Radzen theming dependency, no Radzen service
registration in the library's hosting extension — just one wrapper tag in one
unreferenced component.

### 3.2 Per-usage classification

| Usage | File | Class | Action |
|---|---|---|---|
| `@using Radzen.Blazor` | `_Imports.razor:4` | vestigial-ish (only needed by the one tag) | **delete** |
| `<RadzenProfileMenu>…</RadzenProfileMenu>` | `Notifications/NotificationsDropdown.razor:5,71` | (a) trivially replaceable | **replace** with `MudMenu`/`MudPopover` wrapper, or just drop the wrapper (the children are self-contained MudBlazor) |
| commented `Radzen*` tags | `NotificationsDropdown.razor`, `NotificationsDropdownItem.razor` | dead | optional cleanup (delete comments) |

The `RadzenProfileMenu` is a click-to-open popover/menu shell. MudBlazor's
`MudMenu` (already used elsewhere in this very library — e.g.
`Collections/KeyedVMCollectionView.razor`, `Collections/DynamicData_/ObservableDataView.razor`)
or `MudPopover` is a direct structural equivalent. Because the component is
orphaned, the lowest-risk move is to **drop the Radzen wrapper entirely** (leave
the MudBlazor children as the component body) rather than perfectly reproduce
Radzen's open/close affordance — nothing consumes it today, so behavior parity is
not a constraint.

### 3.3 Ordered plan

1. **Replace / drop the one `<RadzenProfileMenu>` wrapper** in
   `NotificationsDropdown.razor`. Either (a) wrap the existing MudBlazor children
   in a `MudMenu`/`MudPopover` if a click-to-open affordance is wanted, or (b)
   remove the outer wrapper and keep the children. **Effort: S. Risk: Low**
   (component is orphaned; no consumer to regress).
2. **Delete `@using Radzen.Blazor`** from `_Imports.razor`. **Effort: trivial.
   Risk: Low.**
3. *(optional)* Delete the commented-out `Radzen*` lines for hygiene. **Effort:
   trivial. Risk: none.**
4. **Drop `<PackageReference Include="Radzen.Blazor" />`** from the csproj.
   **Effort: trivial. Risk: Low** once steps 1–2 land. (The package version pin
   in `Directory.Packages.props` can stay or be removed depending on whether any
   *other* project in `/src/Core` uses Radzen — grep before touching the central
   pin; out of scope for this library's csproj.)

**Conclusion: Radzen can be removed outright — no satellite/quarantine package is
needed.** Option (c) "isolate into a Radzen satellite" from the brief is *not*
warranted; there is no genuine Radzen surface to preserve.

---

## 4. Orleans elimination

### 4.1 Findings (verified by grep over `.razor` + `.cs`)

- The **only** Orleans references in the entire library are **two vestigial
  `using Orleans.Runtime;` lines**:
  - `Collections/KeyedCollectionView.razor.cs:17`
  - `Collections/DynamicData_/ObservableDataView.razor.cs:20`
- There is **no usage of any Orleans type** anywhere in the library — grep for
  `IGrain`, `GrainId`, `GrainReference`, `IAddressable`, `ObserverManager`,
  `IClusterClient`, `Orleans.` returns *only* those two `using` lines and nothing
  in any file body.
- There is **no usage of any type from `LionFire.Orleans.Structures.Abstractions`**
  either — grep for that project's public surface (`IHasGrainId`, `IProvidesGrain`,
  `ICollectionG`, `IKeyedListG`, `IListG`, `IGrainListG`, `IKeyedG`,
  `IChangeSetObservable…`, `GrainObserver…`, `LionFire.Orleans`) returns **zero
  hits**. The library references the project but consumes nothing from it.

So both the **`using Orleans.Runtime;`** lines *and* the
**`LionFire.Orleans.Structures.Abstractions` `ProjectReference`** are vestigial.

### 4.2 Why `Orleans.Runtime` is in the closure

`LionFire.Orleans.Structures.Abstractions.csproj` (net9.0) references:
- package `Microsoft.Orleans.Core.Abstractions`, and
- project `LionFire.DynamicData.Orleans`, which in turn references package
  **`Microsoft.Orleans.Sdk`** (a meta-package that pulls the full Orleans runtime
  surface, including `Orleans.Runtime`).

That chain — `MudBlazor library → Orleans.Structures.Abstractions →
DynamicData.Orleans → Microsoft.Orleans.Sdk` — is what drags `Orleans.Runtime`
(and the net9 pin) into every consumer of the library. Since the library uses
**none** of it, the chain can be cut at its first link.

### 4.3 Per-usage classification

| Usage | File | Class | Action |
|---|---|---|---|
| `using Orleans.Runtime;` | `KeyedCollectionView.razor.cs:17` | vestigial | **delete** |
| `using Orleans.Runtime;` | `ObservableDataView.razor.cs:20` | vestigial | **delete** |
| `ProjectReference …Orleans.Structures.Abstractions` | csproj | unused (no type consumed) | **sever** |

### 4.4 Ordered plan

1. **Delete the two vestigial `using Orleans.Runtime;` lines.** Once gone, nothing
   in the library's *own source* names Orleans. **Effort: trivial. Risk: Low** —
   but note they will not currently compile-fail anyway because the package is
   present; removing them is prerequisite housekeeping so step 3 doesn't break.
2. **Confirm the negative** with a compile after removing the ProjectReference (see
   build caveat in §7) — i.e. verify no *transitive* type (e.g. something surfaced
   by `LionFire.Data.Async.*` that happens to live in the Orleans abstractions) is
   silently relied on. Static grep says no, but the closure should be re-checked
   on a machine that builds.
3. **Sever the `LionFire.Orleans.Structures.Abstractions` `ProjectReference`** from
   `LionFire.Blazor.Components.MudBlazor.csproj`. This removes the
   `→ DynamicData.Orleans → Microsoft.Orleans.Sdk` chain and with it `Orleans.Runtime`
   from the closure. **Effort: S. Risk: Low–Medium** (low per static analysis; the
   Medium hedge is only because the graph can't be fully compiled here to prove the
   transitive negative).

**No Orleans satellite package is required.** Because the coupling is purely
vestigial/transitive — not a single component genuinely needs Orleans-grain
abstractions — there is nothing to isolate. The clean Orleans-free core is the
library *minus* one ProjectReference and two `using` lines. (Were a future
component to genuinely need grain abstractions, the satellite pattern from §5
would apply; today it does not.)

### 4.5 Bonus: Roslyn (`Microsoft.CodeAnalysis.*`) is in the same vestigial bucket

While tracing Orleans, the same anti-pattern surfaced for Roslyn:

- 5 vestigial usings — `using Microsoft.CodeAnalysis.VisualBasic.Syntax;` in
  `KeyedCollectionView.razor.cs:16`, `KeyedVMCollectionView.razor.cs:16`,
  `ObservableDataView.razor.cs:18`, `AsyncVMSourceCacheView.razor.cs:17`; and
  `using Microsoft.CodeAnalysis.CSharp.Syntax;` in `PropertyGrid/IInspectorCellVM.cs:4`.
- **Zero** Roslyn types used anywhere (no `SyntaxNode`, `SemanticModel`,
  `Compilation`, `Workspace`, `SyntaxFactory`, …).

These were almost certainly auto-inserted by an IDE "add using" action and never
cleaned up. After deleting the 5 usings, the two
`Microsoft.CodeAnalysis.Common` / `Microsoft.CodeAnalysis.Workspaces.Common`
package references should be droppable — verify with a build. This is a free
third heavy-dependency win bundled with the Orleans cleanup. **Effort: S. Risk:
Low.**

---

## 5. Target architecture

A **tiered package layout**, decoupling-by-extraction (the lean-tier model already
proven), with heavy/optional concerns pushed to the leaves:

```
LionFire.Blazor.Components.MudBlazor.Lean   [EXISTS, net9.0;net10.0]
    └─ deps: MudBlazor, AspNetCore.Components.Web only
    └─ InlineSelector, AutoRefreshButton, EnumerableViewComponent

LionFire.Blazor.Components.MudBlazor         [GOAL: net9.0;net10.0, Radzen-/Orleans-/Roslyn-free]
    └─ the reactive/MVVM tier: PropertyGrid/Inspector, async collection views,
       editors, view-type providers
    └─ deps: MudBlazor, ReactiveUI(.Blazor), DynamicData, ObservableCollections,
       System.Reactive(.Async), + LionFire.{Blazor.Components, Inspection, Mvvm,
       Data.Async.*, UI.*} projects  ──  NO Radzen, NO Orleans, NO CodeAnalysis
    └─ ProjectReferences the .Lean tier (unchanged)

(only if a future need arises — NOT needed today):
LionFire.Blazor.Components.MudBlazor.Radzen   [NOT NEEDED — Radzen has no real surface]
LionFire.Blazor.Components.MudBlazor.Orleans  [NOT NEEDED — no Orleans-coupled component]
```

**End state:**
- Lean tier: **already** `net9.0;net10.0`.
- Heavy tier: **multi-target `net9.0;net10.0`** once it no longer transitively
  pulls net9-only Orleans projects. The blocker to multi-targeting is the
  `LionFire.Orleans.Structures.Abstractions` (net9.0) reference and any other
  net9-only project in the remaining graph (e.g. the `LionFire.Data.Async.*` and
  `LionFire.UI.*` refs must also be confirmed net10-capable — see §7).
- No Radzen / Orleans / Roslyn satellite packages are required. The "satellite"
  column exists in the diagram only to document that the pattern is available if a
  genuinely-coupled component is added later; **no extraction work is in scope.**

The architecture's spirit: **the lib's public components depend only on MudBlazor
+ the reactive/MVVM stack; anything that would re-introduce a rival UI kit or a
distributed-runtime closure stays out, and if ever needed, goes in a leaf
package the core does not reference.**

---

## 6. Phased sequence & quick wins

Ordered so the **zero-risk vestigial deletions land first** (they shrink the
problem and de-risk the later ProjectReference surgery), with the bigger structural
moves after. Estimates: **S** ≈ minutes / one file, **M** ≈ a focused session /
needs a build to verify, **L** ≈ cross-project, needs graph-wide verification.

### Phase 0 — Quick wins (zero/low risk, do first)

- [ ] **S** Delete `using Orleans.Runtime;` ×2 (`KeyedCollectionView.razor.cs:17`,
      `ObservableDataView.razor.cs:20`). *Zero behavioral risk — unused.*
- [ ] **S** Delete the 5 vestigial `Microsoft.CodeAnalysis.*.Syntax` usings
      (`KeyedCollectionView.razor.cs:16`, `KeyedVMCollectionView.razor.cs:16`,
      `ObservableDataView.razor.cs:18`, `AsyncVMSourceCacheView.razor.cs:17`,
      `PropertyGrid/IInspectorCellVM.cs:4`). *Zero risk — unused.*
- [ ] **S** Delete `@using Radzen.Blazor` from `_Imports.razor` *(after Phase 1
      step 1 removes the last tag; or together with it)*.
- [ ] **S** *(optional)* Strip the commented-out `Radzen*` lines for hygiene.

### Phase 1 — Radzen removal

- [ ] **S** Replace/drop the single `<RadzenProfileMenu>` wrapper in
      `Notifications/NotificationsDropdown.razor` (MudMenu/MudPopover, or just keep
      the MudBlazor children). *Low risk — component is orphaned.*
- [ ] **S** Drop `<PackageReference Include="Radzen.Blazor" />` from the csproj.
- [ ] **S** Build the lib (where buildable) to confirm green. *(Central pin in
      `Directory.Packages.props` left alone unless no other project uses Radzen —
      separate grep, out of this csproj's scope.)*

### Phase 2 — Orleans removal

- [ ] **S** Sever `ProjectReference …LionFire.Orleans.Structures.Abstractions` from
      the csproj.
- [ ] **M** Build to verify no transitive Orleans/abstractions type was silently
      relied on (static analysis says none; needs a build to prove). *Risk lives
      here, not in the source.*

### Phase 3 — Roslyn removal (bundled with Orleans cleanup)

- [ ] **S** Drop `Microsoft.CodeAnalysis.Common` and
      `Microsoft.CodeAnalysis.Workspaces.Common` package references.
- [ ] **M** Build to confirm. *Risk: Low.*

### Phase 4 — Multi-target to net10

- [ ] **L** Flip `<TargetFramework>net9.0</TargetFramework>` →
      `<TargetFrameworks>net9.0;net10.0</TargetFrameworks>` and resolve any
      remaining net9-only transitive project refs (`LionFire.Data.Async.*`,
      `LionFire.UI.*`, `LionFire.Inspection`, `LionFire.Mvvm`,
      `LionFire.Blazor.Components`). Each must also become net10-capable, so this
      can cascade — scope it as a follow-on graph-wide pass, mirroring the lean
      tier's `net9.0;net10.0` target. *Risk: Medium–High (cascade), but the
      Radzen/Orleans/Roslyn removals in Phases 1–3 are independent of and
      prerequisite to this.*

### Phase 5 — (deferred) further tier slimming

- [ ] **L** *(optional, not required by the goal)* Evaluate moving more
      MudBlazor-only components down into the `.Lean` tier, and confirm which of
      the 10 ProjectReferences are still genuinely needed after Phases 1–4. *Note:
      a naive "project-name appears in source" grep undercounts usage because
      namespaces don't map 1:1 to project names (e.g. `LionFire.Data.Async.*`
      namespaces are used heavily); do this with a real dependency analyzer, not
      grep.*

**Headline:** Phases 0–3 are almost entirely **trivial, zero-/low-risk edits**
(delete 7 usings, replace 1 wrapper tag, drop 1 ProjectReference + 3 package
references) that eliminate **Radzen, Orleans, *and* Roslyn** from the closure.
Phase 4 (multi-target) is the only structurally larger effort and is independent
of the dependency removals.

---

## 7. Open questions / unknowns

- **Unbuildable graph caveat (primary limitation).** The full `/src/Core` graph
  does **not** compile on the authoring machine for **two** independent reasons:
  1. A referenced sibling repo **`/src/StateMachines` is not checked out** →
     pre-existing `MSB1009` (confirmed: the path does not exist).
  2. `/src/Core/global.json` pins SDK **`9.0.107` (`rollForward: latestMinor`)**,
     which selects `9.0.308`, but **no matching 9.0.x runtime is installed** on
     this box (only 10.0.x runtimes present) — so the build host itself cannot
     launch. (`dotnet --version` fails with "must install … Microsoft.NETCore.App
     9.0.11".)
  Consequently **no claim here was confirmed by a clean compile.** All findings
  rest on static source inspection (`grep` for usings *and* actual type usage).
  The vestigial-using conclusions are high-confidence (a `using` with no matching
  type in the file is definitionally removable); the **transitive** negatives
  (that severing the Orleans/Roslyn refs breaks nothing pulled in *indirectly*)
  should be re-verified on a machine that builds.

- **Does any kept ProjectReference re-introduce Orleans or net9-pinning?** The
  `LionFire.Data.Async.*` and `LionFire.UI.*` and `LionFire.Inspection` /
  `LionFire.Mvvm` projects are all `net9.0` today and were not audited for their
  *own* Orleans/Radzen content. Phase 4 (multi-target) depends on them becoming
  net10-capable; that cascade was not scoped here.

- **`NotificationsDropdown` orphan status.** Grep found no consumers under
  `/src/Core/src`, but external consumers (other repos, axi-web, samples outside
  this tree) were not exhaustively searched. If something *does* consume it, the
  Radzen-wrapper replacement (vs. removal) is the safer choice — but either way
  Radzen leaves.

- **Central package pin.** Whether `Radzen.Blazor` (pinned `6.1.6`) and the
  `Microsoft.CodeAnalysis.*` versions in `/src/Core/Directory.Packages.props` can
  be removed depends on *other* projects in the solution — not analyzed here.
  Removing this library's `PackageReference`s is independent of the central pin.

- **Why were these refs added?** The vestigial usings look IDE-auto-inserted; the
  `Orleans.Structures.Abstractions` ProjectReference may be a copy-paste from a
  sibling Orleans-aware library or a remnant of a refactor that moved grain logic
  out. No git-history archaeology was performed to confirm intent.


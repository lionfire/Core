# Codebase Structure

**Analysis Date:** 2026-01-18

## Directory Layout

```
Core/
├── src/                        # All source code (~240 projects)
│   ├── LionFire.Base/          # BCL augmentation (zero dependencies)
│   ├── LionFire.Flex/          # FlexData dynamic extension pattern
│   ├── LionFire.Structures/    # Data structures and collections
│   ├── LionFire.Core/          # Core metadata, DI, MultiTyping
│   ├── LionFire.Environment/   # Environment detection
│   ├── LionFire.Hosting*/      # Application hosting
│   ├── LionFire.Referencing*/  # URL/Reference system
│   ├── LionFire.Persistence*/  # Storage backends
│   ├── LionFire.Serialization*/# Serializers (HJSON, JSON, YAML)
│   ├── LionFire.Data.Async*/   # Async data patterns
│   ├── LionFire.Vos*/          # Virtual Object System
│   ├── LionFire.Assets*/       # Entity/ID referencing
│   ├── LionFire.Blazor*/       # Blazor components
│   ├── LionFire.AspNetCore*/   # ASP.NET Core integration
│   ├── LionFire.Framework/     # Opinionated framework
│   ├── LionFire.Mvvm*/         # MVVM patterns
│   └── [many more...]
├── test/                       # Test projects (~30 projects)
│   ├── LionFire.Core.Tests/
│   ├── LionFire.Vos.Tests/
│   ├── LionFire.Persistence.*.Tests/
│   └── [component].Tests/
├── samples/                    # Sample applications
│   ├── LionFire.Blazor.Components.MudBlazor.Samples/
│   └── [other samples]
├── docs/                       # Documentation
│   ├── README.md               # Documentation index
│   ├── TASKS.md                # Documentation roadmap
│   ├── architecture/           # Architecture docs
│   ├── data/                   # Data domain docs (VOS, async)
│   ├── mvvm/                   # MVVM domain docs
│   ├── ui/                     # UI/Blazor domain docs
│   ├── reactive/               # Reactive programming docs
│   ├── workspaces/             # Workspace architecture
│   └── guides/                 # How-to guides
├── attic/                      # Deprecated/archived code
├── build/                      # Build scripts and outputs
├── tools/                      # Development tools
├── deps/                       # External dependencies (vendored)
├── lib/                        # Pre-built libraries
├── .planning/                  # Planning documents
│   └── codebase/               # Codebase analysis documents
├── .github/workflows/          # GitHub Actions
├── Directory.Build.props       # Central MSBuild properties
├── Directory.Packages.props    # Central Package Management versions
├── global.json                 # .NET SDK version pinning
├── CLAUDE.md                   # Repository AI instructions
├── README.md                   # Repository overview
├── All.sln                     # Comprehensive solution (all projects)
├── Core.sln                    # Core libraries solution
├── Execution.sln               # Execution-related solution
└── Machine.sln                 # Machine-specific solution
```

## Directory Purposes

**`src/`:**
- Purpose: All production source code
- Contains: ~240 .NET projects organized by component/feature
- Key files: Each project has `.csproj`, often `CLAUDE.md` for AI context
- Pattern: `LionFire.[Domain][.Subdomain]` naming

**`test/`:**
- Purpose: Unit and integration tests
- Contains: Test projects matching production projects
- Key files: `*.Tests.csproj`, test classes
- Pattern: `LionFire.[Component].Tests/` naming

**`samples/`:**
- Purpose: Sample applications demonstrating library usage
- Contains: Runnable sample apps (Blazor, Console)
- Key files: `Program.cs`, sample components

**`docs/`:**
- Purpose: Comprehensive documentation
- Contains: Domain guides, architecture docs, how-tos
- Key files: `README.md` (index), domain-specific markdown

**`attic/`:**
- Purpose: Deprecated code kept for reference
- Contains: Old projects no longer maintained
- Status: Do not use; reference only

**`build/`:**
- Purpose: Build artifacts and scripts
- Contains: Publish outputs, build tools

## Key File Locations

**Entry Points:**
- `src/LionFire.Hosting.Samples.Console/Program.cs`: Simple host bootstrap example
- `src/LionFire.Hosting.CommandLine.Sample/Program.cs`: CLI app example
- `src/LionFire.Agent.Api.Host/Program.cs`: API host example
- `src/LionFire.Vos.Api.Host/Program.cs`: VOS API host example

**Configuration:**
- `Directory.Build.props`: Global MSBuild properties (version, symbols, output paths)
- `Directory.Packages.props`: Central Package Management (all NuGet versions)
- `global.json`: .NET SDK version (currently 9.0.107)
- `.editorconfig`: Code style settings (file-scoped namespaces)

**Core Logic (by layer):**
- `src/LionFire.Base/ExtensionMethods/`: BCL extension methods
- `src/LionFire.Core/MultiTyping/`: MultiTyping system
- `src/LionFire.Core/DependencyInjection/`: DI extensions
- `src/LionFire.Hosting/LionFireHostBuilder/`: Host bootstrap
- `src/LionFire.Vos/Vob/`: Virtual Object implementation
- `src/LionFire.Persistence/Persisters/`: Storage backends

**Testing:**
- `test/LionFire.Core.Tests/`: Core library tests
- `test/LionFire.Vos.Tests/`: VOS tests
- `test/LionFire.Persistence.*.Tests/`: Persistence tests
- `test/multi.runsettings`: Multi-threaded test settings
- `test/single.runsettings`: Single-threaded test settings

**Documentation per project:**
- `src/[Project]/CLAUDE.md`: AI context and API documentation
- 11+ completed CLAUDE.md files (see `docs/README.md` for list)

## Naming Conventions

**Files:**
- `.cs` files: PascalCase matching type name (e.g., `LionFireHostBuilder.cs`)
- Interface files: `I` prefix (e.g., `IReference.cs`)
- Extension method files: `[Type]Extensions.cs` (e.g., `StringExtensions.cs`)
- Test files: `[Type]Tests.cs` in matching `*.Tests` project

**Directories:**
- Components: PascalCase matching namespace segment (e.g., `MultiTyping/`)
- Abstractions projects: `LionFire.[Domain].Abstractions/`
- Implementation projects: `LionFire.[Domain]/`
- Test projects: `LionFire.[Domain].Tests/`
- UI-specific: `LionFire.Blazor.*`, `LionFire.Avalon.*`, `LionFire.Wpf.*`

**Projects:**
- Pattern: `LionFire.[Domain][.Subdomain][.Platform]`
- Examples:
  - `LionFire.Base` (foundational)
  - `LionFire.Core` (core toolkit)
  - `LionFire.Vos.Abstractions` (abstractions)
  - `LionFire.Persistence.Filesystem` (implementation)
  - `LionFire.Blazor.Components.MudBlazor` (UI framework)

**Namespaces:**
- Root: `LionFire` (most projects)
- Subdomain: `LionFire.[Domain]` as needed
- Note: Some projects use root `LionFire` namespace regardless of project name

## Where to Add New Code

**New Feature (Core functionality):**
- Primary code: Create `src/LionFire.[FeatureName]/` project
- Abstractions: Create `src/LionFire.[FeatureName].Abstractions/` if needed
- Tests: Create `test/LionFire.[FeatureName].Tests/`
- Add to `All.sln` (and domain-specific `.sln` if applicable)

**New Component/Module (within existing project):**
- Implementation: `src/LionFire.[Project]/[ComponentName]/`
- Follow existing folder structure within project
- Match naming to existing patterns in that project

**Utilities:**
- Extension methods: `src/LionFire.Base/ExtensionMethods/System/[Type]/`
- Shared helpers: `src/LionFire.Base/[Category]/` or `src/LionFire.Structures/[Category]/`
- Core utilities: `src/LionFire.Core/[Category]/`

**New Serializer:**
- Create: `src/LionFire.Serialization.[Format]/`
- Implement: `SerializerBase<T>` from `LionFire.Persistence`
- Reference existing: `src/LionFire.Serialization.Hjson/`, `src/LionFire.Serialization.Json.Newtonsoft/`

**New Persistence Backend:**
- Create: `src/LionFire.Persistence.[Backend]/`
- Implement: Persister interfaces from `LionFire.Persistence.Abstractions`
- Reference existing: `src/LionFire.Persistence.Filesystem/`, `src/LionFire.Persistence.LiteDB/`

**New Blazor Component:**
- UI-agnostic: `src/LionFire.Blazor.Components/`
- MudBlazor-specific: `src/LionFire.Blazor.Components.MudBlazor/`
- Samples: `samples/LionFire.Blazor.Components.MudBlazor.Samples/`

## Special Directories

**`.planning/codebase/`:**
- Purpose: Codebase analysis documents (this file)
- Generated: By analysis tools/agents
- Committed: Yes

**`attic/`:**
- Purpose: Deprecated code archive
- Generated: No (manually moved)
- Committed: Yes (historical reference)

**`build/publish/`:**
- Purpose: Published application outputs
- Generated: Yes (by build process)
- Committed: Some outputs may be committed

**`deps/`:**
- Purpose: Vendored external dependencies
- Generated: No (manually added)
- Committed: Yes

**`lib/`:**
- Purpose: Pre-built libraries
- Generated: No
- Committed: Yes

**`.vs/`, `.idea/`:**
- Purpose: IDE settings
- Generated: Yes (by IDE)
- Committed: Partially (via `.gitignore`)

**`bin/`, `obj/` (within projects):**
- Purpose: Build outputs
- Generated: Yes (by MSBuild)
- Committed: No (excluded by `.gitignore`)

## Solution Organization

**All.sln:**
- Contains: All ~240 projects
- Use for: Complete builds, comprehensive work

**Core.sln:**
- Contains: Core libraries subset
- Use for: Focused work on core functionality

**Execution.sln:**
- Contains: Execution-related projects
- Use for: Work on execution engine

**Machine.sln:**
- Contains: Machine-specific projects
- Use for: Hardware/machine integration work

## Central Package Management

All package versions defined in `Directory.Packages.props`:
- Individual `.csproj` files reference packages WITHOUT version
- To update a version: Edit `Directory.Packages.props` only
- Example in csproj: `<PackageReference Include="Serilog" />` (no Version attribute)

---

*Structure analysis: 2026-01-18*

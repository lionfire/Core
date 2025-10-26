# LionFire.Core Claude Skills

Skills for working with the LionFire.Core framework, including workspace-scoped MVVM patterns, reactive data access, and Blazor integration.

## Plugin Installation

### Option 1: Install from Local Directory

From within Claude Code:

```
/plugin add /src/tp/Core
```

Then install the plugin:

```
/plugin install lionfire-core-patterns@lionfire-core
```

### Option 2: Direct Skill Access

You can also directly reference skills from this directory without installing the plugin by using the full path in your requests.

---

## Available Skills

### workspace-blazor-mvvm

**Purpose**: Guide for implementing workspace documents with Blazor MVVM patterns.

**Use When**:
- Creating Blazor pages for workspace-scoped documents
- Using `ObservableDataView` component
- Fixing workspace service injection errors
- Implementing list/detail views with reactive persistence

**Triggers**:
- "Create a workspace document page"
- "Add ObservableDataView for my entities"
- "Fix unable to resolve IObservableReader error"
- "Implement list and detail views for workspace documents"

**Key Patterns Taught**:
1. **ObservableDataView** - Automatic reactive grids for lists
2. **Manual VM Creation** - Full control for detail views
3. **Workspace Service Scoping** - Using CascadingParameter correctly

**Example Usage**:
```
User: "I need to create a list page for Portfolio entities in my workspace"
Claude: [Loads workspace-blazor-mvvm skill]
        [Guides user through ObservableDataView pattern with code examples]
```

---

## Skill Structure

```
workspace-blazor-mvvm/
├── SKILL.md                           # Main skill instructions
├── references/                        # Detailed documentation (symlinks)
│   ├── service-scoping.md            → docs/architecture/workspaces/service-scoping.md
│   ├── blazor-mvvm-patterns.md       → docs/ui/blazor-mvvm-patterns.md
│   └── workspaces-architecture.md    → docs/architecture/workspaces/README.md
└── examples/                          # Code examples
    ├── list-view-example.razor       # ObservableDataView pattern
    └── detail-view-example.razor     # Manual VM pattern
```

**Benefits**:
- **Progressive Disclosure**: SKILL.md stays lean (<2k words), references loaded as needed
- **Single Source of Truth**: References are symlinks to main documentation
- **Reusable Examples**: Copy-paste ready code patterns

---

## Documentation Integration

The skills reference the comprehensive documentation created in `docs/`:

- **Architecture**: `docs/architecture/workspaces/`
- **Domain Guides**: `docs/workspaces/`, `docs/ui/`
- **How-To Guides**: `docs/guides/how-to/`

Skills provide **quick access** to these docs when working on specific tasks.

---

## Future Skills (Planned)

- **vos-patterns** - Virtual Object System usage patterns
- **async-data-patterns** - IGetter/ISetter/IValue patterns
- **reactive-mvvm** - ReactiveUI and DynamicData patterns
- **persistence-backends** - Custom persistence implementations

---

## Contributing

To add a new skill:

1. Create skill directory: `claude-skills/my-skill-name/`
2. Create `SKILL.md` with frontmatter
3. Add to `.claude-plugin/marketplace.json`
4. Create examples and references as needed
5. Test with Claude Code

---

## Related Documentation

- **[Workspace Architecture](../docs/architecture/workspaces/README.md)**
- **[Blazor MVVM Patterns](../docs/ui/blazor-mvvm-patterns.md)**
- **[Complete Documentation Index](../docs/README.md)**

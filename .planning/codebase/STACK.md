# Technology Stack

**Analysis Date:** 2026-01-18

## Languages

**Primary:**
- C# (Latest version via `<LangVersion>latest</LangVersion>`) - All source code

**Secondary:**
- SQL (PostgreSQL) - Orleans clustering/persistence scripts in `src/LionFire.Orleans.Hosting/SQL/Postgres/`
- YAML - CI/CD configuration files
- XML - MSBuild project files

## Runtime

**Environment:**
- .NET 10.0 (Primary target for most projects)
- .NET Standard 2.0 (LionFire.Base supports both netstandard2.0 and net10.0)
- .NET Framework 4.7.2 (Legacy WPF projects)

**SDK Version:**
- .NET SDK 10.0.100 (defined in `global.json`)
- Roll forward policy: `latestMinor`

**Package Manager:**
- NuGet with Central Package Management (CPM)
- Lockfile: Not present (versions managed in `Directory.Packages.props`)

## Frameworks

**Core:**
- Microsoft.Extensions.Hosting 10.0.1 - Application hosting
- Microsoft.Extensions.DependencyInjection 10.0.1 - Dependency injection
- Microsoft.Extensions.Configuration 10.0.1 - Configuration management
- Microsoft.Extensions.Logging 10.0.1 - Logging abstractions

**Web:**
- ASP.NET Core 10.0 (via FrameworkReference in relevant projects)
- Blazor (Server, WebAssembly, Hybrid) - Multiple component libraries

**UI Frameworks:**
- MudBlazor 8.15.0 - Primary Blazor component library
- Radzen.Blazor 6.1.6 - Alternative Blazor components
- Blazorise 1.7.5 - Additional Blazor components
- WPF (.NET Framework 4.7.2) - Desktop UI legacy projects
- Caliburn.Micro 4.0.230 - WPF MVVM framework

**Reactive:**
- System.Reactive 6.1.0 - Reactive extensions
- ReactiveUI 20.2.45 - MVVM with reactive patterns
- DynamicData 9.1.2 - Reactive collections

**Testing:**
- xUnit 2.9.3 - Primary test framework
- MSTest 3.8.3 - Alternative test framework
- Microsoft.NET.Test.Sdk 17.13.0 - Test SDK
- coverlet.collector 6.0.4 - Code coverage

**Build/Dev:**
- GitVersion.MsBuild 6.1.0 - Semantic versioning
- Central Package Management - Version control

## Key Dependencies

**Critical:**
- Microsoft.Extensions.Hosting 10.0.1 - Core hosting infrastructure
- System.Reactive 6.1.0 - Reactive programming foundation
- ReactiveUI 20.2.45 - MVVM and reactive patterns throughout
- System.Collections.Immutable 10.0.1 - Immutable data structures

**Serialization:**
- Newtonsoft.Json 13.0.4 - JSON serialization
- System.Text.Json 10.0.1 - Modern JSON serialization
- YamlDotNet 16.3.0 - YAML serialization
- Hjson 3.0.0 - Human-friendly JSON

**Logging:**
- Serilog 4.2.0 - Structured logging (primary)
- NLog 5.4.0 - Alternative logging framework
- Serilog.Sinks.* - Console, File, Loki, OpenTelemetry sinks

**Data Access:**
- Entity Framework Core 9.0.3 - ORM
- Dapper 2.1.66 - Micro-ORM
- Marten 7.39.1 - Document database on PostgreSQL
- LiteDB 5.0.21 - Embedded NoSQL database
- StackExchange.Redis 2.8.31 - Redis client

**Distributed Systems:**
- Microsoft.Orleans.* 9.1.2 - Virtual actor framework
- WolverineFx 3.10.1 - Message bus/CQRS framework
- MediatR 12.4.1 - In-process mediator pattern
- NATS.Net 2.7.0 - Message broker client

**Observability:**
- OpenTelemetry 1.14.0 - Distributed tracing
- prometheus-net 8.2.1 - Metrics
- AspNetCore.HealthChecks.* 9.0.0 - Health checks

**Authentication:**
- OpenIddict 6.1.1 - OpenID Connect implementation
- System.IdentityModel.Tokens.Jwt 8.15.0 - JWT handling
- Microsoft.AspNetCore.Authentication.JwtBearer 10.0.1 - JWT bearer auth

**Resilience:**
- Microsoft.Extensions.Resilience 10.1.0 - Resilience patterns
- Polly 8.5.2 - Fault handling and retry policies
- Nito.AsyncEx 5.1.2 - Async coordination primitives

**Infrastructure:**
- Consul 1.7.14.2 - Service discovery
- WolverineFx.RabbitMQ 3.10.1 - RabbitMQ messaging
- Npgsql 10.0.1 - PostgreSQL client

## Configuration

**Environment:**
- Microsoft.Extensions.Configuration with multiple providers
- JSON configuration files (`appsettings.json` pattern)
- Environment variables
- User secrets (development)
- Command-line arguments

**Key Configuration Files:**
- `Directory.Build.props` - Common MSBuild settings
- `Directory.Packages.props` - Centralized package versions (CPM)
- `global.json` - SDK version pinning
- `.editorconfig` - Code style (file-scoped namespaces)
- `GitVersion.yml` - Semantic versioning

**Build Configuration:**
- Version: `8.0.0-preview` (defined in `Directory.Build.props`)
- Portable PDB with embedded sources
- Symbol packages (snupkg) for debugging
- Reference assemblies generated
- NuGet packages generated on Release builds

## Platform Requirements

**Development:**
- Windows 10/11 with WSL2 (primary development environment)
- .NET 10.0 SDK
- Visual Studio 2022+ or VS Code
- Use `dotnet-win` command (Windows .NET via WSL2 bridge)

**Production:**
- Linux containers (Docker support via `docker-compose.yml`)
- Windows Server (for WPF applications)
- Any platform supporting .NET 10.0

**CI/CD:**
- Azure Pipelines (`azure-pipelines.yml`) - Windows latest
- Travis CI (`.travis.yml`) - Legacy, .NET 2.0 era
- GitHub Actions (`.github/` directory present)

## Project Count

- Total projects: ~235 .csproj files in `src/`
- Solution files: Core.sln, All.sln, Execution.sln, Machine.sln

---

*Stack analysis: 2026-01-18*

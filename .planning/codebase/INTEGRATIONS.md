# External Integrations

**Analysis Date:** 2026-01-18

## APIs & External Services

**Service Discovery:**
- Consul - Service discovery and configuration
  - SDK/Client: `Consul` 1.7.14.2
  - Projects: `src/LionFire.AspNetCore.Consul/`, `src/LionFire.Orleans.Hosting/`
  - Orleans integration: `Microsoft.Orleans.Clustering.Consul` 9.1.2

**Email:**
- Mailgun - Email delivery
  - SDK/Client: `FluentEmail.Mailgun` 3.0.2
  - Project: `src/LionFire.Email/LionFire.Email.csproj`

**SMS/Communications:**
- Twilio - SMS and voice
  - SDK/Client: `Twilio` 7.9.0
  - Defined in `Directory.Packages.props`

**Trading/Financial:**
- Binance - Cryptocurrency exchange
  - SDK/Client: `Binance.Net` 10.18.0, `BinanceDotNet` 4.12.0
  - Projects: Trading-related code

- cTrader - Forex trading automation
  - SDK/Client: `cTrader.Automate` 1.0.10

- MetaTrader/NJ4X - Forex trading
  - SDK/Client: `nj4x` 2.9.3

**Game Engines:**
- Unity3D - Game development
  - SDK/Client: `Unity3D` 3.0.0
  - Projects: `src/LionFire.Hosting.Unity/`, `src/LionFire.Platform.Unity/`, `src/LionFire.Unity/`

- Stride Engine - Game development
  - SDK/Client: `Stride.Engine` 4.2.0.1, `Stride.Physics` 4.2.0.1

- Noesis GUI - Game UI
  - SDK/Client: `Noesis.GUI` 3.2.7

## Data Storage

**Databases:**

*PostgreSQL:*
- Client: `Npgsql` 10.0.1
- ORM: `Npgsql.EntityFrameworkCore.PostgreSQL` 9.0.4
- Document DB: `Marten` 7.39.1
- Projects: `src/LionFire.Persistence.Marten/`, Orleans persistence

*SQLite:*
- Client: `Microsoft.EntityFrameworkCore.Sqlite` 9.0.3
- Used for: OpenIddict local storage, health checks UI

*SQL Server:*
- Client: `Microsoft.EntityFrameworkCore.SqlServer` 9.0.3
- Defined in `Directory.Packages.props`

*Redis:*
- Client: `StackExchange.Redis` 2.8.31
- Orleans: `Microsoft.Orleans.Clustering.Redis`, `Microsoft.Orleans.Persistence.Redis` 9.1.2
- Projects: `src/LionFire.Persistence.Redis/`, `src/LionFire.ObjectBus.Redis/`
- Health checks: `AspNetCore.HealthChecks.Redis` 9.0.0

*LiteDB:*
- Client: `LiteDB` 5.0.21
- Projects: `src/LionFire.LiteDB/`, `src/LionFire.Persistence.LiteDB/`

*CouchDB:*
- Client: `MyCouch` 7.6.0
- Projects: `src/LionFire.CouchDB/`, `src/LionFire.Persistence.CouchDB/`

*RethinkDB:*
- Client: `RethinkDb.Driver` 2.3.150
- Defined in `Directory.Packages.props`

**File Storage:**
- Local filesystem (primary via VOS)
- SharpZipLib 1.4.2 for archive support (`src/LionFire.Persistence.SharpZipLib/`)
- LZ4 compression: `K4os.Compression.LZ4.Streams` 1.3.8

**Caching:**
- Redis (via StackExchange.Redis)
- In-memory (Microsoft.Extensions.Caching)

## Message Brokers

**RabbitMQ:**
- Integration: `WolverineFx.RabbitMQ` 3.10.1
- Project: `src/LionFire.Agent.Api.Host/` uses Wolverine with RabbitMQ

**NATS:**
- Client: `NATS.Net` 2.7.0
- Project: `src/LionFire.Nats.RequestReplyExemplar.Host/`

**SignalR:**
- Client: `DotNetify.SignalR` 5.4.0
- Client: `Microsoft.AspNetCore.SignalR` 1.2.0
- Built-in via Blazor Server for real-time updates

## Authentication & Identity

**Auth Providers:**

*OpenIddict:*
- Server: `OpenIddict.AspNetCore` 6.1.1
- Client: `OpenIddict.Client.AspNetCore`, `OpenIddict.Client.WebIntegration` 6.1.1
- Storage: `OpenIddict.EntityFrameworkCore` 6.1.1
- Project: `src/LionFire.Auth.OpenIddict.Client/`

*JWT:*
- Library: `System.IdentityModel.Tokens.Jwt` 8.15.0
- Bearer auth: `Microsoft.AspNetCore.Authentication.JwtBearer` 10.0.1

*IdentityModel:*
- Library: `IdentityModel` 7.0.0

## Monitoring & Observability

**Logging:**

*Serilog (Primary):*
- Core: `Serilog` 4.2.0, `Serilog.AspNetCore` 9.0.0
- Sinks: Console, File, Debug, Grafana Loki, OpenTelemetry
- Configuration: `Serilog.Settings.Configuration` 10.0.0
- Project: `src/LionFire.Hosting/Logging/Serilog/`

*NLog (Alternative):*
- Core: `NLog` 5.4.0
- Extensions: `NLog.Extensions.Logging`, `NLog.Web.AspNetCore` 5.4.0
- Project: `src/LionFire.Extensions.Logging.NLog/`

**Tracing:**
- OpenTelemetry 1.14.0 - Distributed tracing
- OTLP exporter for Jaeger/Tempo/etc.
- ASP.NET Core, HTTP, gRPC instrumentation
- Process and runtime instrumentation

**Metrics:**
- Prometheus: `prometheus-net` 8.2.1
- OpenTelemetry Prometheus exporter (beta)
- Orleans Dashboard: `OrleansDashboard` 8.2.0

**Health Checks:**
- UI: `AspNetCore.HealthChecks.UI` 9.0.0
- Checks: Consul, Network, Npgsql, Redis, System
- Storage: InMemory, SQLite
- Project: `src/LionFire.AspNetCore.Extras/`, `src/LionFire.AspNetCore.Framework/`

## CI/CD & Deployment

**Hosting:**
- Docker containers (via `docker-compose.yml`)
- Azure (Azure Pipelines)
- Any .NET 10.0 compatible host

**CI Pipelines:**

*Azure Pipelines:*
- Config: `azure-pipelines.yml`
- Trigger: master branch
- Platform: windows-latest
- Build: All.sln, Release configuration

*Travis CI (Legacy):*
- Config: `.travis.yml`
- Build: Core.sln

*GitHub:*
- Actions directory: `.github/`

**Container Support:**
- Docker Compose: `docker-compose.yml`
- Docker targets: `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.22.0

**NuGet Publishing:**
- Local feed: `LionFireLocal` (configured in Directory.Build.props AfterPack target)
- Packages and symbol packages (.snupkg) pushed on Release builds

## Environment Configuration

**Required Environment Variables:**
- Database connection strings (PostgreSQL, Redis, etc.)
- Consul address (when using service discovery)
- OpenIddict client credentials
- Serilog Loki endpoint (when using Grafana Loki sink)
- External API keys (Twilio, Binance, etc.)

**Configuration Sources:**
- `appsettings.json` - Base configuration
- `appsettings.{Environment}.json` - Environment-specific
- Environment variables
- User secrets (development)
- Command-line arguments (via System.CommandLine)

**Secrets Location:**
- User secrets (development): standard .NET user secrets
- Production: Environment variables or external secrets manager (not specified)

## Webhooks & Callbacks

**Incoming:**
- ASP.NET Core controllers for webhook endpoints
- SignalR hubs for real-time communication
- Orleans grain endpoints

**Outgoing:**
- HTTP callbacks via `Microsoft.Extensions.Http` 10.0.1
- Polly retry policies: `Microsoft.Extensions.Http.Polly` 9.0.3, `Polly` 8.5.2

## API Documentation

**Swagger/OpenAPI:**
- Swashbuckle: `Swashbuckle.AspNetCore` 7.3.1
- NSwag: `NSwag.AspNetCore` 14.6.3
- Project: `src/LionFire.Orleans.Http.Swagger/`

## Distributed Computing

**Orleans (Virtual Actors):**
- Core: `Microsoft.Orleans.Sdk` 9.1.2
- Server: `Microsoft.Orleans.Server` 9.1.2
- Client: `Microsoft.Orleans.Client` 9.1.2
- Clustering: Consul, Redis
- Persistence: Redis, AdoNet (PostgreSQL)
- Serialization: `Microsoft.Orleans.Serialization.MessagePack` 9.1.2
- Projects: `src/LionFire.Orleans*/`

**Wolverine (CQRS/Messaging):**
- Core: `WolverineFx` 3.10.1
- HTTP: `WolverineFx.Http` 3.10.1
- Marten integration: `WolverineFx.Marten` 3.10.1
- RabbitMQ: `WolverineFx.RabbitMQ` 3.10.1
- Project: `src/LionFire.Agent.Api.Host/`

**MagicOnion (gRPC-based RPC):**
- Client: `MagicOnion.Client` 7.0.2

---

*Integration audit: 2026-01-18*

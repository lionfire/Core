# Testing Patterns

**Analysis Date:** 2026-01-18

## Test Framework

**Runner:**
- xUnit (version managed via `Directory.Packages.props`)
- Config: Individual test project `.csproj` files

**Assertion Library:**
- xUnit built-in assertions (`Assert.Equal`, `Assert.True`, `Assert.Throws`)
- DeepEqual package for object comparison (used in some projects)

**Run Commands:**
```bash
dotnet-win test                                    # Run all tests
dotnet-win test test/LionFire.Core.Tests/          # Run specific project
dotnet-win test --logger "console;verbosity=detailed"  # Detailed output
```

## Test File Organization

**Location:**
- Separate `test/` directory at repository root
- Pattern: `test/LionFire.<Component>.Tests/`

**Naming:**
- Test projects: `LionFire.<Component>.Tests.csproj`
- Test classes: Match system under test with underscore suffix
- Test methods: `P_` prefix for Pass, `F_` prefix for Fail scenarios

**Structure:**
```
test/
├── LionFire.Core.Tests/
│   └── TypeNameRegistry_/
│       └── RegisterTypeNames_.cs
├── LionFire.Persistence.Filesystem.Tests/
│   ├── FilesystemPersister_/
│   │   └── NewtonsoftJson/
│   │       ├── _Update.cs
│   │       └── _Upsert.cs
│   └── List_/
│       └── Filenames_.cs
├── LionFire.Vos.Tests/
│   ├── VosReference_/
│   │   └── _Construction.cs
│   └── String_/
│       └── _VosReference.cs
└── LionFire.Persistence.Testing/       # Shared test utilities
    ├── TestClass1.cs
    ├── TestClass2.cs
    ├── Filesystem/
    │   └── FsTestSetup.cs
    └── PersistenceTestUtils.cs
```

## Test Structure

**Suite Organization:**
```csharp
namespace TypeNameRegistry_
{
    public class RegisterTypeNames_
    {
        [Fact]
        public async void P_Typical()
        {
            await new HostBuilder()
                .ConfigureServices(services =>
                {
                    services
                        .RegisterTypeNames(typeof(TypeResolver).Assembly, concreteTypesOnly: true)
                        .AddTypeNameRegistry();
                })
                .RunAsync(serviceProvider =>
                {
                    var registry = serviceProvider.GetRequiredService<IOptionsMonitor<TypeNameRegistry>>().CurrentValue;
                    var resolvedType = registry.Types.TryGetValue("TypeResolver");
                    Assert.Same(typeof(TypeResolver), resolvedType);
                });
        }

        [Fact]
        public async void F_ConcreteOnly()
        {
            // Test that interfaces are not registered when concreteTypesOnly=true
            await new HostBuilder()
                .ConfigureServices(services => { ... })
                .RunAsync(serviceProvider =>
                {
                    var typeResolver = serviceProvider.GetRequiredService<ITypeResolver>();
                    Assert.Throws<TypeNotFoundException>(() => typeResolver.Resolve("ITypeResolver"));
                });
        }
    }
}
```

**Patterns:**
- Arrange-Act-Assert within async lambdas
- `P_` prefix for tests expected to pass
- `F_` prefix for tests expected to fail/throw
- Test class names match the system under test with underscore suffix
- Nested namespaces for grouping related tests

## Host-Based Testing Pattern

**Common Pattern:**
Tests use `IHostBuilder` to create a configured service container:

```csharp
[Fact]
public async void P_TestObj()
{
    await NewtonsoftJsonFilesystemTestHost.Create().RunAsync(async () =>
    {
        var path = FsTestSetup.TestFile + ".json";
        Assert.False(File.Exists(path));

        File.WriteAllText(path, TestClass1.ExpectedNewtonsoftJson);
        Assert.True(File.Exists(path));

        var testContents2 = TestClass1.Create;
        testContents2.StringProp = "Contents #2";

        await ServiceLocator.Get<FilesystemPersister>().Update(path.ToFileReference(), testContents2);
        Assert.True(File.Exists(path));

        File.Delete(path);
        Assert.False(File.Exists(path));
    });
}
```

**Test Host Creation:**
- `src/test/LionFire.Persistence.Testing/` provides shared test hosts
- Hosts configure DI, serialization, and persistence for tests

## Mocking

**Framework:**
- No dedicated mocking framework detected (Moq/NSubstitute not in `Directory.Packages.props`)
- Tests rely on real implementations with test configurations

**Patterns:**
- Use real services with test-specific configurations
- Create test fixtures and data classes
- Use `ServiceLocator.Get<T>()` for service resolution in tests

**What to Mock:**
- External services (not observed in current codebase)
- Filesystem operations use temp directories

**What NOT to Mock:**
- DI container - use real `IServiceProvider`
- Serializers - use actual implementations
- Options - use real `IOptionsMonitor<T>`

## Fixtures and Factories

**Test Data:**
```csharp
namespace LionFire.Persistence.Testing
{
    public class TestClass1
    {
        public string StringProp { get; set; }
        public int IntProp { get; set; }
        public TestClass2 Object { get; set; }

        public static TestClass1 Create => new TestClass1()
        {
            StringProp = "string1",
            IntProp = 1,
            Object = new TestClass2
            {
                StringProp2 = "string2",
                IntProp2 = 2,
            }
        };

        public static string ExpectedNewtonsoftJson = @"{""$type"":""LionFire.Persistence.Testing.TestClass1, LionFire.Persistence.Testing"",""StringProp"":""string1"",""IntProp"":1,""Object"":{""StringProp2"":""string2"",""IntProp2"":2}}";
    }
}
```

**Test Setup Utilities:**
```csharp
public class FsTestSetup
{
    public static bool EnableFileCleanup = true;
    public static string DataDir => Path.GetTempPath();

    public static void AssertEqual(TestClass1 obj, object deserialized)
    {
        Assert.Equal(typeof(TestClass1), deserialized.GetType());
        var obj2 = (TestClass1)deserialized;
        Assert.Equal(obj.StringProp, obj2.StringProp);
        Assert.Equal(obj.IntProp, obj2.IntProp);
    }

    public static string TestFile => Path.Combine(DataDir, "UnitTest " + Guid.NewGuid().ToString());

    public static void CleanPath(string path)
    {
        if (!path.StartsWith(DataDir)) throw new ArgumentException("CleanPath only works for files in Path.GetTempPath()");
        if (!EnableFileCleanup) return;
        File.Delete(path);
        Assert.False(File.Exists(path), "Cleanup failed.  Delete file: " + path);
    }
}
```

**Location:**
- Shared test utilities: `test/LionFire.Persistence.Testing/`
- Project-specific fixtures: Within test project

## Coverage

**Requirements:** None enforced (no coverage configuration detected)

**View Coverage:**
```bash
dotnet-win test --collect:"XPlat Code Coverage"
```

## Test Types

**Unit Tests:**
- Focus on individual components with minimal dependencies
- Use host builder pattern for DI setup
- Example: `test/LionFire.Core.Tests/TypeNameRegistry_/RegisterTypeNames_.cs`

**Integration Tests:**
- Test persistence with actual filesystem
- Use temp directories for file operations
- Example: `test/LionFire.Persistence.Filesystem.Tests/`

**E2E Tests:**
- Not observed in current codebase

## Common Patterns

**Async Testing:**
```csharp
[Fact]
public async void P_Typical()
{
    await new HostBuilder()
        .ConfigureServices(services => { ... })
        .RunAsync(async serviceProvider =>
        {
            // Test logic here
        });
}
```

**Error Testing:**
```csharp
[Fact]
public async void F_TestObj_Missing()
{
    await NewtonsoftJsonFilesystemTestHost.Create().RunAsync(async () =>
    {
        var path = FsTestSetup.TestFile + ".json";
        Assert.False(File.Exists(path));

        // Don't create the file - test missing file scenario
        var testContents2 = TestClass1.Create;

        await Assert.ThrowsAsync<NotFoundException>(() =>
            ServiceLocator.Get<FilesystemPersister>().Update(path.ToFileReference(), testContents2));

        Assert.False(File.Exists(path));
    });
}
```

**File Cleanup Pattern:**
```csharp
[Fact]
public async void P_bytes()
{
    await FilesystemTestHost.Create().RunAsync(async () =>
    {
        var path = FsTestSetup.TestFile + ".bin";
        Assert.False(File.Exists(path));

        var testContents = new byte[] { 1, 2, 3, 4, 5 };
        File.WriteAllBytes(path, testContents);
        Assert.True(File.Exists(path));

        // ... test operations ...

        File.Delete(path);
        Assert.False(File.Exists(path));
    });
}
```

## Test Parallelization

**Configuration:**
- Disabled globally in `test/XunitDisableParallelization.cs`:
```csharp
[assembly: CollectionBehavior(DisableTestParallelization = true)]
```

**Rationale:**
- Tests may share filesystem resources
- Integration tests with services may conflict

## xUnit Output

**Test Output Helper:**
```csharp
public class Stages_
{
    public ITestOutputHelper Output { get; }

    public Stages_(ITestOutputHelper output)
    {
        Output = output;
    }

    [Fact]
    public void P_RandomizedStageMembers()
    {
        var dsm = new DependencyStateMachine() { IsLoggingEnabled = true };
        dsm.Started += participant => Output.WriteLine($"Started '{participant}'");
        dsm.StartedStage += stageId => Output.WriteLine($"[stage {stageId}] started");
        // ...
    }
}
```

## Test Project Structure

**Standard Test Project (.csproj):**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <RootNamespace />
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="coverlet.collector">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\LionFire.Core\LionFire.Core.csproj" />
    <!-- Additional project references -->
  </ItemGroup>
</Project>
```

## Test Projects List

| Project | Purpose |
|---------|---------|
| `LionFire.Core.Tests` | Core library tests (TypeNameRegistry, etc.) |
| `LionFire.Persistence.Filesystem.Tests` | Filesystem persister tests |
| `LionFire.Persistence.Testing` | Shared test utilities and fixtures |
| `LionFire.Vos.Tests` | Virtual Object System tests |
| `LionFire.DependencyMachines.Tests` | Dependency state machine tests |
| `LionFire.Applications.Tests` | Application framework tests |
| `LionFire.Serialization.Tests` | Serialization tests |
| `LionFire.Assets.Tests` | Asset system tests |

---

*Testing analysis: 2026-01-18*

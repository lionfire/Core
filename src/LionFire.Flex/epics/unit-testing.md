# Unit Testing Plan for LionFire.Flex

## Overview
This document outlines a comprehensive unit testing strategy for the LionFire.Flex project, which provides a flexible object composition system with support for dynamic typing, metadata, and extensible object structures.

## Testing Scope

### Core Components to Test

#### 1. Core Interfaces and Base Classes
- **IFlex Interface** (`IFlex.cs`)
  - FlexData property get/set operations
  - Null handling
  - Thread safety considerations
  
- **FlexObject Class** (`FlexObject.cs`)
  - Constructor initialization (default and with value)
  - ToString() implementation
  - Property behavior

- **Flex Class** (`Flex.cs`)
  - Static Create method with various component types
  - Reflection-based component addition
  - Edge cases with null and empty arrays

#### 2. Extension Methods (`IFlexExtensions.cs`)
- **Metadata Operations**
  - Meta() extension method
  - ConditionalWeakTable behavior
  - IFlexWithMeta vs standard IFlex

- **Type Operations**
  - SingleValueType() with various object types
  - IsSingleValue() validation
  - SingleValueOrDefault() behavior
  - Type wrapping with TypedObject

- **Query and Retrieval**
  - GetOrCreate<T>() with various scenarios
  - Query<T>() operations
  - Thread-safe locking mechanisms

- **State Checks**
  - IsEmpty() validation
  - IsFlexImplementationType() checks

#### 3. Dictionary Support (`FlexDictionary.cs`)
- **FlexDictionary<TKey>**
  - GetFlex() and QueryFlex() operations
  - Concurrent operations thread safety
  - Add<T>() method behavior
  - Factory pattern implementation

#### 4. Options System
- **FlexOptions** (`FlexOptions.cs`)
  - Option creation and retrieval
  - Default values
  - Inheritance behavior

- **FlexGlobalOptions** (`FlexGlobalOptions.cs`)
  - Global configuration
  - Default factory patterns

- **FlexMemberOptions** (`FlexMemberOptions.cs`)
  - Member-specific configurations
  - Override mechanisms

#### 5. Default Type Support
- **DefaultTypeFlexObject** (`DefaultTypeFlexObject.cs`)
  - Type constraints enforcement
  - Type conversion behavior

- **SingleTypeFlexObject** (`SingleTypeFlexObject.cs`)
  - Single type enforcement
  - Invalid type rejection

#### 6. Change Notification
- **FlexChangeNotifier** (`FlexChangeNotifier.cs`)
  - Event firing mechanisms
  - Subscription/unsubscription

- **FlexChangeListener** (`FlexChangeListener.cs`)
  - Event handling
  - Callback registration

#### 7. Advanced Features
- **RecursiveFlexX** (`RecursiveFlexX.cs`)
  - Recursive structure handling
  - Circular reference detection

- **FlexTypeDictionary** (`FlexTypeDictionary.cs`)
  - Type registry operations
  - Type lookup performance

#### 8. Error Handling
- **CreationFailureException** (`CreationFailureException.cs`)
  - Exception scenarios
  - Error message formatting
  - Stack trace preservation

## Test Project Structure

```
LionFire.Flex.Tests/
├── Unit/
│   ├── Core/
│   │   ├── IFlexTests.cs
│   │   ├── FlexObjectTests.cs
│   │   └── FlexTests.cs
│   ├── Extensions/
│   │   ├── IFlexExtensionsTests.cs
│   │   ├── MetadataTests.cs
│   │   └── TypeOperationsTests.cs
│   ├── Dictionaries/
│   │   └── FlexDictionaryTests.cs
│   ├── Options/
│   │   ├── FlexOptionsTests.cs
│   │   ├── FlexGlobalOptionsTests.cs
│   │   └── FlexMemberOptionsTests.cs
│   ├── DefaultTypes/
│   │   ├── DefaultTypeFlexObjectTests.cs
│   │   └── SingleTypeFlexObjectTests.cs
│   ├── ChangeTracking/
│   │   ├── FlexChangeNotifierTests.cs
│   │   └── FlexChangeListenerTests.cs
│   └── Exceptions/
│       └── CreationFailureExceptionTests.cs
├── Integration/
│   ├── ThreadSafetyTests.cs
│   ├── PerformanceTests.cs
│   └── ComplexScenarioTests.cs
├── Fixtures/
│   ├── TestObjects.cs
│   └── TestHelpers.cs
└── LionFire.Flex.Tests.csproj
```

## Testing Framework and Tools

### Required NuGet Packages
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
  <PackageReference Include="xunit" Version="2.9.2" />
  <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
  <PackageReference Include="FluentAssertions" Version="6.12.1" />
  <PackageReference Include="Moq" Version="4.20.72" />
  <PackageReference Include="coverlet.collector" Version="6.0.2" />
  <PackageReference Include="BenchmarkDotNet" Version="0.14.0" />
</ItemGroup>
```

### Testing Approach
- **xUnit** - Primary testing framework
- **FluentAssertions** - Readable assertion syntax
- **Moq** - Mocking framework for dependencies
- **BenchmarkDotNet** - Performance benchmarking

## Test Categories

### 1. Unit Tests (Priority: High)

#### Basic Functionality Tests
```csharp
[Fact]
public void FlexObject_Constructor_InitializesWithNull()
[Fact]
public void FlexObject_Constructor_InitializesWithValue()
[Fact]
public void Flex_Create_HandlesMultipleComponents()
[Fact]
public void IFlex_FlexData_GetSet_WorksCorrectly()
```

#### Type System Tests
```csharp
[Theory]
[InlineData(typeof(string), "test")]
[InlineData(typeof(int), 42)]
public void SingleValueType_ReturnsCorrectType(Type expectedType, object value)
[Fact]
public void SingleTypeFlexObject_RejectsInvalidTypes()
[Fact]
public void TypedObject_WrapsValuesProperly()
```

#### Dictionary Operations Tests
```csharp
[Fact]
public void FlexDictionary_GetFlex_CreatesNewWhenMissing()
[Fact]
public void FlexDictionary_QueryFlex_ReturnsNullWhenMissing()
[Fact]
public void FlexDictionary_Add_AddsValueToFlex()
```

#### Extension Method Tests
```csharp
[Fact]
public void Meta_CreatesMetadataForStandardFlex()
[Fact]
public void Meta_UsesIFlexWithMeta_WhenAvailable()
[Fact]
public void GetOrCreate_CreatesWhenMissing()
[Fact]
public void GetOrCreate_ReturnsExistingWhenPresent()
```

### 2. Integration Tests (Priority: Medium)

#### Thread Safety Tests
```csharp
[Fact]
public async Task FlexDictionary_ConcurrentOperations_ThreadSafe()
[Fact]
public async Task GetOrCreate_ConcurrentCalls_CreateOnlyOnce()
[Fact]
public async Task Meta_ConditionalWeakTable_ThreadSafe()
```

#### Complex Scenario Tests
```csharp
[Fact]
public void ComplexObjectGraph_HandlesNestedFlexObjects()
[Fact]
public void RecursiveFlex_DetectsCircularReferences()
[Fact]
public void ChangeNotification_PropagatesThroughHierarchy()
```

### 3. Performance Tests (Priority: Low)

```csharp
[Benchmark]
public void GetOrCreate_Performance()
[Benchmark]
public void FlexDictionary_LookupPerformance()
[Benchmark]
public void TypeOperations_Performance()
```

## Test Data and Fixtures

### Test Objects
```csharp
public class TestPerson 
{
    public string Name { get; set; }
    public int Age { get; set; }
}

public class TestAddress 
{
    public string Street { get; set; }
    public string City { get; set; }
}

public interface ITestService 
{
    void DoSomething();
}
```

### Test Helpers
```csharp
public static class FlexTestHelpers
{
    public static IFlex CreateTestFlex(params object[] components)
    public static void AssertFlexContains<T>(IFlex flex, T expectedValue)
    public static void AssertFlexEmpty(IFlex flex)
}
```

## Code Coverage Goals

### Target Coverage Metrics
- **Overall Line Coverage**: ≥ 80%
- **Branch Coverage**: ≥ 75%
- **Critical Path Coverage**: 100%

### Critical Paths (Must Have 100% Coverage)
1. Core IFlex operations (get/set FlexData)
2. GetOrCreate<T>() method
3. Thread-safe operations in FlexDictionary
4. Exception handling paths
5. Type validation in SingleTypeFlexObject

### Areas Acceptable with Lower Coverage
1. ToString() implementations
2. Simple property getters/setters
3. Defensive null checks in edge cases

## Testing Best Practices

### 1. Test Naming Convention
```
MethodName_StateUnderTest_ExpectedBehavior
```
Example: `GetOrCreate_WhenValueMissing_CreatesNewInstance`

### 2. Arrange-Act-Assert Pattern
```csharp
[Fact]
public void GetOrCreate_WhenValueExists_ReturnsExisting()
{
    // Arrange
    var flex = new FlexObject();
    var expected = new TestPerson { Name = "John" };
    flex.Add(expected);
    
    // Act
    var result = flex.GetOrCreate<TestPerson>();
    
    // Assert
    result.Should().BeSameAs(expected);
}
```

### 3. Test Isolation
- Each test should be independent
- Use fresh instances for each test
- Avoid static state modifications

### 4. Mock External Dependencies
```csharp
var mockServiceProvider = new Mock<IServiceProvider>();
mockServiceProvider.Setup(x => x.GetService(typeof(ITestService)))
                   .Returns(new TestService());
```

## Implementation Timeline

### Phase 1: Core Functionality (Week 1-2)
- [ ] Set up test project structure
- [ ] Implement tests for IFlex, FlexObject, Flex
- [ ] Implement tests for basic IFlexExtensions methods
- [ ] Achieve 80% coverage for core components

### Phase 2: Advanced Features (Week 3-4)
- [ ] Implement tests for FlexDictionary
- [ ] Implement tests for Options system
- [ ] Implement tests for Default Type support
- [ ] Add thread safety tests

### Phase 3: Integration & Performance (Week 5)
- [ ] Implement complex scenario tests
- [ ] Add performance benchmarks
- [ ] Implement change notification tests
- [ ] Document any discovered issues

### Phase 4: Polish & Documentation (Week 6)
- [ ] Achieve target code coverage
- [ ] Add missing edge case tests
- [ ] Update documentation with test examples
- [ ] Create CI/CD integration for tests

## Continuous Integration

### GitHub Actions Workflow
```yaml
name: LionFire.Flex Tests

on:
  push:
    paths:
      - 'src/LionFire.Flex/**'
      - 'tests/LionFire.Flex.Tests/**'
  pull_request:
    paths:
      - 'src/LionFire.Flex/**'
      - 'tests/LionFire.Flex.Tests/**'

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
    - name: Restore dependencies
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore
    - name: Test
      run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"
    - name: Upload coverage
      uses: codecov/codecov-action@v3
```

## Known Challenges and Mitigations

### 1. Reflection-based Operations
**Challenge**: Testing reflection-based code in Flex.Create()
**Mitigation**: Use various type combinations, including edge cases like interfaces, abstract classes

### 2. WeakReference Testing
**Challenge**: ConditionalWeakTable behavior is non-deterministic with GC
**Mitigation**: Force GC collections in tests, use GC.KeepAlive strategically

### 3. Thread Safety
**Challenge**: Reproducing race conditions
**Mitigation**: Use Task.WhenAll with multiple concurrent operations, add delays strategically

### 4. Generic Method Testing
**Challenge**: Testing generic extension methods with various type parameters
**Mitigation**: Use Theory tests with multiple type parameters, test with value types and reference types

## Success Criteria

### Minimum Requirements
- [ ] All critical paths have 100% test coverage
- [ ] No failing tests in CI/CD pipeline
- [ ] Performance benchmarks establish baseline metrics
- [ ] Thread safety tests pass consistently

### Stretch Goals
- [ ] Mutation testing score > 75%
- [ ] Property-based testing for complex scenarios
- [ ] Fuzz testing for input validation
- [ ] Integration tests with dependent projects

## Review and Maintenance

### Monthly Review
- Review test coverage reports
- Update tests for new features
- Remove obsolete tests
- Optimize slow-running tests

### Quarterly Review
- Review and update performance baselines
- Evaluate testing strategy effectiveness
- Consider new testing tools/frameworks
- Update documentation

## Appendix: Example Test Implementation

```csharp
using Xunit;
using FluentAssertions;
using LionFire.FlexObjects;

namespace LionFire.Flex.Tests.Unit.Core
{
    public class FlexObjectTests
    {
        [Fact]
        public void Constructor_Default_CreatesEmptyFlex()
        {
            // Arrange & Act
            var flex = new FlexObject();
            
            // Assert
            flex.FlexData.Should().BeNull();
            flex.ToString().Should().Be("(null)");
        }
        
        [Theory]
        [InlineData("test string")]
        [InlineData(42)]
        [InlineData(3.14)]
        public void Constructor_WithValue_StoresValue(object value)
        {
            // Arrange & Act
            var flex = new FlexObject(value);
            
            // Assert
            flex.FlexData.Should().Be(value);
            flex.ToString().Should().Be(value.ToString());
        }
        
        [Fact]
        public void FlexData_SetGet_WorksCorrectly()
        {
            // Arrange
            var flex = new FlexObject();
            var testData = new { Name = "Test", Value = 123 };
            
            // Act
            flex.FlexData = testData;
            
            // Assert
            flex.FlexData.Should().BeSameAs(testData);
        }
    }
}
```

---

*This document should be reviewed and updated regularly as the LionFire.Flex project evolves.*
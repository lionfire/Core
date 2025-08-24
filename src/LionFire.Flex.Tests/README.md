# LionFire.Flex.Tests

## Overview
This test project provides comprehensive unit testing for the LionFire.Flex project, which implements a flexible object composition system with support for dynamic typing, metadata, and extensible object structures.

## Project Structure

```
LionFire.Flex.Tests/
├── Fixtures/
│   ├── TestObjects.cs          # Test data classes and constants
│   └── FlexTestHelpers.cs      # Test utility methods and assertions
├── Unit/
│   ├── Core/
│   │   ├── IFlexTests.cs       # Tests for IFlex interface
│   │   ├── FlexObjectTests.cs  # Tests for FlexObject class
│   │   ├── FlexTests.cs        # Tests for Flex static methods
│   │   └── SimpleFlexObjectTests.cs # Working basic tests (xUnit compatible)
│   ├── Extensions/
│   │   └── IFlexExtensionsTests.cs # Tests for extension methods
│   ├── Dictionaries/
│   │   └── FlexDictionaryTests.cs  # Tests for FlexDictionary
│   ├── Options/
│   │   ├── FlexOptionsTests.cs      # Tests for FlexOptions
│   │   ├── FlexGlobalOptionsTests.cs # Tests for FlexGlobalOptions
│   │   └── FlexMemberOptionsTests.cs # Tests for FlexMemberOptions
│   └── DefaultTypes/
│       ├── DefaultTypeFlexObjectTests.cs # Tests for DefaultTypeFlexObject
│       └── SingleTypeFlexObjectTests.cs  # Tests for SingleTypeFlexObject
├── Integration/
│   ├── ThreadSafetyTests.cs    # Concurrent operations tests
│   └── ComplexScenarioTests.cs # End-to-end integration tests
└── LionFire.Flex.Tests.csproj
```

## Test Coverage

The test suite covers all major components of the LionFire.Flex system:

### Core Components (✓ Working)
- **IFlex Interface**: Property get/set operations, null handling
- **FlexObject Class**: Constructors, ToString() implementation, property behavior  
- **Flex Class**: Static Create method (partial - reflection issues with some overloads)

### Extension Methods (⚠️ Needs FluentAssertions)
- **Metadata Operations**: Meta() extension, ConditionalWeakTable behavior
- **Type Operations**: SingleValueType(), IsSingleValue(), type wrapping
- **Query and Retrieval**: GetOrCreate<T>(), Query<T>(), thread-safe operations
- **Add/Set Operations**: Various Add overloads, SetExclusive, keyed operations

### Dictionary Support (⚠️ Needs FluentAssertions)
- **FlexDictionary<TKey>**: GetFlex(), QueryFlex(), Add() methods
- **Thread Safety**: Concurrent operations validation

### Options System (⚠️ Needs FluentAssertions)
- **FlexOptions**: SingleType property, IsSingleType behavior
- **FlexGlobalOptions**: Default factory patterns, customization
- **FlexMemberOptions**: AllowMultiple configuration

### Advanced Features (⚠️ Needs FluentAssertions)
- **DefaultTypeFlexObject**: Type constraints, inheritance behavior
- **SingleTypeFlexObject**: Single type enforcement, type validation
- **Thread Safety**: Concurrent access patterns, race condition prevention
- **Complex Scenarios**: Nested objects, metadata, real-world usage patterns

## Current Status

### Working Tests (18/18 passing)
The `SimpleFlexObjectTests.cs` file contains 18 basic tests that are currently working and use standard xUnit assertions:

- ✅ FlexObject construction and basic properties
- ✅ FlexData get/set operations
- ✅ Flex.Create with no components
- ✅ GetOrCreate basic functionality
- ✅ IsEmpty validation
- ✅ Meta() method basics
- ✅ SingleTypeFlexObject construction
- ✅ FlexDictionary basic operations
- ✅ FlexOptions basic properties

### Tests Requiring FluentAssertions
The remaining comprehensive test files use FluentAssertions syntax and would need to be either:
1. Added to the project's Directory.Packages.props file, or
2. Converted to use standard xUnit assertions

These include approximately 200+ additional test methods covering:
- Complex extension method scenarios
- Thread safety and concurrency
- Advanced type operations
- Integration scenarios
- Performance characteristics

## Test Framework Configuration

- **Framework**: xUnit 2.9.3
- **Test SDK**: Microsoft.NET.Test.Sdk 17.13.0
- **Coverage**: coverlet.collector
- **Target Framework**: .NET 9.0
- **Package Management**: Central package version management

## Running Tests

```bash
# Build the test project
dotnet build

# Run all tests
dotnet test

# Run tests with verbose output
dotnet test --verbosity normal

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Test Patterns and Conventions

### Naming Convention
Tests follow the pattern: `MethodName_StateUnderTest_ExpectedBehavior`

Example: `GetOrCreate_WithExistingValue_ReturnsExistingValue`

### Test Structure
All tests use the Arrange-Act-Assert pattern:

```csharp
[Fact]
public void Method_Scenario_ExpectedResult()
{
    // Arrange
    var testData = CreateTestData();
    
    // Act
    var result = systemUnderTest.Method(testData);
    
    // Assert
    Assert.Equal(expectedValue, result);
}
```

### Test Fixtures
- `TestObjects.cs`: Contains test data classes and constants
- `FlexTestHelpers.cs`: Provides utility methods for common assertions and test data creation

## Key Test Scenarios

### Basic Functionality
- Object creation and initialization
- Property get/set operations
- Type validation and conversion
- Null handling

### Advanced Operations  
- Dynamic type management
- Metadata handling
- Concurrent operations
- Factory patterns
- Complex object graphs

### Error Conditions
- Invalid type assignments
- Concurrent access violations
- Creation failures
- Type constraint violations

## Future Enhancements

1. **Add FluentAssertions**: Enable the comprehensive test suite
2. **Performance Benchmarks**: Add BenchmarkDotNet tests
3. **Property-based Testing**: Use FsCheck for edge case discovery
4. **Mutation Testing**: Verify test quality with Stryker.NET
5. **Integration with CI/CD**: Add automated test execution
6. **Code Coverage Reporting**: Generate and track coverage metrics

## Notes

- Some tests are currently disabled due to FluentAssertions dependency
- Reflection-based operations in Flex.Create may need adjustment for ambiguous method resolution
- Thread safety tests require careful timing considerations
- The codebase uses nullable reference types - tests should respect this

This test suite provides a solid foundation for validating the LionFire.Flex functionality and can be extended as the library evolves.
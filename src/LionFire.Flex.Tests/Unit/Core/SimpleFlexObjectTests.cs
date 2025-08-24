using LionFire.FlexObjects;
using LionFire.FlexObjects.Tests.Fixtures;
using Xunit;

namespace LionFire.FlexObjects.Tests.Unit.Core;

public class SimpleFlexObjectTests
{
    [Fact]
    public void Constructor_Default_CreatesEmptyFlex()
    {
        // Arrange & Act
        var flex = new FlexObject();

        // Assert
        Assert.Null(flex.FlexData);
        Assert.Equal("(null)", flex.ToString());
        FlexTestHelpers.AssertFlexEmpty(flex);
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
        Assert.Equal(value, flex.FlexData);
        Assert.Equal(value.ToString(), flex.ToString());
        FlexTestHelpers.AssertFlexNotEmpty(flex);
    }

    [Fact]
    public void Constructor_WithNull_StoresNull()
    {
        // Arrange & Act
        var flex = new FlexObject(null);

        // Assert
        Assert.Null(flex.FlexData);
        Assert.Equal("(null)", flex.ToString());
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
        Assert.Same(testData, flex.FlexData);
    }

    [Fact]
    public void FlexData_CanStoreComplexObjects()
    {
        // Arrange
        var flex = new FlexObject();
        var person = FlexTestHelpers.CreateDefaultPerson();

        // Act
        flex.FlexData = person;

        // Assert
        Assert.Same(person, flex.FlexData);
        var retrievedPerson = flex.FlexData as TestPerson;
        Assert.NotNull(retrievedPerson);
        Assert.Equal(TestConstants.DefaultPersonName, retrievedPerson.Name);
        Assert.Equal(TestConstants.DefaultPersonAge, retrievedPerson.Age);
    }

    [Fact]
    public void FlexCreate_WithNoComponents_CreatesEmptyFlex()
    {
        // Arrange & Act
        var flex = Flex.Create();

        // Assert
        Assert.NotNull(flex);
        Assert.IsType<Flex>(flex);
        FlexTestHelpers.AssertFlexEmpty(flex);
    }

    [Fact]
    public void GetOrCreate_WithExistingValue_ReturnsExistingValue()
    {
        // Arrange
        var flex = new FlexObject();
        var person = FlexTestHelpers.CreateDefaultPerson();
        flex.Add(person);

        // Act
        var result = flex.GetOrCreate<TestPerson>();

        // Assert
        Assert.Equal(person.Name, result.Name);
        Assert.Equal(person.Age, result.Age);
    }

    [Fact]
    public void GetOrCreate_WithNoExistingValue_CreatesNewValue()
    {
        // Arrange
        var flex = new FlexObject();

        // Act
        var result = flex.GetOrCreate<TestPerson>();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<TestPerson>(result);
    }

    [Fact]
    public void Meta_WithStandardFlex_CreatesMetadata()
    {
        // Arrange
        var flex = new FlexObject();

        // Act
        var meta = flex.Meta();

        // Assert
        Assert.NotNull(meta);
        Assert.IsAssignableFrom<IFlex>(meta);
    }

    [Fact]
    public void IsEmpty_WithNullFlexData_ReturnsTrue()
    {
        // Arrange
        var flex = new FlexObject();

        // Act & Assert
        Assert.True(flex.IsEmpty());
    }

    [Fact]
    public void IsEmpty_WithNonNullFlexData_ReturnsFalse()
    {
        // Arrange
        var flex = new FlexObject("data");

        // Act & Assert
        Assert.False(flex.IsEmpty());
    }

    [Fact]
    public void SingleTypeFlexObject_Constructor_PropertiesSetCorrectly()
    {
        // Arrange & Act
        var flex = new SingleTypeFlexObject<TestPerson>();

        // Assert
        Assert.Equal(typeof(TestPerson), flex.DefaultType);
        Assert.Null(flex.PrimaryValue);
        Assert.Null(flex.FlexData);
    }

    [Fact]
    public void SingleTypeFlexObject_WithValue_InitializesCorrectly()
    {
        // Arrange
        var person = FlexTestHelpers.CreateDefaultPerson();

        // Act
        var flex = new SingleTypeFlexObject<TestPerson>(person);

        // Assert
        Assert.Same(person, flex.PrimaryValue);
        Assert.Same(person, flex.FlexData);
    }

    [Fact]
    public void FlexDictionary_GetFlex_WithNewKey_CreatesNewFlexObject()
    {
        // Arrange
        var flexDict = new FlexDictionary<string>();
        var key = "testKey";

        // Act
        var flex = flexDict.GetFlex(key);

        // Assert
        Assert.NotNull(flex);
        Assert.IsType<FlexObject>(flex);
        FlexTestHelpers.AssertFlexEmpty(flex);
        Assert.True(flexDict.Values.ContainsKey(key));
        Assert.Same(flex, flexDict.Values[key]);
    }

    [Fact]
    public void FlexOptions_DefaultValues_PropertiesSetCorrectly()
    {
        // Arrange & Act
        var options = new FlexOptions();

        // Assert
        Assert.Null(options.SingleType);
        Assert.False(options.IsSingleType);
    }

    [Fact]
    public void FlexOptions_SingleType_SetToType_UpdatesIsSingleType()
    {
        // Arrange
        var options = new FlexOptions();

        // Act
        options.SingleType = typeof(TestPerson);

        // Assert
        Assert.Equal(typeof(TestPerson), options.SingleType);
        Assert.True(options.IsSingleType);
    }
}
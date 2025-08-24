using LionFire.FlexObjects;
using Xunit;

namespace LionFire.FlexObjects.Tests.Fixtures;

public static class FlexTestHelpers
{
    /// <summary>
    /// Creates a test Flex object with the specified components
    /// </summary>
    public static IFlex CreateTestFlex(params object[] components)
    {
        return Flex.Create(components);
    }

    /// <summary>
    /// Creates a FlexObject with the specified value
    /// </summary>
    public static FlexObject CreateFlexObject(object? value = null)
    {
        return value == null ? new FlexObject() : new FlexObject(value);
    }

    /// <summary>
    /// Creates a test person object with default values
    /// </summary>
    public static TestPerson CreateDefaultPerson()
    {
        return new TestPerson
        {
            Name = TestConstants.DefaultPersonName,
            Age = TestConstants.DefaultPersonAge,
            Address = new TestAddress
            {
                Street = TestConstants.DefaultStreetAddress,
                City = TestConstants.DefaultCity,
                ZipCode = TestConstants.DefaultZipCode
            }
        };
    }

    /// <summary>
    /// Creates a test product with default values
    /// </summary>
    public static TestProduct CreateDefaultProduct()
    {
        return new TestProduct
        {
            Name = TestConstants.DefaultProductName,
            Price = TestConstants.DefaultProductPrice,
            InStock = true
        };
    }

    /// <summary>
    /// Asserts that a Flex contains the expected value of type T
    /// </summary>
    public static void AssertFlexContains<T>(IFlex flex, T expectedValue)
    {
        var actualValue = flex.GetOrCreate<T>();
        Assert.Equal(expectedValue, actualValue);
    }

    /// <summary>
    /// Asserts that a Flex is empty (FlexData is null)
    /// </summary>
    public static void AssertFlexEmpty(IFlex flex)
    {
        Assert.Null(flex.FlexData);
        Assert.True(flex.IsEmpty());
    }

    /// <summary>
    /// Asserts that a Flex contains data (FlexData is not null)
    /// </summary>
    public static void AssertFlexNotEmpty(IFlex flex)
    {
        Assert.NotNull(flex.FlexData);
        Assert.False(flex.IsEmpty());
    }

    /// <summary>
    /// Asserts that a Flex has a specific single value type
    /// </summary>
    public static void AssertSingleValueType<T>(IFlex flex)
    {
        Assert.Equal(typeof(T), flex.SingleValueType());
        Assert.True(flex.IsSingleValue());
    }

    /// <summary>
    /// Asserts that a Flex has metadata
    /// </summary>
    public static void AssertHasMetadata(IFlex flex)
    {
        var meta = flex.Meta();
        Assert.NotNull(meta);
    }

    /// <summary>
    /// Creates a collection of test objects for bulk testing
    /// </summary>
    public static IEnumerable<object[]> GetTestObjects()
    {
        yield return new object[] { "string value", typeof(string) };
        yield return new object[] { 42, typeof(int) };
        yield return new object[] { 3.14, typeof(double) };
        yield return new object[] { true, typeof(bool) };
        yield return new object[] { DateTime.Now, typeof(DateTime) };
        yield return new object[] { CreateDefaultPerson(), typeof(TestPerson) };
        yield return new object[] { CreateDefaultProduct(), typeof(TestProduct) };
    }

    /// <summary>
    /// Creates test objects of various types for parameterized tests
    /// </summary>
    public static IEnumerable<object[]> GetVariousTypeObjects()
    {
        yield return new object[] { "test" };
        yield return new object[] { 123 };
        yield return new object[] { 45.67m };
        yield return new object[] { new TestPerson { Name = "Alice", Age = 25 } };
        yield return new object[] { new TestRecord("RecordValue", 999) };
        yield return new object[] { new TestStruct { Value = 100, Text = "StructText" } };
    }
}
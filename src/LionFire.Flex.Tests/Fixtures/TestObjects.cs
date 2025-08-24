using LionFire.FlexObjects;

namespace LionFire.FlexObjects.Tests.Fixtures;

public class TestPerson
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public TestAddress? Address { get; set; }
}

public class TestAddress
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
}

public class TestProduct
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool InStock { get; set; }
}

public interface ITestService
{
    void DoSomething();
    string GetMessage();
}

public class TestService : ITestService
{
    public void DoSomething()
    {
        // Test implementation
    }

    public string GetMessage() => "Test message";
}

public class TestFlexObject : IFlex
{
    public object? FlexData { get; set; }
}

public class TestFlexWithMeta : IFlexWithMeta
{
    public object? FlexData { get; set; }
    public IFlex Meta { get; set; } = new FlexObject();
}

public record TestRecord(string Value, int Number);

public struct TestStruct
{
    public int Value { get; set; }
    public string Text { get; set; }
}

public static class TestConstants
{
    public const string DefaultPersonName = "John Doe";
    public const int DefaultPersonAge = 30;
    public const string DefaultStreetAddress = "123 Main St";
    public const string DefaultCity = "Anytown";
    public const string DefaultZipCode = "12345";
    public const string DefaultProductName = "Test Product";
    public const decimal DefaultProductPrice = 29.99m;
}
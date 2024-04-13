namespace MotorcycleRent.UnitTests.Core.Shared;

public sealed class MethodExtensionsTests
{
    public class TestClass
    {
        public string? Name { get; set; }
        public int Age { get; set; }
    }

    [Fact]
    public void AsJson_SerializesObject_Correctly()
    {
        var testObject = new TestClass { Name = "John", Age = 30 };
        var jsonResult = testObject.AsJson();

        var expectedJson = "{\r\n  \"Name\": \"John\",\r\n  \"Age\": 30\r\n}";
        Assert.Equal(expectedJson, jsonResult);
    }

    [Fact]
    public void AsJson_UsesDefaultSerializerOptions_WhenNoOptionsProvided()
    {
        var testObject = new TestClass { Name = "Jane", Age = 25 };
        var jsonResult = testObject.AsJson();

        // Check if the output is indented as per the default settings
        Assert.Contains("  \"Name\": \"Jane\"", jsonResult); // Checking for indentation
        Assert.Contains("  \"Age\": 25", jsonResult); // Checking for indentation
    }

    [Fact]
    public void AsJson_UsesProvidedSerializerOptions_WhenOptionsProvided()
    {
        var options = new JsonSerializerOptions { WriteIndented = false };
        var testObject = new TestClass { Name = "Dave", Age = 40 };
        var jsonResult = testObject.AsJson(options);

        var expectedJson = "{\"Name\":\"Dave\",\"Age\":40}";
        Assert.Equal(expectedJson, jsonResult);
    }
}
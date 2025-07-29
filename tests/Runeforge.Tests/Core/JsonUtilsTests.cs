using NUnit.Framework;
using Runeforge.Core.Json;
using System.Text.Json;

namespace Runeforge.Tests.Core;

/// <summary>
/// Test model for JSON serialization tests
/// </summary>
public class TestModel
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public bool IsActive { get; set; }
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// Test model with enum for JSON serialization tests
/// </summary>
public class TestModelWithEnum
{
    public string Name { get; set; } = string.Empty;
    public TestEnum Status { get; set; }
}

public enum TestEnum
{
    Active,
    Inactive,
    Pending
}

[TestFixture]
public class JsonUtilsTests
{
    private readonly string _testDirectory = Path.Combine(Path.GetTempPath(), "runeforge_tests");

    [SetUp]
    public void Setup()
    {
        JsonUtils.RegisterJsonContext(TestJsonContext.Default);
        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
    }

    [Test]
    public void Serialize_WithValidObject_ShouldReturnJsonString()
    {
        // Arrange
        var testModel = new TestModel
        {
            Name = "Test User",
            Age = 25,
            IsActive = true,
            Tags = ["tag1", "tag2"]
        };

        // Act
        var json = JsonUtils.Serialize(testModel);

        // Assert
        Assert.That(json, Is.Not.Null);
        Assert.That(json, Does.Contain("Test User"));
        Assert.That(json, Does.Contain("25"));
        Assert.That(json, Does.Contain("true"));
        Assert.That(json, Does.Contain("tag1"));
    }

    [Test]
    public void Serialize_WithNullObject_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => JsonUtils.Serialize<TestModel>(null));
    }

    [Test]
    public void Deserialize_WithValidJson_ShouldReturnObject()
    {
        // Arrange
        var json = """
                   {
                       "name": "Test User",
                       "age": 30,
                       "isActive": false,
                       "tags": ["work", "personal"]
                   }
                   """;

        // Act
        var result = JsonUtils.Deserialize<TestModel>(json);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Test User"));
        Assert.That(result.Age, Is.EqualTo(30));
        Assert.That(result.IsActive, Is.False);
        Assert.That(result.Tags, Has.Count.EqualTo(2));
        Assert.That(result.Tags, Contains.Item("work"));
        Assert.That(result.Tags, Contains.Item("personal"));
    }

    [Test]
    public void Deserialize_WithNullJson_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => JsonUtils.Deserialize<TestModel>(null));
    }

    [Test]
    public void Deserialize_WithInvalidJson_ShouldThrowJsonException()
    {
        // Arrange
        var invalidJson = "{ invalid json }";

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonUtils.Deserialize<TestModel>(invalidJson));
    }

    [Test]
    public void SerializeToFile_WithValidObject_ShouldCreateFile()
    {
        // Arrange
        var testModel = new TestModel
        {
            Name = "File Test",
            Age = 35,
            IsActive = true,
            Tags = ["file", "test"]
        };
        var filePath = Path.Combine(_testDirectory, "test_serialize.json");

        // Act
        JsonUtils.SerializeToFile(testModel, filePath);

        // Assert
        Assert.That(File.Exists(filePath), Is.True);

        var fileContent = File.ReadAllText(filePath);
        Assert.That(fileContent, Does.Contain("File Test"));
        Assert.That(fileContent, Does.Contain("35"));
    }

    [Test]
    public void DeserializeFromFile_WithValidFile_ShouldReturnObject()
    {
        // Arrange
        var testModel = new TestModel
        {
            Name = "File Deserialize Test",
            Age = 40,
            IsActive = false,
            Tags = ["deserialize", "file"]
        };
        var filePath = Path.Combine(_testDirectory, "test_deserialize.json");

        // First serialize to create the file
        JsonUtils.SerializeToFile(testModel, filePath);

        // Act
        var result = JsonUtils.DeserializeFromFile<TestModel>(filePath);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("File Deserialize Test"));
        Assert.That(result.Age, Is.EqualTo(40));
        Assert.That(result.IsActive, Is.False);
        Assert.That(result.Tags, Has.Count.EqualTo(2));
    }

    [Test]
    public void DeserializeFromFile_WithNonExistentFile_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var nonExistentPath = Path.Combine(_testDirectory, "nonexistent.json");

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => JsonUtils.DeserializeFromFile<TestModel>(nonExistentPath));
    }

    [Test]
    public void RoundTrip_SerializeAndDeserialize_ShouldPreserveData()
    {
        // Arrange
        var original = new TestModel
        {
            Name = "Round Trip Test",
            Age = 45,
            IsActive = true,
            Tags = ["round", "trip", "test"]
        };

        // Act
        var json = JsonUtils.Serialize(original);
        var result = JsonUtils.Deserialize<TestModel>(json);

        // Assert
        Assert.That(result.Name, Is.EqualTo(original.Name));
        Assert.That(result.Age, Is.EqualTo(original.Age));
        Assert.That(result.IsActive, Is.EqualTo(original.IsActive));
        Assert.That(result.Tags, Is.EqualTo(original.Tags));
    }

    [Test]
    public void Serialize_WithEnum_ShouldSerializeAsCamelCase()
    {
        // Arrange
        var testModel = new TestModelWithEnum
        {
            Name = "Enum Test",
            Status = TestEnum.Active
        };

        // Act
        var json = JsonUtils.Serialize(testModel);

        // Assert
        Assert.That(json, Does.Contain("\"status\": \"active\""));
        Assert.That(json, Does.Not.Contain("Active")); // Should be camelCase
    }

    [Test]
    public void Deserialize_WithCamelCaseEnum_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = """
                   {
                       "name": "Enum Test",
                       "status": "pending"
                   }
                   """;

        // Act
        var result = JsonUtils.Deserialize<TestModelWithEnum>(json);

        // Assert
        Assert.That(result.Name, Is.EqualTo("Enum Test"));
        Assert.That(result.Status, Is.EqualTo(TestEnum.Pending));
    }
}

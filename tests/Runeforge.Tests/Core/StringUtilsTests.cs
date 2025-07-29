using NUnit.Framework;
using Runeforge.Core.Utils;

namespace Runeforge.Tests.Core;

[TestFixture]
public class StringUtilsTests
{
    [Test]
    [TestCase("HelloWorld", "hello_world")]
    [TestCase("APIResponse", "api_response")]
    [TestCase("userId", "user_id")]
    [TestCase("HTML", "html")]
    [TestCase("A", "a")]
    public void ToSnakeCase_WithVariousInputs_ShouldReturnCorrectSnakeCase(string input, string expected)
    {
        // Act
        var result = StringUtils.ToSnakeCase(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ToSnakeCase_WithNullInput_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToSnakeCase(null));
    }

    [Test]
    [TestCase("hello_world", "helloWorld")]
    [TestCase("api_response", "apiResponse")]
    [TestCase("user-id", "userId")]
    [TestCase("HELLO WORLD", "helloWorld")]
    [TestCase("A", "a")]
    public void ToCamelCase_WithVariousInputs_ShouldReturnCorrectCamelCase(string input, string expected)
    {
        // Act
        var result = StringUtils.ToCamelCase(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ToCamelCase_WithNullInput_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToCamelCase(null));
    }

    [Test]
    [TestCase("hello_world", "HelloWorld")]
    [TestCase("api-response", "ApiResponse")]
    [TestCase("userId", "UserId")]
    [TestCase("HELLO WORLD", "HelloWorld")]
    [TestCase("A", "A")]
    public void ToPascalCase_WithVariousInputs_ShouldReturnCorrectPascalCase(string input, string expected)
    {
        // Act
        var result = StringUtils.ToPascalCase(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ToPascalCase_WithNullInput_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToPascalCase(null));
    }

    [Test]
    [TestCase("HelloWorld", "hello-world")]
    [TestCase("API_RESPONSE", "api-response")]
    [TestCase("userId", "user-id")]
    [TestCase("HELLO WORLD", "hello-world")]
    [TestCase("A", "a")]
    public void ToKebabCase_WithVariousInputs_ShouldReturnCorrectKebabCase(string input, string expected)
    {
        // Act
        var result = StringUtils.ToKebabCase(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ToKebabCase_WithNullInput_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToKebabCase(null));
    }

    [Test]
    [TestCase("HelloWorld", "HELLO_WORLD")]
    [TestCase("apiResponse", "API_RESPONSE")]
    [TestCase("user-id", "USER_ID")]
    [TestCase("A", "A")]
    public void ToUpperSnakeCase_WithVariousInputs_ShouldReturnCorrectUpperSnakeCase(string input, string expected)
    {
        // Act
        var result = StringUtils.ToUpperSnakeCase(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ToUpperSnakeCase_WithNullInput_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToUpperSnakeCase(null));
    }

    [Test]
    [TestCase("hello_world", "Hello World")]
    [TestCase("API_RESPONSE", "Api Response")]
    [TestCase("user-id", "User Id")]
    [TestCase("HELLO WORLD", "Hello World")]
    [TestCase("A", "A")]
    public void ToTitleCase_WithVariousInputs_ShouldReturnCorrectTitleCase(string input, string expected)
    {
        // Act
        var result = StringUtils.ToTitleCase(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ToTitleCase_WithNullInput_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToTitleCase(null));
    }

    [Test]
    public void CaseConversions_RoundTrip_ShouldWorkCorrectly()
    {
        // Arrange
        var original = "UserAccountSettings";

        // Act - Convert through different cases
        var snakeCase = StringUtils.ToSnakeCase(original);
        var camelCase = StringUtils.ToCamelCase(snakeCase);
        var pascalCase = StringUtils.ToPascalCase(camelCase);
        var kebabCase = StringUtils.ToKebabCase(pascalCase);
        var upperSnake = StringUtils.ToUpperSnakeCase(kebabCase);
        var titleCase = StringUtils.ToTitleCase(upperSnake);

        // Assert - Verify intermediate results
        Assert.That(snakeCase, Is.EqualTo("user_account_settings"));
        Assert.That(camelCase, Is.EqualTo("userAccountSettings"));
        Assert.That(pascalCase, Is.EqualTo("UserAccountSettings"));
        Assert.That(kebabCase, Is.EqualTo("user-account-settings"));
        Assert.That(upperSnake, Is.EqualTo("USER_ACCOUNT_SETTINGS"));
        Assert.That(titleCase, Is.EqualTo("User Account Settings"));
    }

    [Test]
    public void ToSnakeCase_WithComplexAcronyms_ShouldHandleCorrectly()
    {
        // Arrange & Act & Assert
        Assert.That(StringUtils.ToSnakeCase("HTTPSConnection"), Is.EqualTo("https_connection"));
        Assert.That(StringUtils.ToSnakeCase("XMLParser"), Is.EqualTo("xml_parser"));
        Assert.That(StringUtils.ToSnakeCase("JSONData"), Is.EqualTo("json_data"));
        Assert.That(StringUtils.ToSnakeCase("URLPath"), Is.EqualTo("url_path"));
    }

    [Test]
    public void ToSnakeCase_WithNumbersAndSpecialCases_ShouldHandleCorrectly()
    {
        // Arrange & Act & Assert
        Assert.That(StringUtils.ToSnakeCase("Item2D"), Is.EqualTo("item2_d"));
        Assert.That(StringUtils.ToSnakeCase("Vector3D"), Is.EqualTo("vector3_d"));
    }
}

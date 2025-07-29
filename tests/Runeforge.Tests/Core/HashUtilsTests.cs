using Runeforge.Core.Utils;

namespace Runeforge.Tests.Core;

[TestFixture]
public class HashUtilsTests
{
    [Test]
    public void ComputeSha256Hash_WithValidString_ShouldReturnConsistentHash()
    {
        // Arrange
        var input = "test string";

        // Act
        var hash1 = HashUtils.ComputeSha256Hash(input);
        var hash2 = HashUtils.ComputeSha256Hash(input);

        // Assert
        Assert.That(hash1, Is.Not.Null);
        Assert.That(hash1, Is.Not.Empty);
        Assert.That(hash1, Is.EqualTo(hash2));
        Assert.That(hash1.Length, Is.EqualTo(64));
        Assert.That(hash1, Does.Match("^[a-f0-9]{64}$"));
    }

    [Test]
    public void ComputeSha256Hash_WithDifferentStrings_ShouldReturnDifferentHashes()
    {
        // Arrange
        var input1 = "string1";
        var input2 = "string2";

        // Act
        var hash1 = HashUtils.ComputeSha256Hash(input1);
        var hash2 = HashUtils.ComputeSha256Hash(input2);

        // Assert
        Assert.That(hash1, Is.Not.EqualTo(hash2));
    }

    [Test]
    public void ComputeSha256Hash_WithEmptyString_ShouldReturnValidHash()
    {
        // Arrange
        var input = string.Empty;

        // Act
        var hash = HashUtils.ComputeSha256Hash(input);

        // Assert
        Assert.That(hash, Is.Not.Null);
        Assert.That(hash, Is.Not.Empty);
        Assert.That(hash.Length, Is.EqualTo(64));
        Assert.That(hash, Is.EqualTo("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"));
    }



    [Test]
    public void HashPassword_WithValidPassword_ShouldReturnHashAndSalt()
    {
        // Arrange
        var password = "mySecretPassword123";

        // Act
        var (hash, salt) = HashUtils.HashPassword(password);

        // Assert
        Assert.That(hash, Is.Not.Null);
        Assert.That(hash, Is.Not.Empty);
        Assert.That(salt, Is.Not.Null);
        Assert.That(salt, Is.Not.Empty);

        // Hash and salt should be base64 encoded
        Assert.DoesNotThrow(() => Convert.FromBase64String(hash));
        Assert.DoesNotThrow(() => Convert.FromBase64String(salt));
    }

    [Test]
    public void HashPassword_WithSamePassword_ShouldReturnDifferentSalts()
    {
        // Arrange
        var password = "samePassword";

        // Act
        var (hash1, salt1) = HashUtils.HashPassword(password);
        var (hash2, salt2) = HashUtils.HashPassword(password);

        // Assert
        Assert.That(salt1, Is.Not.EqualTo(salt2)); // Salts should be different
        Assert.That(hash1, Is.Not.EqualTo(hash2)); // Hashes should be different due to different salts
    }

    [Test]
    public void CheckPasswordHash_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "testPassword123";
        var (hash, salt) = HashUtils.HashPassword(password);

        // Act
        var isValid = HashUtils.CheckPasswordHash(password, hash, salt);

        // Assert
        Assert.That(isValid, Is.True);
    }

    [Test]
    public void CheckPasswordHash_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var correctPassword = "correctPassword";
        var incorrectPassword = "wrongPassword";
        var (hash, salt) = HashUtils.HashPassword(correctPassword);

        // Act
        var isValid = HashUtils.CheckPasswordHash(incorrectPassword, hash, salt);

        // Assert
        Assert.That(isValid, Is.False);
    }

    [Test]
    public void CreatePassword_WithValidPassword_ShouldReturnFormattedString()
    {
        // Arrange
        var password = "myPassword";

        // Act
        var result = HashUtils.CreatePassword(password);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Does.Contain(":")); // Should contain separator

        var parts = result.Split(':');
        Assert.That(parts, Has.Length.EqualTo(2));

        // Both parts should be valid base64
        Assert.DoesNotThrow(() => Convert.FromBase64String(parts[0]));
        Assert.DoesNotThrow(() => Convert.FromBase64String(parts[1]));
    }

    [Test]
    public void VerifyPassword_WithHashPrefix_ShouldHandleCorrectly()
    {
        // Arrange
        var password = "testPassword";
        var hashSaltCombined = HashUtils.CreatePassword(password);
        var withPrefix = "hash://" + hashSaltCombined;

        // Act
        var isValid = HashUtils.VerifyPassword(password, withPrefix);

        // Assert
        Assert.That(isValid, Is.True);
    }

    [Test]
    public void VerifyPassword_WithInvalidFormat_ShouldThrowFormatException()
    {
        // Arrange
        var password = "testPassword";
        var invalidFormat = "invalidformat";

        // Act & Assert
        Assert.Throws<FormatException>(() => HashUtils.VerifyPassword(password, invalidFormat));
    }

    [Test]
    public void GenerateRandomRefreshToken_ShouldReturnValidBase64Token()
    {
        // Arrange & Act
        var token = HashUtils.GenerateRandomRefreshToken();

        // Assert
        Assert.That(token, Is.Not.Null);
        Assert.That(token, Is.Not.Empty);
        Assert.DoesNotThrow(() => Convert.FromBase64String(token));
    }

    [Test]
    public void GenerateRandomRefreshToken_WithCustomSize_ShouldReturnCorrectSize()
    {
        // Arrange
        var size = 16; // 16 bytes = 24 base64 characters (including padding)

        // Act
        var token = HashUtils.GenerateRandomRefreshToken(size);
        var bytes = Convert.FromBase64String(token);

        // Assert
        Assert.That(bytes.Length, Is.EqualTo(size));
    }

    [Test]
    public void GenerateRandomRefreshToken_MultipleCalls_ShouldReturnDifferentTokens()
    {
        // Arrange & Act
        var token1 = HashUtils.GenerateRandomRefreshToken();
        var token2 = HashUtils.GenerateRandomRefreshToken();

        // Assert
        Assert.That(token1, Is.Not.EqualTo(token2));
    }

    [Test]
    public void GenerateBase64Key_ShouldReturnValidKey()
    {
        // Arrange & Act
        var key = HashUtils.GenerateBase64Key();

        // Assert
        Assert.That(key, Is.Not.Null);
        Assert.That(key, Is.Not.Empty);
        Assert.DoesNotThrow(() => Convert.FromBase64String(key));

        var bytes = Convert.FromBase64String(key);
        Assert.That(bytes.Length, Is.EqualTo(32)); // Default 32 bytes
    }

    [Test]
    public void GenerateBase64Key_WithCustomLength_ShouldReturnCorrectLength()
    {
        // Arrange
        var length = 16;

        // Act
        var key = HashUtils.GenerateBase64Key(length);
        var bytes = Convert.FromBase64String(key);

        // Assert
        Assert.That(bytes.Length, Is.EqualTo(length));
    }

    [Test]
    public void Encrypt_AndDecrypt_ShouldPreserveOriginalText()
    {
        // Arrange
        var plaintext = "This is a secret message that needs to be encrypted!";
        var key = Convert.FromBase64String(HashUtils.GenerateBase64Key());

        // Act
        var encrypted = HashUtils.Encrypt(plaintext, key);
        var decrypted = HashUtils.Decrypt(encrypted, key);

        // Assert
        Assert.That(decrypted, Is.EqualTo(plaintext));
    }

    [Test]
    public void Encrypt_WithSameInputAndKey_ShouldReturnDifferentCiphertext()
    {
        // Arrange
        var plaintext = "Same message";
        var key = Convert.FromBase64String(HashUtils.GenerateBase64Key());

        // Act
        var encrypted1 = HashUtils.Encrypt(plaintext, key);
        var encrypted2 = HashUtils.Encrypt(plaintext, key);

        // Assert
        Assert.That(encrypted1, Is.Not.EqualTo(encrypted2)); // Different due to random IV
    }

    [Test]
    public void Encrypt_WithEmptyString_ShouldWorkCorrectly()
    {
        // Arrange
        var plaintext = string.Empty;
        var key = Convert.FromBase64String(HashUtils.GenerateBase64Key());

        // Act
        var encrypted = HashUtils.Encrypt(plaintext, key);
        var decrypted = HashUtils.Decrypt(encrypted, key);

        // Assert
        Assert.That(decrypted, Is.EqualTo(plaintext));
    }

    [Test]
    public void PasswordWorkflow_EndToEnd_ShouldWorkCorrectly()
    {
        // Arrange
        var originalPassword = "MySecurePassword123!";

        // Act - Create password hash
        var passwordHash = HashUtils.CreatePassword(originalPassword);

        // Verify correct password
        var isCorrectValid = HashUtils.VerifyPassword(originalPassword, passwordHash);

        // Verify incorrect password
        var isIncorrectValid = HashUtils.VerifyPassword("WrongPassword", passwordHash);

        // Assert
        Assert.That(isCorrectValid, Is.True);
        Assert.That(isIncorrectValid, Is.False);
        Assert.That(passwordHash, Does.Contain(":"));
    }
}

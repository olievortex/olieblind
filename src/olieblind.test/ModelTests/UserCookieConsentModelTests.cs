using olieblind.lib.Models;

namespace olieblind.test.ModelTests;

public class UserCookieConsentModelTests
{
    [Test]
    public void Constructor_Model_Inline()
    {
        // Arrange
        var value = Guid.NewGuid().ToString();

        // Act
        var result = new UserCookieConsentModel() { CookieValue = value };

        // Assert
        Assert.That(result.CookieValue, Is.EqualTo(value));
    }
}

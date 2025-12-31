using olieblind.lib.CookieConsent;

namespace olieblind.test.CookieConsentTests;

public class CookieConsentModelTests
{
    [Test]
    public void ReturnsFalse_NullValue()
    {
        // Arrange, Act, Assert
        Assert.That(CookieConsentModel.TryParse(null, out _), Is.False);
    }

    [Test]
    public void ReturnsFalse_NotFourParts()
    {
        // Arrange
        const string value = "hoist&Meow";

        // Act, Assert
        Assert.That(CookieConsentModel.TryParse(value, out _), Is.False);
    }

    [Test]
    public void ReturnsFalse_PartFourNotVersion()
    {
        // Arrange
        const string value = "hoist&Meow&cow&dog";

        // Act, Assert
        Assert.That(CookieConsentModel.TryParse(value, out _), Is.False);
    }

    [Test]
    public void ReturnsFalse_PartFourNotInteger()
    {
        // Arrange
        const string value = "hoist&Meow&cow&v=dog";

        // Act, Assert
        Assert.That(CookieConsentModel.TryParse(value, out _), Is.False);
    }

    [Test]
    public void ReturnsFalse_StatusNotInPartOne()
    {
        // Arrange
        const string value = "hoist&Meow&cow&v=42";

        // Act, Assert
        Assert.That(CookieConsentModel.TryParse(value, out _), Is.False);
    }

    [Test]
    public void ReturnsFalse_IdNotInPartTwo()
    {
        // Arrange
        const string value = "status=Essential&Meow&cow&v=42";

        // Act, Assert
        Assert.That(CookieConsentModel.TryParse(value, out _), Is.False);
    }

    [Test]
    public void ReturnsFalse_IdNotGuidInPartTwo()
    {
        // Arrange
        const string value = "status=Essential&id=Meow&cow&v=42";

        // Act, Assert
        Assert.That(CookieConsentModel.TryParse(value, out _), Is.False);
    }

    [Test]
    public void ReturnsTrue_Essential()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var value = $"status=Essential&id={guid}&cow&v=42";

        // Act
        var result = CookieConsentModel.TryParse(value, out var model);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(model.Id, Is.EqualTo(guid));
            Assert.That(model.Version, Is.EqualTo(42));
            Assert.That(model.Status, Is.EqualTo(CookieConsentStatusEnum.Essential));
        });
    }

    [Test]
    public void ReturnsTrue_All()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var value = $"status=All&id={guid}&cow&v=42";

        // Act
        var result = CookieConsentModel.TryParse(value, out var model);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(model.Id, Is.EqualTo(guid));
            Assert.That(model.Version, Is.EqualTo(42));
            Assert.That(model.Status, Is.EqualTo(CookieConsentStatusEnum.All));
        });
    }

    [Test]
    public void ReturnsTrue_Unknown()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var value = $"status=Peggy&id={guid}&cow&v=42";

        // Act
        var result = CookieConsentModel.TryParse(value, out var model);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(model.Id, Is.EqualTo(guid));
            Assert.That(model.Version, Is.EqualTo(42));
            Assert.That(model.Status, Is.EqualTo(CookieConsentStatusEnum.Unknown));
        });
    }

    [Test]
    public void ReturnsTrue_FullCookie()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var value = $"OlieCookieConsent=status=Essential&id={guid}&cow&v=42; Expires=SushiGood4u";

        // Act
        var result = CookieConsentModel.TryParse(value, out var model);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(model.Id, Is.EqualTo(guid));
            Assert.That(model.Version, Is.EqualTo(42));
            Assert.That(model.Status, Is.EqualTo(CookieConsentStatusEnum.Essential));
        });
    }
}
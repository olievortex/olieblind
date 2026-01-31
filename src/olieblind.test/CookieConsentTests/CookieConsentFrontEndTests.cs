using Moq;
using olieblind.lib.CookieConsent;
using olieblind.lib.Services;

namespace olieblind.test.CookieConsentTests;

public class CookieConsentFrontEndTests
{
    #region CreateBaseCookie

    [Test]
    public void CreateBaseCookie_CorrectStatus_ValidInput()
    {
        // Arrange
        var config = new Mock<IOlieConfig>();
        config.SetupGet(s => s.CookieConsentVersion).Returns(23);
        config.SetupGet(s => s.CookieConsentCookieName).Returns("OlieCookieConsent");
        var testable = new CookieConsentFrontEnd(config.Object);

        // Act
        var result = testable.CreateBaseCookie(CookieConsentStatusEnum.Essential);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Does.Contain("OlieCookieConsent=status=Essential&id="));
            Assert.That(result, Does.Contain("&v=23;"));
        }
    }

    #endregion

    #region GetBlueApiUrl

    [Test]
    public void GetBlueApiUrl_Value_Configured()
    {
        // Arrange
        var url = Guid.NewGuid().ToString();
        var config = new Mock<IOlieConfig>();
        config.SetupGet(s => s.BlueUrl).Returns(url);
        var testable = new CookieConsentFrontEnd(config.Object);

        // Act
        var result = testable.GetBlueApiUrl();

        // Assert
        Assert.That(result, Is.EqualTo(url));
    }

    #endregion

    #region GetCookieConsentStatus

    [Test]
    public void GetCookieConsentStatus_Unknown_NullCookieValue()
    {
        // Arrange
        var config = new Mock<IOlieConfig>();
        var testable = new CookieConsentFrontEnd(config.Object);

        // Act
        var result = testable.GetCookieConsentStatus(null!);

        // Assert
        Assert.That(result, Is.EqualTo(CookieConsentStatusEnum.Unknown));
    }

    [Test]
    public void GetCookieConsentStatu_Unknown_VersionTooOld()
    {
        // Arrange
        var config = new Mock<IOlieConfig>();
        config.SetupGet(s => s.CookieConsentVersion).Returns(48);
        var testable = new CookieConsentFrontEnd(config.Object);
        var value = $"status=Essential&id={Guid.NewGuid()}&cow&v=42";

        // Act
        var result = testable.GetCookieConsentStatus(value);

        // Assert
        Assert.That(result, Is.EqualTo(CookieConsentStatusEnum.Unknown));
    }

    [Test]
    public void ReturnsEssential_VersionGood()
    {
        // Arrange
        var config = new Mock<IOlieConfig>();
        config.SetupGet(s => s.CookieConsentVersion).Returns(48);
        var testable = new CookieConsentFrontEnd(config.Object);
        var value = $"status=Essential&id={Guid.NewGuid()}&cow&v=48";

        // Act
        var result = testable.GetCookieConsentStatus(value);

        // Assert
        Assert.That(result, Is.EqualTo(CookieConsentStatusEnum.Essential));
    }

    #endregion
}

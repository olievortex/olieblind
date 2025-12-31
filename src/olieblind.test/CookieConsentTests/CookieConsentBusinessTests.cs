using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.CookieConsent;

namespace olieblind.test.CookieConsentTests;

public class CookieConsentBusinessTests
{
    [Test]
    public async Task LogUserCookieConsentAsync_False_NullCookieValue()
    {
        // Arrange
        var repo = new Mock<IMyRepository>();
        var testable = new CookieConsentBusiness(repo.Object);

        // Act
        var result = await testable.LogUserCookieConsentAsync(null, null!, null!, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task LogUserCookieConsentAsync_False_InvalidCookieValue()
    {
        // Arrange
        var repo = new Mock<IMyRepository>();
        var testable = new CookieConsentBusiness(repo.Object);
        var cookieValue = Guid.NewGuid().ToString();

        // Act
        var result = await testable.LogUserCookieConsentAsync(cookieValue, null!, null!, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task LogUserCookieConsentAsync_True_ValidCookieValue()
    {
        // Arrange
        var entity = new UserCookieConsentLogEntity();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.UserCookieConsentLogCreate(It.IsAny<UserCookieConsentLogEntity>(),
                CancellationToken.None))
            .Callback((UserCookieConsentLogEntity e, CancellationToken _) => entity = e);
        var testable = new CookieConsentBusiness(repo.Object);
        var guid = Guid.NewGuid();
        var cookieValue = $"status=Essential&id={guid}&cow&v=42";
        var sourceIp = Guid.NewGuid().ToString();
        var userAgent = Guid.NewGuid().ToString();

        // Act
        var result = await testable.LogUserCookieConsentAsync(cookieValue, userAgent, sourceIp, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(entity.Id, Is.EqualTo(guid.ToString("N")));
            Assert.That(entity.Status, Is.EqualTo("Essential"));
            Assert.That(entity.Cookie, Is.EqualTo(cookieValue));
            Assert.That(entity.Timestamp, Is.Not.EqualTo(DateTime.MinValue));
            Assert.That(entity.SourceIp, Is.EqualTo(sourceIp));
            Assert.That(entity.UserAgent, Is.EqualTo(userAgent));
        }
    }
}
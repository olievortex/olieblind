using Azure.Messaging.ServiceBus.Administration;
using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite;
using olieblind.lib.Services;

namespace olieblind.test.SatelliteTests;

public class SatelliteRequestSourceTests
{
    #region CreateUserLog

    [Test]
    public async Task CreateUserLog_CreatesRecord_ValidData()
    {
        // Arrange
        var ct = CancellationToken.None;
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteRequestSource(repo.Object, null!, null!);
        var userId = "testUser";
        var effectiveDate = "20240101";
        var isFree = true;

        // Act
        await testable.CreateUserLog(userId, effectiveDate, isFree, ct);

        // Assert
        repo.Verify(v => v.UserSatelliteAdHocLogCreate(It.Is<UserSatelliteAdHocLogEntity>(e =>
            e.Id == userId &&
            e.EffectiveDate == effectiveDate &&
            e.IsFree == isFree &&
            e.Channel == 2 &&
            e.DayPart == DayPartsEnum.Afternoon &&
            e.Timestamp > DateTime.MinValue
        ), ct), Times.Once);
    }

    #endregion

    #region GetHourlyProductList

    [Test]
    public async Task GetHourlyProductList_ReturnsList_ValidData()
    {
        // Arrange
        var ct = CancellationToken.None;
        const string effectiveDate = "20240101";
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.SatelliteProductGetList(effectiveDate, ct))
            .ReturnsAsync(
            [
                new() { Id = "1", ScanTime = new DateTime(2024, 1, 1, 10, 0, 0), Path1080 = null, PathPoster = null },
                new() { Id = "2", ScanTime = new DateTime(2024, 1, 1, 10, 30, 0), Path1080 = "path", PathPoster = null },
                new() { Id = "3", ScanTime = new DateTime(2024, 1, 1, 11, 0, 0), Path1080 = null, PathPoster = null },
                new() { Id = "4", ScanTime = new DateTime(2024, 1, 1, 11, 15, 0), Path1080 = null, PathPoster = "path" },
                new() { Id = "5", ScanTime = new DateTime(2024, 1, 1, 12, 0, 0), Path1080 = "path", PathPoster = "path" }
            ]);
        var testable = new SatelliteRequestSource(repo.Object, null!, null!);

        // Act
        var result = await testable.GetHourlyProductList(effectiveDate, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo("1"));
            Assert.That(result[1].Id, Is.EqualTo("3"));
        }
    }

    #endregion

    #region GetRequestStatistics

    [Test]
    public async Task GetRequestStatistic_ReturnsModel_UserHasHistory()
    {
        // Arrange
        var ct = CancellationToken.None;
        var config = new Mock<IOlieConfig>();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.UserSatelliteAdHocLogUserStatistics(It.IsAny<int>(), ct))
            .ReturnsAsync(new Dictionary<string, int>
            {
                { "testUser", 3 },
                { "otherUser", 7 },
            });
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.ServiceBusQueueLength(It.IsAny<ServiceBusAdministrationClient>(), It.IsAny<string>(), ct))
            .ReturnsAsync(5);
        var testable = new SatelliteRequestSource(repo.Object, ows.Object, config.Object);

        // Act
        var result = await testable.GetRequestStatistics("testUser", null!, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.GlobalRequests, Is.EqualTo(10));
            Assert.That(result.UserRequests, Is.EqualTo(3));
            Assert.That(result.QueueLength, Is.EqualTo(5));
        }
    }

    [Test]
    public async Task GetRequestStatistic_ReturnsModel_NoUserHistory()
    {
        // Arrange
        var ct = CancellationToken.None;
        var config = new Mock<IOlieConfig>();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.UserSatelliteAdHocLogUserStatistics(It.IsAny<int>(), ct))
            .ReturnsAsync(new Dictionary<string, int>
            {
                { "otherUser2", 3 },
                { "otherUser", 7 },
            });
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.ServiceBusQueueLength(It.IsAny<ServiceBusAdministrationClient>(), It.IsAny<string>(), ct))
            .ReturnsAsync(5);
        var testable = new SatelliteRequestSource(repo.Object, ows.Object, config.Object);

        // Act
        var result = await testable.GetRequestStatistics("testUser", null!, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.GlobalRequests, Is.EqualTo(10));
            Assert.That(result.UserRequests, Is.Zero);
            Assert.That(result.QueueLength, Is.EqualTo(5));
        }
    }

    #endregion

    #region IsFreeDay

    [Test]
    public async Task IsFreeDay_ReturnsTrue_WhenSubstantialTornado()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int year = 2024;
        const string effectiveDate = "20240101";
        const string sourceFk = "source1";
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.StormEventsDailySummaryGet(effectiveDate, year, sourceFk, ct))
            .ReturnsAsync(new StormEventsDailySummaryEntity
            {
                F5 = 1,
                F4 = 2,
                F3 = 3,
                F2 = 4
            });
        var testable = new SatelliteRequestSource(repo.Object, null!, null!);

        // Act
        var result = await testable.IsFreeDay(effectiveDate, sourceFk, ct);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsFreeDay_ReturnsFalse_WhenNoSubstantialTornado()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int year = 2024;
        const string effectiveDate = "20240101";
        const string sourceFk = "source1";
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.StormEventsDailySummaryGet(effectiveDate, year, sourceFk, ct))
            .ReturnsAsync(new StormEventsDailySummaryEntity
            {
                F1 = 1,
                Hail = 2,
                Wind = 3
            });
        var testable = new SatelliteRequestSource(repo.Object, null!, null!);

        // Act
        var result = await testable.IsFreeDay(effectiveDate, sourceFk, ct);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsFreeDay_ReturnsFalse_NoRecord()
    {
        // Arrange
        var ct = CancellationToken.None;
        const string effectiveDate = "20240101";
        const string sourceFk = "source1";
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteRequestSource(repo.Object, null!, null!);

        // Act
        var result = await testable.IsFreeDay(effectiveDate, sourceFk, ct);

        // Assert
        Assert.That(result, Is.False);
    }

    #endregion

    #region IsQuotaAvailale

    [Test]
    public async Task IsQuotaAvailable_ReturnsFalse_QuotaTappedOut()
    {
        // Arrange
        const int lookbackHours = 24;
        var ct = CancellationToken.None;
        var config = new Mock<IOlieConfig>();
        config.SetupGet(g => g.SatelliteRequestLookbackHours).Returns(lookbackHours);
        config.SetupGet(g => g.SatelliteRequestUserLimit).Returns(1);
        config.SetupGet(g => g.SatelliteRequestGlobalLimit).Returns(3);
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.UserSatelliteAdHocLogUserStatistics(lookbackHours, ct))
            .ReturnsAsync(new Dictionary<string, int>()
            {
                { "anyUser", 3 },
                { "otherUser", 1 }
            });
        var testable = new SatelliteRequestSource(repo.Object, null!, config.Object);

        // Act
        var result = await testable.IsQuotaAvailable("anyUser", CancellationToken.None);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsQuotaAvailable_ReturnsTrue_QuotaAvailable()
    {
        // Arrange
        const int lookbackHours = 24;
        var ct = CancellationToken.None;
        var config = new Mock<IOlieConfig>();
        config.SetupGet(g => g.SatelliteRequestLookbackHours).Returns(lookbackHours);
        config.SetupGet(g => g.SatelliteRequestUserLimit).Returns(5);
        config.SetupGet(g => g.SatelliteRequestGlobalLimit).Returns(10);
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.UserSatelliteAdHocLogUserStatistics(lookbackHours, ct))
            .ReturnsAsync(new Dictionary<string, int>()
            {
                { "anyUser", 3 },
                { "otherUser", 1 }
            });
        var testable = new SatelliteRequestSource(repo.Object, null!, config.Object);

        // Act
        var result = await testable.IsQuotaAvailable("anyUser", CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
    }

    #endregion

    #region SendMessage

    [Test]
    public async Task SendMessage_EnqueuesMessages_ValidProducts()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var testable = new SatelliteRequestSource(null!, ows.Object, null!);
        var sender = Mock.Of<Azure.Messaging.ServiceBus.ServiceBusSender>();
        var products = new List<SatelliteProductEntity>
        {
            new() { Id = "prod1", EffectiveDate = "20240101" },
            new() { Id = "prod2", EffectiveDate = "20240101" }
        };

        // Act
        await testable.SendMessage(products, sender, ct);

        // Assert
        ows.Verify(v => v.ServiceBusSendJson(sender, It.IsAny<object>(), ct), Times.Exactly(products.Count));
    }

    #endregion
}

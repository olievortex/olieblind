using Azure.Messaging.ServiceBus.Administration;
using Moq;
using olieblind.data;
using olieblind.lib.Models;
using olieblind.lib.Satellite;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Services;

namespace olieblind.test.SatelliteTests;

public class SatelliteRequestProcessTests
{
    #region GetStatistics

    [Test]
    public async Task GetStatistic_ReturnsModel_UserHasHistory()
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
        var testable = new SatelliteRequestProcess(null!, ows.Object, config.Object, repo.Object);

        // Act
        var result = await testable.GetStatistics("testUser", null!, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.GlobalRequests, Is.EqualTo(10));
            Assert.That(result.UserRequests, Is.EqualTo(3));
            Assert.That(result.QueueLength, Is.EqualTo(5));
        }
    }

    [Test]
    public async Task GetStatistic_ReturnsModel_NoUserHistory()
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
        var testable = new SatelliteRequestProcess(null!, ows.Object, config.Object, repo.Object);

        // Act
        var result = await testable.GetStatistics("testUser", null!, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.GlobalRequests, Is.EqualTo(10));
            Assert.That(result.UserRequests, Is.Zero);
            Assert.That(result.QueueLength, Is.EqualTo(5));
        }
    }

    #endregion

    #region RequestHourlySatellite

    [Test]
    public async Task RequestHourlySatellite_ReturnsNothingToDo_WhenNoProducts()
    {
        // Arrange
        var ct = CancellationToken.None;
        var business = new Mock<ISatelliteRequestBusiness>();
        business.Setup(s => s.GetHourlyProductList(It.IsAny<string>(), ct))
            .ReturnsAsync([]);
        var testable = new SatelliteRequestProcess(business.Object, null!, null!, null!);
        var model = new SatelliteRequestModel
        {
            EffectiveDate = "20240101",
            SourceFk = "sourceFk",
            UserId = "testUser",
        };

        // Act
        var result = await testable.RequestHourlySatellite(model, null!, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccessful, Is.False);
            Assert.That(result.Message, Is.EqualTo(SatelliteRequestProcess.NothingToDoMessage));
        }
    }

    [Test]
    public async Task RequestHourlySatellite_ReturnsQuotaExceeded_QuotaExceeded()
    {
        // Arrange
        var ct = CancellationToken.None;
        var business = new Mock<ISatelliteRequestBusiness>();
        business.Setup(s => s.GetHourlyProductList(It.IsAny<string>(), ct))
            .ReturnsAsync([new()]);
        business.Setup(s => s.IsFreeRequest(It.IsAny<string>(), It.IsAny<string>(), ct))
            .ReturnsAsync(false);
        business.Setup(s => s.IsQuotaAvailable(It.IsAny<string>(), ct))
            .ReturnsAsync(false);
        var testable = new SatelliteRequestProcess(business.Object, null!, null!, null!);
        var model = new SatelliteRequestModel
        {
            EffectiveDate = "20240101",
            SourceFk = "sourceFk",
            UserId = "testUser",
        };

        // Act
        var result = await testable.RequestHourlySatellite(model, null!, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccessful, Is.False);
            Assert.That(result.Message, Is.EqualTo(SatelliteRequestProcess.QuotaExceededMessage));
        }
    }

    [Test]
    public async Task RequestHourlySatellite_ReturnsSuccess_QuotaAvailable()
    {
        // Arrange
        var ct = CancellationToken.None;
        var business = new Mock<ISatelliteRequestBusiness>();
        business.Setup(s => s.GetHourlyProductList(It.IsAny<string>(), ct))
            .ReturnsAsync([new()]);
        business.Setup(s => s.IsFreeRequest(It.IsAny<string>(), It.IsAny<string>(), ct))
            .ReturnsAsync(false);
        business.Setup(s => s.IsQuotaAvailable(It.IsAny<string>(), ct))
            .ReturnsAsync(true);
        var testable = new SatelliteRequestProcess(business.Object, null!, null!, null!);
        var model = new SatelliteRequestModel
        {
            EffectiveDate = "20240101",
            SourceFk = "sourceFk",
            UserId = "testUser",
        };

        // Act
        var result = await testable.RequestHourlySatellite(model, null!, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccessful, Is.True);
            Assert.That(result.Message, Is.EqualTo(SatelliteRequestProcess.SuccessMessage));
        }
    }

    #endregion
}

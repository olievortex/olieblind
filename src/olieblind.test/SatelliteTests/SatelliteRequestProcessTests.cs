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
    #region RequestHourlySatellite

    [Test]
    public async Task RequestHourlySatellite_ReturnsNothingToDo_WhenNoProducts()
    {
        // Arrange
        var ct = CancellationToken.None;
        var business = new Mock<ISatelliteRequestSource>();
        business.Setup(s => s.GetHourlyProductList(It.IsAny<string>(), ct))
            .ReturnsAsync([]);
        var testable = new SatelliteRequestProcess(business.Object);
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
        var business = new Mock<ISatelliteRequestSource>();
        business.Setup(s => s.GetHourlyProductList(It.IsAny<string>(), ct))
            .ReturnsAsync([new()]);
        business.Setup(s => s.IsFreeDay(It.IsAny<string>(), It.IsAny<string>(), ct))
            .ReturnsAsync(false);
        business.Setup(s => s.IsQuotaAvailable(It.IsAny<string>(), ct))
            .ReturnsAsync(false);
        var testable = new SatelliteRequestProcess(business.Object);
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
        var business = new Mock<ISatelliteRequestSource>();
        business.Setup(s => s.GetHourlyProductList(It.IsAny<string>(), ct))
            .ReturnsAsync([new()]);
        business.Setup(s => s.IsFreeDay(It.IsAny<string>(), It.IsAny<string>(), ct))
            .ReturnsAsync(false);
        business.Setup(s => s.IsQuotaAvailable(It.IsAny<string>(), ct))
            .ReturnsAsync(true);
        var testable = new SatelliteRequestProcess(business.Object);
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

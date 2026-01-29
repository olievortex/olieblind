using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite;
using olieblind.lib.Satellite.Interfaces;
using olieblind.test.SatelliteTests.SourcesTests;
using SixLabors.ImageSharp;

namespace olieblind.test.SatelliteTests;

public class SatelliteImageProcessTests
{
    #region CreateThumbnailAndUpdateDailySummary

    [Test]
    public async Task CreateThumbnailAndUpdateDailySummary_DoesNothing_AlreadyCreated()
    {
        // Arrange
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteImageProcess(null!, repo.Object);
        var ct = CancellationToken.None;
        var finalSize = new Point(128, 128);
        var summary = new StormEventsDailySummaryEntity
        {
            SatellitePath1080 = "a",
            SatellitePathPoster = "b"
        };

        // Act
        await testable.CreateThumbnailAndUpdateDailySummary(null!, summary, finalSize, null!, ct);

        // Assert
        repo.Verify(v => v.StormEventsDailySummaryUpdate(summary, ct), Times.Never);
    }

    [Test]
    public async Task CreateThumbnailAndUpdateDailySummary_DoesNothing_MarqueeMissing()
    {
        // Arrange
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteImageProcess(null!, repo.Object);
        var ct = CancellationToken.None;
        var finalSize = new Point(128, 128);
        var summary = new StormEventsDailySummaryEntity();

        // Act
        await testable.CreateThumbnailAndUpdateDailySummary(null!, summary, finalSize, null!, ct);

        // Assert
        repo.Verify(v => v.StormEventsDailySummaryUpdate(summary, ct), Times.Never);
    }

    [Test]
    public async Task CreateThumbnailAndUpdateDailySummary_CreatesPoster_NoPoster()
    {
        // Arrange
        var repo = new Mock<IMyRepository>();
        var business = new Mock<ISatelliteImageBusiness>();
        var testable = new SatelliteImageProcess(business.Object, repo.Object);
        var ct = CancellationToken.None;
        var finalSize = new Point(128, 128);
        var summary = new StormEventsDailySummaryEntity
        {
            SatellitePath1080 = "a"
        };
        var satellite = new SatelliteProductEntity();

        // Act
        await testable.CreateThumbnailAndUpdateDailySummary(satellite, summary, finalSize, null!, ct);

        // Assert
        business.Verify(v => v.MakeThumbnail(satellite, finalSize, null!, ct), Times.Once());
    }

    #endregion

    #region GetMarqueeSatelliteProduct

    [Test]
    public async Task GetMarqueeSatelliteProduct_Null_NoHeadlineEvent()
    {
        // Arrange
        var testable = new SatelliteImageProcess(null!, null!);
        var ct = CancellationToken.None;
        var summary = new StormEventsDailySummaryEntity();

        // Act
        var result = await testable.GetMarqueeProduct(summary, ct);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetMarqueeSatelliteProduct_Null_IsComplete()
    {
        // Arrange
        var testable = new SatelliteImageProcess(null!, null!);
        var ct = CancellationToken.None;
        var summary = new StormEventsDailySummaryEntity
        {
            HeadlineEventTime = new DateTime(2021, 7, 18),
            SatellitePathPoster = "a",
            SatellitePath1080 = "b"
        };

        // Act
        var result = await testable.GetMarqueeProduct(summary, ct);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetMarqueeSatelliteProduct_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        var business = new Mock<ISatelliteImageBusiness>();
        var testable = new SatelliteImageProcess(business.Object, null!);
        var ct = CancellationToken.None;
        var time = new DateTime(2021, 7, 18, 18, 0, 0);
        var summary = new StormEventsDailySummaryEntity
        {
            Id = "2021-07-18",
            HeadlineEventTime = time
        };
        var expected = new SatelliteProductEntity();
        business.Setup(s => s.GetMarqueeProduct("2021-07-18", time, ct))
            .ReturnsAsync(expected);

        // Act
        var result = await testable.GetMarqueeProduct(summary, ct);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion

    #region DownloadInventory

    [Test]
    public async Task DownloadInventory_ShortCircuit_NoKeys()
    {
        // Arrange
        var ct = CancellationToken.None;
        const string effectiveDate = "2021-07-21";
        const int satellite = 16;
        const int channel = 99;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var business = new Mock<ISatelliteImageBusiness>();
        var source = new SatelliteTestSource { Ows = null! };
        var testable = new SatelliteImageProcess(business.Object, null!);

        // Act
        await testable.DownloadInventory(effectiveDate, satellite, channel, dayPart, source, ct);

        // Assert
        business.Verify(v => v.AddInventoryToDatabase(It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<int>(), It.IsAny<DayPartsEnum>(), ct), Times.Never);
    }

    [Test]
    public async Task DownloadInventory_Finishes_HasKeys()
    {
        // Arrange
        var ct = CancellationToken.None;
        const string effectiveDate = "2021-07-21";
        const int satellite = 16;
        const int channel = 2;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var repo = new Mock<IMyRepository>();
        var business = new Mock<ISatelliteImageBusiness>();
        var source = new SatelliteTestSource { Ows = null! };
        var testable = new SatelliteImageProcess(business.Object, repo.Object);

        // Act
        await testable.DownloadInventory(effectiveDate, satellite, channel, dayPart, source, ct);

        // Assert
        business.Verify(v => v.AddInventoryToDatabase("2021-07-21", "", channel, dayPart, ct), Times.Once);
    }

    #endregion
}

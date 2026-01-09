using Moq;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Satellite.Models;
using olieblind.lib.StormEvents.Interfaces;

namespace olieblind.test.SatelliteTests;

public class SatelliteProcessTests
{
    //#region CreatePoster

    //[Test]
    //public async Task CreatePosterAsync_DoesNothing_PosterAlreadyCreated()
    //{
    //    // Arrange
    //    var awsBusiness = new Mock<ISatelliteAwsBusiness>();
    //    var summaryBusiness = new Mock<IDailySummaryBusiness>();
    //    var testable = new SatelliteProcess(null!, awsBusiness.Object, null!, summaryBusiness.Object);
    //    var ct = CancellationToken.None;
    //    var finalSize = new Point(128, 128);
    //    var summary = new StormEventsDailySummaryEntity
    //    {
    //        SatellitePath1080 = "a",
    //        SatellitePathPoster = "b"
    //    };

    //    // Act
    //    await testable.CreatePosterAsync(null!, summary, finalSize, null!, ct);

    //    // Assert
    //    summaryBusiness.Verify(v => v.UpdateCosmosAsync(summary, ct), Times.Never);
    //}

    //[Test]
    //public async Task CreatePosterAsync_CreatesPoster_NoPoster()
    //{
    //    // Arrange
    //    var awsBusiness = new Mock<ISatelliteAwsBusiness>();
    //    var summaryBusiness = new Mock<IDailySummaryBusiness>();
    //    var source = new Mock<ISatelliteSource>();
    //    var testable = new SatelliteProcess(source.Object, awsBusiness.Object, null!, summaryBusiness.Object);
    //    var ct = CancellationToken.None;
    //    var finalSize = new Point(128, 128);
    //    var summary = new StormEventsDailySummaryEntity
    //    {
    //        SatellitePath1080 = "a"
    //    };
    //    var satellite = new SatelliteAwsProductEntity();

    //    // Act
    //    await testable.CreatePosterAsync(satellite, summary, finalSize, null!, ct);

    //    // Assert
    //    source.Verify(v => v.MakePosterAsync(satellite, finalSize, null!, ct),
    //        Times.Exactly(1));
    //}

    //#endregion

    #region GetMarqueeSatelliteProduct

    [Test]
    public async Task GetMarqueeSatelliteProduct_Null_NoHeadlineEvent()
    {
        // Arrange
        var testable = new SatelliteProcess(null!, null!, null!);
        var ct = CancellationToken.None;
        var summary = new StormEventsDailySummaryEntity();

        // Act
        var result = await testable.GetMarqueeSatelliteProduct(summary, ct);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetMarqueeSatelliteProduct_Null_IsComplete()
    {
        // Arrange
        var testable = new SatelliteProcess(null!, null!, null!);
        var ct = CancellationToken.None;
        var summary = new StormEventsDailySummaryEntity
        {
            HeadlineEventTime = new DateTime(2021, 7, 18),
            SatellitePathPoster = "a",
            SatellitePath1080 = "b"
        };

        // Act
        var result = await testable.GetMarqueeSatelliteProduct(summary, ct);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetMarqueeSatelliteProduct_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        var source = new Mock<ISatelliteSource>();
        var testable = new SatelliteProcess(null!, null!, source.Object);
        var ct = CancellationToken.None;
        var time = new DateTime(2021, 7, 18, 18, 0, 0);
        var summary = new StormEventsDailySummaryEntity
        {
            Id = "2021-07-18",
            HeadlineEventTime = time
        };
        var expected = new SatelliteAwsProductEntity();
        source.Setup(s => s.GetMarqueeSatelliteProduct("2021-07-18", time, ct))
            .ReturnsAsync(expected);

        // Act
        var result = await testable.GetMarqueeSatelliteProduct(summary, ct);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion

    #region ProcessMissingDay

    [Test]
    public async Task ProcessMissingDayAsync_ShortCircuit_NoKeys()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int year = 2021;
        const string missingDay = "2021-07-21";
        const int satellite = 16;
        const int channel = 2;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var source = new Mock<ISatelliteSource>();
        var awsBusiness = new Mock<ISatelliteAwsBusiness>();
        var testable = new SatelliteProcess(awsBusiness.Object, null!, source.Object);

        // Act
        await testable.ProcessMissingDay(year, missingDay, satellite, channel, dayPart, null!, ct);

        // Assert
        source.Verify(v => v.AddInventoryToDatabase(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DayPartsEnum>(), ct),
            Times.Never);
    }

    [Test]
    public async Task ProcessMissingDayAsync_AwsProcess_RecentYear()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int year = 2021;
        const string missingDay = "2021-07-21";
        const int satellite = 16;
        const int channel = 2;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var source = new Mock<ISatelliteSource>();
        var awsBusiness = new Mock<ISatelliteAwsBusiness>();
        awsBusiness.Setup(s => s.ListKeys(missingDay, satellite, channel, dayPart, null!, ct))
            .ReturnsAsync(new AwsKeysModel
            {
                Keys = ["a, b"],
                GetScanTimeFunc = null!
            });
        var testable = new SatelliteProcess(awsBusiness.Object, null!, source.Object);

        // Act
        await testable.ProcessMissingDay(year, missingDay, satellite, channel, dayPart, null!, ct);

        // Assert
        source.Verify(v => v.AddInventoryToDatabase(missingDay, string.Empty, channel, dayPart, ct),
            Times.Exactly(1));
    }

    [Test]
    public async Task ProcessMissingDayAsync_IemProcess_OldYear()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int year = 2011;
        const string missingDay = "2011-07-21";
        const int satellite = 16;
        const int channel = 2;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var source = new Mock<ISatelliteSource>();
        var iemBusiness = new Mock<ISatelliteIemBusiness>();
        iemBusiness.Setup(s => s.ListKeys(missingDay, channel, dayPart, ct))
            .ReturnsAsync(new AwsKeysModel
            {
                Keys = ["a, b"],
                GetScanTimeFunc = null!
            });
        var testable = new SatelliteProcess(null!, iemBusiness.Object, source.Object);

        // Act
        await testable.ProcessMissingDay(year, missingDay, satellite, channel, dayPart, null!, ct);

        // Assert
        source.Verify(v => v.AddInventoryToDatabase(missingDay, string.Empty, channel, dayPart, ct),
            Times.Exactly(1));
    }

    #endregion

    //#region Source1080

    //[Test]
    //public async Task Source1080Async_AwsProcess_ValidParameters()
    //{
    //    // Arrange
    //    const int year = 2021;
    //    var awsBusiness = new Mock<ISatelliteAwsBusiness>();
    //    var source = new Mock<ISatelliteSource>();
    //    var testable = new SatelliteProcess(source.Object, awsBusiness.Object, null!, null!);
    //    var ct = CancellationToken.None;

    //    // Act
    //    await testable.Source1080Async(year, null!, null!, null!, null!, null!, ct);

    //    // Assert
    //    awsBusiness.Verify(v => v.DownloadAsync(It.IsAny<SatelliteAwsProductEntity>(),
    //            It.IsAny<Func<int, Task>>(), It.IsAny<BlobContainerClient>(), It.IsAny<IAmazonS3>(), ct),
    //        Times.Exactly(1));
    //}

    //[Test]
    //public async Task Source1080Async_IemProcess_ValidParameters()
    //{
    //    // Arrange
    //    const int year = 2011;
    //    var iemBusiness = new Mock<ISatelliteIemBusiness>();
    //    var source = new Mock<ISatelliteSource>();
    //    var testable = new SatelliteProcess(source.Object, null!, iemBusiness.Object, null!);
    //    var ct = CancellationToken.None;
    //    source.Setup(s => s.MessagePurpleAsync(null!, null!, ct))
    //        .ReturnsAsync(true);

    //    // Act
    //    await testable.Source1080Async(year, null!, null!, null!, null!, null!, ct);

    //    // Assert
    //    iemBusiness.Verify(v => v.DownloadAsync(It.IsAny<SatelliteAwsProductEntity>(),
    //            It.IsAny<Func<int, Task>>(), It.IsAny<BlobContainerClient>(), ct),
    //        Times.Exactly(1));
    //}

    //#endregion

    //#region Update1080

    //[Test]
    //public async Task Update1080Async_DoesNothing_AlreadyUpdated()
    //{
    //    // Arrange
    //    var awsBusiness = new Mock<ISatelliteAwsBusiness>();
    //    var summaryBusiness = new Mock<IDailySummaryBusiness>();
    //    var source = new Mock<ISatelliteSource>();
    //    var testable = new SatelliteProcess(source.Object, awsBusiness.Object, null!, summaryBusiness.Object);
    //    var ct = CancellationToken.None;
    //    source.Setup(s => s.MessagePurpleAsync(null!, null!, ct))
    //        .ReturnsAsync(true);
    //    var summary = new StormEventsDailySummaryEntity();
    //    var satellite = new SatelliteAwsProductEntity();

    //    // Act
    //    await testable.Update1080Async(satellite, summary, ct);

    //    // Assert
    //    summaryBusiness.Verify(v => v.UpdateCosmosAsync(summary, ct), Times.Never);
    //}

    //[Test]
    //public async Task Update1080Async_CompletesAllSteps_ValidParameters()
    //{
    //    // Arrange
    //    var awsBusiness = new Mock<ISatelliteAwsBusiness>();
    //    var summaryBusiness = new Mock<IDailySummaryBusiness>();
    //    var source = new Mock<ISatelliteSource>();
    //    var testable = new SatelliteProcess(source.Object, awsBusiness.Object, null!, summaryBusiness.Object);
    //    var ct = CancellationToken.None;
    //    source.Setup(s => s.MessagePurpleAsync(null!, null!, ct))
    //        .ReturnsAsync(true);
    //    var summary = new StormEventsDailySummaryEntity();
    //    var satellite = new SatelliteAwsProductEntity
    //    {
    //        Path1080 = "a"
    //    };

    //    // Act
    //    await testable.Update1080Async(satellite, summary, ct);

    //    // Assert
    //    Assert.That(summary.SatellitePath1080, Is.EqualTo("a"));
    //}

    //#endregion
}

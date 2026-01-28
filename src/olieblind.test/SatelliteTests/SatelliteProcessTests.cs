using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite;
using olieblind.lib.Satellite.Interfaces;
using SixLabors.ImageSharp;

namespace olieblind.test.SatelliteTests;

public class SatelliteProcessTests
{
    #region CreateThumbnailAndUpdateDailySummary

    [Test]
    public async Task CreateThumbnailAndUpdateDailySummary_DoesNothing_AlreadyCreated()
    {
        // Arrange
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteProcess(null!, null!, null!, repo.Object, null!);
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
        var testable = new SatelliteProcess(null!, null!, null!, repo.Object, null!);
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
        var source = new Mock<ISatelliteSource>();
        var testable = new SatelliteProcess(null!, null!, source.Object, repo.Object, null!);
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
        source.Verify(v => v.MakeThumbnail(satellite, finalSize, null!, ct),
            Times.Exactly(1));
    }

    #endregion

    #region GetMarqueeSatelliteProduct

    [Test]
    public async Task GetMarqueeSatelliteProduct_Null_NoHeadlineEvent()
    {
        // Arrange
        var testable = new SatelliteProcess(null!, null!, null!, null!, null!);
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
        var testable = new SatelliteProcess(null!, null!, null!, null!, null!);
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
        var testable = new SatelliteProcess(null!, null!, source.Object, null!, null!);
        var ct = CancellationToken.None;
        var time = new DateTime(2021, 7, 18, 18, 0, 0);
        var summary = new StormEventsDailySummaryEntity
        {
            Id = "2021-07-18",
            HeadlineEventTime = time
        };
        var expected = new SatelliteProductEntity();
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
        const int channel = 99;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var repo = new Mock<IMyRepository>();
        var source = new SatelliteTestSource { DelayFunc = null!, Repository = repo.Object };
        var testable = new SatelliteProcess(source, null!, null!, null!, null!);

        // Act
        await testable.ProcessMissingDay(year, missingDay, satellite, channel, dayPart, null!, ct);

        // Assert
        repo.Verify(v => v.SatelliteInventoryCreate(It.IsAny<SatelliteInventoryEntity>(), ct), Times.Never);
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
        var repo = new Mock<IMyRepository>();
        var source = new SatelliteTestSource { DelayFunc = null!, Repository = repo.Object };
        var testable = new SatelliteProcess(source, null!, null!, repo.Object, null!);

        // Act
        await testable.ProcessMissingDay(year, missingDay, satellite, channel, dayPart, null!, ct);

        // Assert
        repo.Verify(v => v.SatelliteInventoryCreate(It.IsAny<SatelliteInventoryEntity>(), ct), Times.Once);
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
        var repo = new Mock<IMyRepository>();
        var source = new SatelliteTestSource { DelayFunc = null!, Repository = repo.Object };
        var testable = new SatelliteProcess(null!, source, null!, null!, null!);

        // Act
        await testable.ProcessMissingDay(year, missingDay, satellite, channel, dayPart, null!, ct);

        // Assert
        repo.Verify(v => v.SatelliteInventoryCreate(It.IsAny<SatelliteInventoryEntity>(), ct), Times.Once);
    }

    #endregion

    //#region DownloadSatelliteFile

    //[Test]
    //public async Task DownloadSatelliteFile_AwsProcess_ValidParameters()
    //{
    //    // Arrange
    //    const int year = 2021;
    //    var awsBusiness = new Mock<ISatelliteAwsBusiness>();
    //    var source = new Mock<ISatelliteSource>();
    //    var testable = new SatelliteProcess(awsBusiness.Object, null!, source.Object, null!);
    //    var ct = CancellationToken.None;

    //    // Act
    //    await testable.DownloadSatelliteFile(year, null!, null!, null!, null!, null!, ct);

    //    // Assert
    //    awsBusiness.Verify(v => v.Download(It.IsAny<SatelliteAwsProductEntity>(),
    //            It.IsAny<Func<int, Task>>(), null!, null!, ct),
    //        Times.Once);
    //}

    //[Test]
    //public async Task DownloadSatelliteFile_IemProcess_ValidParameters()
    //{
    //    // Arrange
    //    const int year = 2011;
    //    var iemBusiness = new Mock<ISatelliteIemBusiness>();
    //    var source = new Mock<ISatelliteSource>();
    //    var testable = new SatelliteProcess(null!, iemBusiness.Object, source.Object, null!);
    //    var ct = CancellationToken.None;

    //    // Act
    //    await testable.DownloadSatelliteFile(year, null!, null!, null!, null!, null!, ct);

    //    // Assert
    //    iemBusiness.Verify(v => v.Download(It.IsAny<SatelliteAwsProductEntity>(), It.IsAny<Func<int, Task>>(), null!, ct),
    //        Times.Exactly(1));
    //}

    //#endregion

    #region UpdateDailySummary

    [Test]
    public async Task UpdateDailySummary_DoesNothing_AlreadyUpdated()
    {
        // Arrange
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteProcess(null!, null!, null!, repo.Object, null!);
        var ct = CancellationToken.None;
        var summary = new StormEventsDailySummaryEntity()
        {
            SatellitePath1080 = "a"
        };
        var satellite = new SatelliteProductEntity();

        // Act
        await testable.UpdateDailySummary(satellite, summary, ct);

        // Assert
        repo.Verify(v => v.StormEventsDailySummaryUpdate(summary, ct), Times.Never);
    }

    [Test]
    public async Task UpdateDailySummary_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteProcess(null!, null!, null!, repo.Object, null!);
        var ct = CancellationToken.None;
        var summary = new StormEventsDailySummaryEntity();
        var satellite = new SatelliteProductEntity
        {
            Path1080 = "a"
        };

        // Act
        await testable.UpdateDailySummary(satellite, summary, ct);

        // Assert
        Assert.That(summary.SatellitePath1080, Is.EqualTo("a"));
    }

    #endregion
}

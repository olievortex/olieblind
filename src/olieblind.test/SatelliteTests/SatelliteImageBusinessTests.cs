using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite;
using olieblind.lib.Satellite.Sources;
using olieblind.lib.Services;
using olieblind.test.SatelliteTests.SourcesTests;
using SixLabors.ImageSharp;

namespace olieblind.test.SatelliteTests;

public class SatelliteImageBusinessTests
{
    #region AddInventoryToDatabase

    [Test]
    public async Task AddInventoryToDatabase_AddsInventory_ValidParameters()
    {
        // Arrange
        const int channel = 2;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var entity = new SatelliteInventoryEntity();
        var ct = CancellationToken.None;
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.SatelliteInventoryCreate(It.IsAny<SatelliteInventoryEntity>(), ct))
            .Callback((SatelliteInventoryEntity e, CancellationToken _) => entity = e);
        var testable = new SatelliteImageBusiness(null!, null!, repo.Object);
        var effectiveDate = Guid.NewGuid().ToString();
        var bucket = Guid.NewGuid().ToString();

        // Act
        await testable.AddInventoryToDatabase(effectiveDate, bucket, channel, dayPart, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(entity.EffectiveDate, Is.EqualTo(effectiveDate));
            Assert.That(entity.Id, Is.EqualTo(bucket));
            Assert.That(entity.Channel, Is.EqualTo(2));
            Assert.That(entity.DayPart, Is.EqualTo(DayPartsEnum.Afternoon));
            Assert.That(entity.Timestamp, Is.Not.EqualTo(DateTime.MinValue));
        }
    }

    #endregion

    #region AddProductsToDatabase

    [Test]
    public async Task AddProductsToDatabase_AddsProducts_ValidParameters()
    {
        // Arrange
        const int channel = 2;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var scanTime = new DateTime(2017, 3, 1, 2, 2, 49, 700);
        var entity = new SatelliteProductEntity();
        var ct = CancellationToken.None;
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.SatelliteProductCreate(It.IsAny<List<SatelliteProductEntity>>(), ct))
            .Callback((List<SatelliteProductEntity> e, CancellationToken _) => entity = e[0]);
        var testable = new SatelliteImageBusiness(null!, null!, repo.Object);
        const string key = "OR_ABI-L1b-RadC-M3C01_G16_s20170600202497_e20170600205270_c20170600205311.nc";
        var effectiveDate = Guid.NewGuid().ToString();
        var bucket = Guid.NewGuid().ToString();

        // Act
        await testable.AddProductsToDatabase([key], effectiveDate, bucket, channel, dayPart,
            _ => scanTime, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(entity.EffectiveDate, Is.EqualTo(effectiveDate));
            Assert.That(entity.Id, Is.EqualTo(key));
            Assert.That(entity.BucketName, Is.EqualTo(bucket));
            Assert.That(entity.Channel, Is.EqualTo(2));
            Assert.That(entity.DayPart, Is.EqualTo(DayPartsEnum.Afternoon));
            Assert.That(entity.Path1080, Is.Null);
            Assert.That(entity.PathPoster, Is.Null);
            Assert.That(entity.PathSource, Is.Null);
            Assert.That(entity.ScanTime, Is.EqualTo(scanTime));
            Assert.That(entity.Timestamp, Is.Not.EqualTo(DateTime.MinValue));
            Assert.That(entity.TimeTaken1080, Is.Zero);
            Assert.That(entity.TimeTakenDownload, Is.Zero);
            Assert.That(entity.TimeTakenPoster, Is.Zero);
        }
    }

    #endregion

    #region CreateDelayFunc

    [Test]
    public async Task CreateDelayFunc_ValidTask_ValidParameters()
    {
        // Arrange
        const int attempt = 0;
        var ct = CancellationToken.None;
        var task = SatelliteImageBusiness.CreateDelayFunc(ct);

        // Act
        await task(attempt);

        // Assert
        Assert.Pass();
    }

    #endregion

    #region CreateSatelliteSource

    [Test]
    public void CreateSatelliteSource_ReturnsAws_ModernYear()
    {
        // Arrange
        const int year = 2021;
        var testable = new SatelliteImageBusiness(null!, null!, null!);

        // Act
        var result = testable.CreateSatelliteSource(year, null!);

        // Assert
        Assert.That(result, Is.InstanceOf<SatelliteAwsSource>());
    }

    [Test]
    public void CreateSatelliteSource_ReturnsIem_ModernYear()
    {
        // Arrange
        const int year = 2015;
        var testable = new SatelliteImageBusiness(null!, null!, null!);

        // Act
        var result = testable.CreateSatelliteSource(year, null!);

        // Assert
        Assert.That(result, Is.InstanceOf<SatelliteIemSource>());
    }

    #endregion

    #region DownloadProduct

    [Test]
    public async Task DownloadProduct_UpdatesProduct_ValidParameters()
    {
        // Arrange
        var ows = new Mock<IOlieWebService>();
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteImageBusiness(ows.Object, null!, repo.Object);
        var ct = CancellationToken.None;
        var product = new SatelliteProductEntity();
        var source = new SatelliteTestSource { Ows = ows.Object };

        // Act
        await testable.DownloadProduct(product, source, null!, ct);

        // Assert
        Assert.That(product.PathLocal, Is.EqualTo("b"));
    }

    [Test]
    public async Task DownloadProduct_ShortCircuit_AlreadyDownloaded()
    {
        // Arrange
        var ows = new Mock<IOlieWebService>();
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteImageBusiness(ows.Object, null!, repo.Object);
        var ct = CancellationToken.None;
        var product = new SatelliteProductEntity { PathSource = "a" };
        var source = new SatelliteTestSource { Ows = ows.Object };

        // Act
        await testable.DownloadProduct(product, source, null!, ct);

        // Assert
        repo.Verify(v => v.SatelliteProductUpdate(product, ct), Times.Never);
    }

    #endregion

    #region GetMarqueeProduct

    [Test]
    public async Task GetMarqueeProduct_Null_NoHeadlineEvent()
    {
        // Arrange
        var testable = new SatelliteImageBusiness(null!, null!, null!);
        var ct = CancellationToken.None;
        var summary = new StormEventsDailySummaryEntity();

        // Act
        var result = await testable.GetMarqueeProduct(summary, ct);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetMarqueeProduct_Null_IsComplete()
    {
        // Arrange
        var testable = new SatelliteImageBusiness(null!, null!, null!);
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
    public async Task GetMarqueeProduct_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteImageBusiness(null!, null!, repo.Object);
        var ct = CancellationToken.None;
        const string effectiveDate = "2021-05-18";
        var eventTime = new DateTime(2021, 5, 18, 18, 0, 0);
        var expected = new SatelliteProductEntity();
        repo.Setup(s => s.SatelliteProductGetPoster(effectiveDate, eventTime, ct))
            .ReturnsAsync(expected);
        var summary = new StormEventsDailySummaryEntity { Id = effectiveDate, HeadlineEventTime = eventTime };

        // Act
        var result = await testable.GetMarqueeProduct(summary, ct);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion

    #region Make1080

    [Test]
    public async Task Make1080_NoException_ValidData()
    {
        // Arrange
        const string purpleCmdPath = "purple.sh";
        var ct = CancellationToken.None;
        var config = new Mock<IOlieConfig>();
        config.SetupGet(g => g.PurpleCmdPath).Returns(purpleCmdPath);
        var ows = new Mock<IOlieWebService>();
        var testable = new SatelliteImageBusiness(ows.Object, null!, null!);
        var product = new SatelliteProductEntity { Id = "a", EffectiveDate = "b" };

        // Act
        await testable.Make1080(product, config.Object, ct);

        // Assert
        ows.Verify(v => v.Shell(It.IsAny<IOlieConfig>(), purpleCmdPath, "olievortex_purple_nc_2_png.py a b", ct));
    }

    #endregion

    #region MakePoster

    [Test]
    public async Task MakePoster_ShortCircuit_ExistingPoster()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var ois = new Mock<IOlieImageService>();
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteImageBusiness(ows.Object, ois.Object, repo.Object);
        var satellite = new SatelliteProductEntity { PathPoster = "a" };
        var finalSize = new Point(128, 128);

        // Act
        await testable.MakePoster(satellite, finalSize, null!, ct);

        // Assert
        Assert.That(satellite.Timestamp, Is.EqualTo(DateTime.MinValue));
    }

    [Test]
    public void MakePoster_ThrowsException_MissingSource()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var ois = new Mock<IOlieImageService>();
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteImageBusiness(ows.Object, ois.Object, repo.Object);
        var satellite = new SatelliteProductEntity();
        var finalSize = new Point(128, 128);

        // Act, Assert
        Assert.ThrowsAsync<NullReferenceException>(() =>
            testable.MakePoster(satellite, finalSize, null!, ct));
    }

    [Test]
    public async Task MakePoster_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var ois = new Mock<IOlieImageService>();
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteImageBusiness(ows.Object, ois.Object, repo.Object);
        var satellite = new SatelliteProductEntity { Path1080 = "a" };
        var finalSize = new Point(128, 128);

        // Act
        await testable.MakePoster(satellite, finalSize, null!, ct);

        // Assert
        Assert.That(satellite.Timestamp, Is.Not.EqualTo(DateTime.MinValue));
    }

    #endregion

    #region UpdateDailySummary1080

    [Test]
    public async Task UpdateDailySummary1080_DoesNothing_AlreadyUpdated()
    {
        // Arrange
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteImageBusiness(null!, null!, repo.Object);
        var ct = CancellationToken.None;
        var summary = new StormEventsDailySummaryEntity()
        {
            SatellitePath1080 = "a"
        };
        var satellite = new SatelliteProductEntity();

        // Act
        await testable.UpdateDailySummary1080(satellite, summary, ct);

        // Assert
        repo.Verify(v => v.StormEventsDailySummaryUpdate(summary, ct), Times.Never);
    }

    [Test]
    public async Task UpdateDailySummary1080_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteImageBusiness(null!, null!, repo.Object);
        var ct = CancellationToken.None;
        var summary = new StormEventsDailySummaryEntity();
        var satellite = new SatelliteProductEntity
        {
            Path1080 = "a"
        };

        // Act
        await testable.UpdateDailySummary1080(satellite, summary, ct);

        // Assert
        Assert.That(summary.SatellitePath1080, Is.EqualTo("a"));
    }

    #endregion

    #region CreatePosterForDailySummary

    [Test]
    public async Task UpdateDailySummaryPoster_DoesNothing_AlreadyUpdated()
    {
        // Arrange
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteImageBusiness(null!, null!, repo.Object);
        var ct = CancellationToken.None;
        var summary = new StormEventsDailySummaryEntity()
        {
            SatellitePathPoster = "a"
        };
        var satellite = new SatelliteProductEntity();

        // Act
        await testable.UpdateDailySummaryPoster(satellite, summary, ct);

        // Assert
        repo.Verify(v => v.StormEventsDailySummaryUpdate(summary, ct), Times.Never);
    }

    [Test]
    public async Task UpdateDailySummaryPoster_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteImageBusiness(null!, null!, repo.Object);
        var ct = CancellationToken.None;
        var summary = new StormEventsDailySummaryEntity();
        var satellite = new SatelliteProductEntity
        {
            PathPoster = "a"
        };

        // Act
        await testable.UpdateDailySummaryPoster(satellite, summary, ct);

        // Assert
        Assert.That(summary.SatellitePathPoster, Is.EqualTo("a"));
    }

    #endregion

}

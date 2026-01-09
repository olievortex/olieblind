using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Services;

namespace olieblind.test.SatelliteTests;

public class SatelliteSourceTests
{
    #region AddInventoryToDatabase

    [Test]
    public async Task AddInventoryToDatabase_AddsInventory_ValidParameters()
    {
        // Arrange
        const int channel = 2;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var entity = new SatelliteAwsInventoryEntity();
        var ct = CancellationToken.None;
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.SatelliteAwsInventoryCreate(It.IsAny<SatelliteAwsInventoryEntity>(), ct))
            .Callback((SatelliteAwsInventoryEntity e, CancellationToken _) => entity = e);
        var testable = new SatelliteSource(repo.Object, null!);
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
        var entity = new SatelliteAwsProductEntity();
        var ct = CancellationToken.None;
        var source = new Mock<ISatelliteAwsSource>();
        source.Setup(s => s.GetScanTime(It.IsAny<string>())).Returns(scanTime);
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.SatelliteAwsProductCreate(It.IsAny<List<SatelliteAwsProductEntity>>(), ct))
            .Callback((List<SatelliteAwsProductEntity> e, CancellationToken _) => entity = e[0]);
        var testable = new SatelliteSource(repo.Object, null!);
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

    #region GetEffectiveDate

    [Test]
    public void GetEffectiveDate_Date_ValidParameters()
    {
        // Arrange
        const string effectiveDate = "2010-07-21";
        var testable = new SatelliteSource(null!, null!);

        // Act
        var result = testable.GetEffectiveDate(effectiveDate);

        // Assert
        Assert.That(result, Is.EqualTo(new DateTime(2010, 7, 21)));
    }

    #endregion

    #region GetEffectiveStart

    [Test]
    public void GetEffectiveStart_ReturnsDateTime_Owl()
    {
        // Arrange
        var effectiveDate = new DateTime(2021, 5, 18);
        var expected = new DateTime(2021, 5, 18, 6, 0, 0);
        var testable = new SatelliteSource(null!, null!);

        // Act
        var result = testable.GetEffectiveStart(effectiveDate, DayPartsEnum.Owl);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GetEffectiveStart_ReturnsDateTime_Morning()
    {
        // Arrange
        var effectiveDate = new DateTime(2021, 5, 18);
        var expected = new DateTime(2021, 5, 18, 12, 0, 0);
        var testable = new SatelliteSource(null!, null!);

        // Act
        var result = testable.GetEffectiveStart(effectiveDate, DayPartsEnum.Morning);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GetEffectiveStart_ReturnsDateTime_Afternoon()
    {
        // Arrange
        var effectiveDate = new DateTime(2021, 5, 18);
        var expected = new DateTime(2021, 5, 18, 18, 0, 0);
        var testable = new SatelliteSource(null!, null!);

        // Act
        var result = testable.GetEffectiveStart(effectiveDate, DayPartsEnum.Afternoon);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GetEffectiveStart_ReturnsDateTime_Night()
    {
        // Arrange
        var effectiveDate = new DateTime(2021, 5, 18);
        var expected = new DateTime(2021, 5, 19);
        var testable = new SatelliteSource(null!, null!);

        // Act
        var result = testable.GetEffectiveStart(effectiveDate, DayPartsEnum.Night);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion

    #region GetEffectiveStop

    [Test]
    public void GetEffectiveStop_ReturnsDateTime_Owl()
    {
        // Arrange
        var effectiveDate = new DateTime(2021, 5, 18);
        var expected = new DateTime(2021, 5, 18, 11, 0, 0);
        var testable = new SatelliteSource(null!, null!);

        // Act
        var result = testable.GetEffectiveStop(effectiveDate, DayPartsEnum.Owl);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GetEffectiveStop_ReturnsDateTime_Morning()
    {
        // Arrange
        var effectiveDate = new DateTime(2021, 5, 18);
        var expected = new DateTime(2021, 5, 18, 17, 0, 0);
        var testable = new SatelliteSource(null!, null!);

        // Act
        var result = testable.GetEffectiveStop(effectiveDate, DayPartsEnum.Morning);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GetEffectiveStop_ReturnsDateTime_Afternoon()
    {
        // Arrange
        var effectiveDate = new DateTime(2021, 5, 18);
        var expected = new DateTime(2021, 5, 18, 23, 0, 0);
        var testable = new SatelliteSource(null!, null!);

        // Act
        var result = testable.GetEffectiveStop(effectiveDate, DayPartsEnum.Afternoon);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GetEffectiveStop_ReturnsDateTime_Night()
    {
        // Arrange
        var effectiveDate = new DateTime(2021, 5, 18);
        var expected = new DateTime(2021, 5, 19, 5, 0, 0);
        var testable = new SatelliteSource(null!, null!);

        // Act
        var result = testable.GetEffectiveStop(effectiveDate, DayPartsEnum.Night);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion

    //#region GetPath

    //[Test]
    //public void GetPrefix_Prefix_ValidParameters()
    //{
    //    // Arrange
    //    var testable = new SatelliteSource(null!, null!, null!);
    //    const string metal = "gold";
    //    var effectiveDate = new DateTime(2021, 7, 21);

    //    // Act
    //    var result = testable.GetPath(effectiveDate, metal);

    //    // Assert
    //    Assert.That(result, Is.EqualTo("gold/aws/satellite/2021/07/21"));
    //}

    //#endregion

    //#region GetProductList

    //[Test]
    //public async Task GetProductListAsync_AllSteps_Valid()
    //{
    //    // Arrange
    //    var ows = new Mock<IOlieWebServices>();
    //    var ois = new Mock<IOlieImageServices>();
    //    var cosmos = new Mock<ICosmosRepository>();
    //    var testable = new SatelliteSource(ows.Object, cosmos.Object, ois.Object);
    //    var ct = CancellationToken.None;
    //    const string effectiveDate = "2021-05-18";
    //    const string bucketName = "dillon";
    //    const int channel = 12;
    //    var expected = new List<SatelliteAwsProductEntity>();
    //    cosmos.Setup(s => s.SatelliteAwsProductListAsync(effectiveDate, bucketName, channel, ct))
    //        .ReturnsAsync(expected);

    //    // Act
    //    var result = await testable.GetProductListAsync(effectiveDate, bucketName, channel, ct);

    //    // Assert
    //    Assert.That(result, Is.EqualTo(expected));
    //}

    //#endregion

    //#region GetProductListNoPoster

    //[Test]
    //public async Task GetProductListNoPosterAsync_AllSteps_Valid()
    //{
    //    // Arrange
    //    var ows = new Mock<IOlieWebServices>();
    //    var ois = new Mock<IOlieImageServices>();
    //    var cosmos = new Mock<ICosmosRepository>();
    //    var testable = new SatelliteSource(ows.Object, cosmos.Object, ois.Object);
    //    var ct = CancellationToken.None;
    //    var expected = new List<SatelliteAwsProductEntity>();
    //    cosmos.Setup(s => s.SatelliteAwsProductListNoPosterAsync(ct))
    //        .ReturnsAsync(expected);

    //    // Act
    //    var result = await testable.GetProductListNoPosterAsync(ct);

    //    // Assert
    //    Assert.That(result, Is.EqualTo(expected));
    //}

    //#endregion

    #region GetMarqueeSatelliteProduct

    [Test]
    public async Task GetMarqueeSatelliteProduct_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteSource(repo.Object, null!);
        var ct = CancellationToken.None;
        const string effectiveDate = "2021-05-18";
        var eventTime = new DateTime(2021, 5, 18, 18, 0, 0);
        var expected = new SatelliteAwsProductEntity();
        repo.Setup(s => s.SatelliteAwsProductGetPoster(effectiveDate, eventTime, ct))
            .ReturnsAsync(expected);

        // Act
        var result = await testable.GetMarqueeSatelliteProduct(effectiveDate, eventTime, ct);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion

    //#region MakePoster

    //[Test]
    //public async Task MakePosterAsync_ShortCircuit_ExistingPoster()
    //{
    //    // Arrange
    //    var ct = CancellationToken.None;
    //    var ows = new Mock<IOlieWebServices>();
    //    var ois = new Mock<IOlieImageServices>();
    //    var cosmos = new Mock<ICosmosRepository>();
    //    var testable = new SatelliteSource(ows.Object, cosmos.Object, ois.Object);
    //    var satellite = new SatelliteAwsProductEntity { PathPoster = "a" };
    //    var finalSize = new Point(128, 128);

    //    // Act
    //    await testable.MakePosterAsync(satellite, finalSize, null!, ct);

    //    // Assert
    //    Assert.That(satellite.Timestamp, Is.EqualTo(DateTime.MinValue));
    //}

    //[Test]
    //public void MakePosterAsync_ThrowsException_MissingSource()
    //{
    //    // Arrange
    //    var ct = CancellationToken.None;
    //    var ows = new Mock<IOlieWebServices>();
    //    var ois = new Mock<IOlieImageServices>();
    //    var cosmos = new Mock<ICosmosRepository>();
    //    var testable = new SatelliteSource(ows.Object, cosmos.Object, ois.Object);
    //    var satellite = new SatelliteAwsProductEntity();
    //    var finalSize = new Point(128, 128);

    //    // Act, Assert
    //    Assert.ThrowsAsync<NullReferenceException>(() =>
    //        testable.MakePosterAsync(satellite, finalSize, null!, ct));
    //}

    //[Test]
    //public async Task MakePosterAsync_CompletesAllSteps_ValidParameters()
    //{
    //    // Arrange
    //    var ct = CancellationToken.None;
    //    var ows = new Mock<IOlieWebServices>();
    //    var ois = new Mock<IOlieImageServices>();
    //    var cosmos = new Mock<ICosmosRepository>();
    //    var testable = new SatelliteSource(ows.Object, cosmos.Object, ois.Object);
    //    var satellite = new SatelliteAwsProductEntity { Path1080 = "a" };
    //    var finalSize = new Point(128, 128);

    //    // Act
    //    await testable.MakePosterAsync(satellite, finalSize, null!, ct);

    //    // Assert
    //    Assert.That(satellite.Timestamp, Is.Not.EqualTo(DateTime.MinValue));
    //}

    //#endregion

    #region MessagePurple

    [Test]
    public async Task MessagePurpleAsync_ShortCircuit_Missing1080Path()
    {
        // Arrange
        var ows = new Mock<IOlieWebService>();
        var testable = new SatelliteSource(null!, ows.Object);
        var satellite = new SatelliteAwsProductEntity();
        var ct = CancellationToken.None;

        // Act
        await testable.MessagePurple(satellite, null!, ct);

        // Assert
        ows.Verify(v => v.ServiceBusSendJson(null!, satellite, ct),
            Times.Never());
    }

    [Test]
    public async Task MessagePurpleAsync_ShortCircuit_AlreadyProcessed()
    {
        // Arrange
        var ows = new Mock<IOlieWebService>();
        var testable = new SatelliteSource(null!, ows.Object);
        var satellite = new SatelliteAwsProductEntity()
        {
            Path1080 = "meow"
        };
        var ct = CancellationToken.None;

        // Act
        await testable.MessagePurple(satellite, null!, ct);

        // Assert
        ows.Verify(v => v.ServiceBusSendJson(null!, satellite, ct),
            Times.Never());
    }

    [Test]
    public async Task MessagePurpleAsync_SendsMessage_ValidParameters()
    {
        // Arrange
        var ows = new Mock<IOlieWebService>();
        var testable = new SatelliteSource(null!, ows.Object);
        var satellite = new SatelliteAwsProductEntity
        {
            PathSource = "meow"
        };
        var ct = CancellationToken.None;

        // Act
        await testable.MessagePurple(satellite, null!, ct);

        // Assert
        ows.Verify(v => v.ServiceBusSendJson(null!, satellite, ct),
            Times.Once());
    }

    #endregion

    //#region Start1080Container

    //[Test]
    //public async Task Start1080ContainerAsync_CompletesAllSteps_ValidParameters()
    //{
    //    // Arrange
    //    const int containerLimit = 2;
    //    var ows = new Mock<IOlieWebServices>();
    //    var cosmos = new Mock<ICosmosRepository>();
    //    var ois = new Mock<IOlieImageServices>();
    //    var testable = new SatelliteSource(ows.Object, cosmos.Object, ois.Object);
    //    var ct = CancellationToken.None;

    //    // Act
    //    await testable.Start1080ContainersAsync(null!, containerLimit, ct);

    //    // Assert
    //    ows.Verify(v => v.StartContainerGroupsAsync(It.IsAny<ContainerStartInfo>(), containerLimit, ct),
    //        Times.Exactly(1));
    //}

    //#endregion
}

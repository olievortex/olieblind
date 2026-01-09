using Moq;
using olieblind.data.Enums;
using olieblind.lib.Satellite;
using olieblind.lib.Satellite.Interfaces;

namespace olieblind.test.SatelliteTests;

public class SatelliteIemBusinessTests
{
    [Test]
    public void Download_ThrowsException()
    {
        var testable = new SatelliteIemBusiness(null!, null!);
        Assert.Throws<NotImplementedException>(() => testable.Download(null!, null!, null!, CancellationToken.None));
    }

    //#region Download

    //[Test]
    //public async Task DownloadAsync_ShortCircuit_AlreadyDownloaded()
    //{
    //    // Arrange
    //    var ct = CancellationToken.None;
    //    var ows = new Mock<IOlieWebServices>();
    //    var iemSource = new Mock<ISatelliteIemSource>();
    //    var source = new Mock<ISatelliteSource>();
    //    var cosmos = new Mock<ICosmosRepository>();
    //    var testable = new SatelliteIemBusiness(source.Object, iemSource.Object, ows.Object, cosmos.Object);
    //    var product = new SatelliteAwsProductEntity
    //    {
    //        PathSource = "a"
    //    };

    //    // Act
    //    await testable.DownloadAsync(product, Delay, null!, ct);

    //    // Assert
    //    Assert.That(product.Timestamp, Is.EqualTo(DateTime.MinValue));
    //}

    //[Test]
    //public async Task DownloadAsync_Retries_OneFailure()
    //{
    //    // Arrange
    //    var ct = CancellationToken.None;
    //    var ows = new Mock<IOlieWebServices>();
    //    ows.SetupSequence(s => s.ApiGetBytesAsync(It.IsAny<string>(), ct))
    //        .ThrowsAsync(new ApplicationException("Olie"))
    //        .ReturnsAsync([]);
    //    var iemSource = new Mock<ISatelliteIemSource>();
    //    var source = new Mock<ISatelliteSource>();
    //    var cosmos = new Mock<ICosmosRepository>();
    //    var testable = new SatelliteIemBusiness(source.Object, iemSource.Object, ows.Object, cosmos.Object);
    //    var product = new SatelliteAwsProductEntity();

    //    // Act
    //    await testable.DownloadAsync(product, Delay, null!, ct);

    //    // Assert
    //    Assert.That(product.Timestamp, Is.Not.EqualTo(DateTime.MinValue));
    //}

    //[Test]
    //public void DownloadAsync_Trows_ThreeFailures()
    //{
    //    // Arrange
    //    var ct = CancellationToken.None;
    //    var ows = new Mock<IOlieWebServices>();
    //    ows.Setup(s => s.ApiGetBytesAsync(It.IsAny<string>(), ct))
    //        .ThrowsAsync(new ApplicationException("Olie"));
    //    var iemSource = new Mock<ISatelliteIemSource>();
    //    var source = new Mock<ISatelliteSource>();
    //    var cosmos = new Mock<ICosmosRepository>();
    //    var testable = new SatelliteIemBusiness(source.Object, iemSource.Object, ows.Object, cosmos.Object);
    //    var product = new SatelliteAwsProductEntity();

    //    // Act, Assert
    //    Assert.ThrowsAsync<ApplicationException>(() => testable.DownloadAsync(product, Delay, null!, ct));
    //}

    //private static Task Delay(int _)
    //{
    //    return Task.CompletedTask;
    //}

    //#endregion

    #region ListKeys

    [Test]
    public async Task ListKeys_List_ValidParameters()
    {
        // Arrange
        const string value = "2021-07-18";
        const int channel = 2;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var ct = CancellationToken.None;
        var listFiles = new List<string> { "a", "b", "c" };
        var effectiveDate = new DateTime(2021, 7, 18);
        var url = Guid.NewGuid().ToString();
        var start = new DateTime(2021, 7, 18, 18, 0, 0);
        var source = new Mock<ISatelliteSource>();
        source.Setup(s => s.GetEffectiveDate(value)).Returns(effectiveDate);
        source.Setup(s => s.GetEffectiveStart(effectiveDate, dayPart)).Returns(start);
        source.Setup(s => s.GetEffectiveStop(effectiveDate, dayPart)).Returns(start);
        var iemSource = new Mock<ISatelliteIemSource>();
        iemSource.Setup(s => s.GetPrefix(effectiveDate)).Returns(url);
        iemSource.Setup(s => s.IemList(url, ct)).ReturnsAsync(listFiles);
        iemSource.SetupSequence(s => s.GetChannelFromKey(It.IsAny<string>()))
            .Returns(channel)
            .Returns(0)
            .Returns(channel);
        iemSource.Setup(s => s.GetScanTimeFromKey(effectiveDate, It.IsAny<string>())).Returns(start);
        var testable = new SatelliteIemBusiness(source.Object, iemSource.Object);

        // Act
        var result = await testable.ListKeys(value, channel, dayPart, ct);
        var scanTime = result?.GetScanTimeFunc(string.Empty);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Keys, Has.Length.EqualTo(2));
            Assert.That(result?.Bucket, Is.EqualTo("IEM"));
            Assert.That(scanTime, Is.EqualTo(start));
        }
    }

    [Test]
    public async Task ListKeys_Null_DateTooOld()
    {
        // Arrange
        const string value = "2009-07-18";
        const int channel = 2;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var ct = CancellationToken.None;
        var effectiveDate = new DateTime(2009, 7, 18);
        var source = new Mock<ISatelliteSource>();
        source.Setup(s => s.GetEffectiveDate(value)).Returns(effectiveDate);
        var iemSource = new Mock<ISatelliteIemSource>();
        var testable = new SatelliteIemBusiness(source.Object, iemSource.Object);

        // Act
        var result = await testable.ListKeys(value, channel, dayPart, ct);

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion
}
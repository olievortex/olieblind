using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite;
using olieblind.lib.Services;

namespace olieblind.test.SatelliteTests.SourcesTests;

public class SatelliteIemSourceTests
{
    private readonly string _html = File.ReadAllText("./Resources/Iem/archive_data_2021_03_26_GIS_sat.html");

    #region Download

    [Test]
    public async Task Download_Retries_OneFailure()
    {
        // Arrange
        const string value = "2021-07-18";
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        ows.SetupSequence(s => s.ApiGetBytes(It.IsAny<string>(), ct))
            .ThrowsAsync(new ApplicationException("Olie"))
            .ReturnsAsync([]);
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteIemSource { Ows = ows.Object, Repository = null! };
        var product = new SatelliteProductEntity { Id = "Dillon.tif", EffectiveDate = value };

        // Act
        var (blobName, localFilename) = await testable.Download(product, Delay, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(blobName, Is.EqualTo("bronze/aws/satellite/2021/07/18/Dillon.tif"));
            Assert.That(localFilename, Contains.Substring(".tif"));
        }
    }

    [Test]
    public void Download_Throws_ThreeFailures()
    {
        // Arrange
        const string value = "2021-07-18";
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.ApiGetBytes(It.IsAny<string>(), ct))
            .ThrowsAsync(new ApplicationException("Olie"));
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteIemSource { Ows = ows.Object, Repository = null! };
        var product = new SatelliteProductEntity { Id = "Dillon.tif", EffectiveDate = value };

        // Act, Assert
        Assert.ThrowsAsync<ApplicationException>(() => testable.Download(product, Delay, ct));
    }

    private static Task Delay(int _)
    {
        return Task.CompletedTask;
    }

    #endregion

    #region ListKeys

    [Test]
    public async Task ListKeys_List_ValidParameters()
    {
        // Arrange
        const string value = "2021-07-18";
        const int channel = 2;
        const int satellite = 0;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var ct = CancellationToken.None;
        var listFiles = new List<string> { "a", "b", "c" };
        var effectiveDate = new DateTime(2021, 7, 18);
        var url = Guid.NewGuid().ToString();
        var start = new DateTime(2021, 7, 18, 18, 0, 0);
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.ApiGetString(It.IsAny<string>(), ct))
            .ReturnsAsync(_html);
        var testable = new SatelliteIemSource { Ows = ows.Object, Repository = null! };

        // Act
        var result = await testable.ListKeys(value, satellite, channel, dayPart, ct);
        var scanTime = result?.GetScanTimeFunc("conus_goes_vis4km_1830.tif");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Keys, Has.Length.EqualTo(24));
            Assert.That(result?.Bucket, Is.EqualTo("IEM"));
            Assert.That(scanTime, Is.EqualTo(new DateTime(2021, 7, 18, 18, 30, 0)));
        }
    }

    [Test]
    public async Task ListKeys_Null_DateTooOld()
    {
        // Arrange
        const string value = "2009-07-18";
        const int satellite = 0;
        const int channel = 2;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var ct = CancellationToken.None;
        var testable = new SatelliteIemSource { Ows = null!, Repository = null! };

        // Act
        var result = await testable.ListKeys(value, satellite, channel, dayPart, ct);

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion

    #region IemList

    [Test]
    public async Task IemList_List_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var url = Guid.NewGuid().ToString();
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.ApiGetString(It.IsAny<string>(), ct))
            .ReturnsAsync(_html);
        var testable = new SatelliteIemSource { Ows = ows.Object, Repository = null! };

        // Act
        var result = await testable.IemList(url, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Count.EqualTo(288));
            Assert.That(result[0], Is.EqualTo("conus_goes_ir4km_0000.tif"));
        }
    }

    #endregion

    #region GetChannelFromKey

    [Test]
    public void GetChannelFromKey_14_InfraRed()
    {
        // Arrange
        const string value = "conus_goes_ir4km_0230.tif";
        const int expected = 14;

        // Act
        var result = SatelliteIemSource.GetChannelFromKey(value);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GetChannelFromKey_2_Visible()
    {
        // Arrange
        const string value = "conus_goes_vis4km_0300.tif";
        const int expected = 2;

        // Act
        var result = SatelliteIemSource.GetChannelFromKey(value);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GetChannelFromKey_10_WaterVapor()
    {
        // Arrange
        const string value = "conus_goes_wv4km_0300.tif";
        const int expected = 10;

        // Act
        var result = SatelliteIemSource.GetChannelFromKey(value);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GetChannelFromKey_N1_Unknown()
    {
        // Arrange
        const string value = "dillon.tif";
        const int expected = -1;

        // Act
        var result = SatelliteIemSource.GetChannelFromKey(value);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion

    #region GetPath

    [Test]
    public void GetPrefix_Url_ValidParameters()
    {
        // Arrange
        var effectiveDate = new DateTime(2021, 7, 18);
        const string expected = "https://mesonet.agron.iastate.edu/archive/data/2021/07/18/GIS/sat/";

        // Act
        var result = SatelliteIemSource.GetPrefix(effectiveDate);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion

    #region GetScanTimeFromKey

    [Test]
    public void GetScanTimeFromKey_Value_ValidParameters()
    {
        // Arrange
        const string value = "conus_goes_vis4km_1830.tif";
        var effectiveDate = new DateTime(2021, 7, 18);
        var expected = new DateTime(2021, 7, 18, 18, 30, 0);

        // Act
        var result = SatelliteIemSource.GetScanTimeFromKey(effectiveDate, value);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion
}
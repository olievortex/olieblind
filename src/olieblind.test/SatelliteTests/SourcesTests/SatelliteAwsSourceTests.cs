using Amazon.S3;
using Moq;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite.Sources;
using olieblind.lib.Services;

namespace olieblind.test.SatelliteTests.SourcesTests;

public class SatelliteAwsSourceTests
{
    #region Download

    [Test]
    public async Task Download_Retries_OneFailure()
    {
        // Arrange
        const string effectiveDate = "2021-05-18";
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        ows.SetupSequence(s => s.AwsDownload(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), null!, ct))
            .Returns(Task.FromException(new AmazonS3Exception("Olie")))
            .Returns(Task.CompletedTask);
        var testable = new SatelliteAwsSource { AmazonS3Client = null!, Ows = ows.Object, Repository = null! };
        var product = new SatelliteProductEntity { Id = "Dillon.nc", EffectiveDate = effectiveDate };

        // Act
        var (blobName, localFileName) = await testable.Download(product, Delay, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(blobName, Is.EqualTo("bronze/aws/satellite/2021/05/18/Dillon.nc"));
            Assert.That(localFileName, Contains.Substring(".nc"));
        }
    }

    [Test]
    public void Download_Trows_ThreeFailures()
    {
        // Arrange
        const string effectiveDate = "2021-05-18";
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.AwsDownload(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), null!, ct))
            .Returns(Task.FromException(new AmazonS3Exception("Olie")));
        var testable = new SatelliteAwsSource { AmazonS3Client = null!, Ows = ows.Object, Repository = null! };
        var product = new SatelliteProductEntity { EffectiveDate = effectiveDate };

        // Act, Assert
        Assert.ThrowsAsync<AmazonS3Exception>(() => testable.Download(product, Delay, ct));
    }

    private static Task Delay(int _)
    {
        return Task.CompletedTask;
    }

    #endregion

    #region ListAwsKeys

    [Test]
    public async Task ListKeys_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.AwsList(It.IsAny<string>(), It.IsAny<string>(), null!, ct))
            .ReturnsAsync([
                "https://noaa-goes16.s3.amazonaws.com/ABI-L1b-RadC/2021/044/10/OR_ABI-L1b-RadC-M6C07_G16_s20210441036054_e20210441038438_c20210441038494.nc",
                "https://noaa-goes16.s3.amazonaws.com/ABI-L1b-RadC/2021/044/10/OR_ABI-L1b-RadC-M6C07_G16_s20210441036054_e20210441038438_c20210441038494.nc"]);
        const string dayValue = "2021-05-18";
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        const int satellite = 16;
        const int channel = 7;
        var effectiveDate = new DateTime(2021, 5, 18);
        var testable = new SatelliteAwsSource { AmazonS3Client = null!, Ows = ows.Object, Repository = null! };

        // Act
        var result = await testable.ListKeys(dayValue, satellite, channel, dayPart, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Keys, Has.Length.EqualTo(12));
    }

    [Test]
    public async Task ListKeys_ShortCircuit_OldYear()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        const string dayValue = "2017-05-18";
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        const int satellite = 16;
        const int channel = 13;
        var testable = new SatelliteAwsSource { AmazonS3Client = null!, Ows = ows.Object, Repository = null! };

        // Act
        var result = await testable.ListKeys(dayValue, satellite, channel, dayPart, ct);

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion

    #region GetBucketName

    [Test]
    public void GetBucketName_ReturnsBucket_ValidParameters()
    {
        // Arrange
        const int satellite = 16;

        // Act
        var result = SatelliteAwsSource.GetBucketName(satellite);

        // Assert
        Assert.That(result, Is.EqualTo("noaa-goes16"));
    }

    #endregion

    #region GetChannelFromAwsKey

    [Test]
    public void GetChannelFromAwsKey_ReturnsChannel_ValidKey()
    {
        // Arrange
        const string key =
            "https://noaa-goes16.s3.amazonaws.com/ABI-L1b-RadC/2021/044/10/OR_ABI-L1b-RadC-M6C07_G16_s20210441036054_e20210441038438_c20210441038494.nc";

        // Act
        var result = SatelliteAwsSource.GetChannelFromAwsKey(key);

        // Assert
        Assert.That(result, Is.EqualTo(7));
    }

    #endregion

    #region GetPrefix

    [Test]
    public void GetPrefix_ReturnsPrefix_ValidParameters()
    {
        // Arrange
        var effectiveDate = new DateTime(2021, 5, 18, 18, 0, 0);

        // Act
        var result = SatelliteAwsSource.GetPrefix(effectiveDate);

        // Assert
        Assert.That(result, Is.EqualTo("ABI-L1b-RadC/2021/138/18/"));
    }

    #endregion

    #region GetScanTime

    [Test]
    public void GetScanTime_ReturnsCreated_ValidFilename()
    {
        // Arrange
        const string filename = "OR_ABI-L1b-RadF-M3C02_G16_s20171671145342_e20171671156109_c20171671156144.nc";
        var expected = new DateTime(2017, 6, 16, 11, 45, 34, 200);

        // Act
        var result = SatelliteAwsSource.GetScanTime(filename);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion
}
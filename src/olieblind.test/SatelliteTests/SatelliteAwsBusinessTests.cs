using Amazon.S3;
using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Satellite;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Services;

namespace olieblind.test.SatelliteTests;

public class SatelliteAwsBusinessTests
{
    #region Download

    [Test]
    public async Task Download_ShortCircuit_AlreadyDownloaded()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var awsSource = new Mock<ISatelliteAwsSource>();
        var source = new Mock<ISatelliteSource>();
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteAwsBusiness(source.Object, awsSource.Object, ows.Object, repo.Object);
        var product = new SatelliteAwsProductEntity
        {
            PathSource = "a"
        };

        // Act
        await testable.Download(product, Delay, null!, null!, ct);

        // Assert
        Assert.That(product.Timestamp, Is.EqualTo(DateTime.MinValue));
    }

    [Test]
    public async Task Download_Retries_OneFailure()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        ows.SetupSequence(s => s.AwsDownload(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), null!, ct))
            .Returns(Task.FromException(new AmazonS3Exception("Olie")))
            .Returns(Task.CompletedTask);
        var awsSource = new Mock<ISatelliteAwsSource>();
        var source = new Mock<ISatelliteSource>();
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteAwsBusiness(source.Object, awsSource.Object, ows.Object, repo.Object);
        var product = new SatelliteAwsProductEntity();

        // Act
        await testable.Download(product, Delay, null!, null!, ct);

        // Assert
        Assert.That(product.PathLocal, Is.Not.Null);
    }

    [Test]
    public void Download_Trows_ThreeFailures()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.AwsDownload(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), null!, ct))
            .Returns(Task.FromException(new AmazonS3Exception("Olie")));
        var awsSource = new Mock<ISatelliteAwsSource>();
        var source = new Mock<ISatelliteSource>();
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteAwsBusiness(source.Object, awsSource.Object, ows.Object, repo.Object);
        var product = new SatelliteAwsProductEntity();

        // Act, Assert
        Assert.ThrowsAsync<AmazonS3Exception>(() => testable.Download(product, Delay, null!, null!, ct));
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
        ows.Setup(s => s.AwsList(null!, null!, null!, ct))
            .ReturnsAsync(["a", "b"]);
        const string dayValue = "2021-05-18";
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        const int satellite = 16;
        const int channel = 13;
        var effectiveDate = new DateTime(2021, 5, 18);
        var awsSource = new Mock<ISatelliteAwsSource>();
        awsSource.Setup(s => s.GetChannelFromAwsKey(It.IsAny<string>())).Returns(channel);
        var source = new Mock<ISatelliteSource>();
        source.Setup(s => s.GetEffectiveDate(dayValue)).Returns(effectiveDate);
        var testable = new SatelliteAwsBusiness(source.Object, awsSource.Object, ows.Object, null!);

        // Act
        var result = await testable.ListKeys(dayValue, satellite, channel, dayPart, null!, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Keys, Has.Length.EqualTo(2));
    }

    [Test]
    public async Task ListAwsKeysAsync_ShortCircuit_OldYear()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        const string dayValue = "2021-05-18";
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        const int satellite = 16;
        const int channel = 13;
        var awsSource = new Mock<ISatelliteAwsSource>();
        var source = new Mock<ISatelliteSource>();
        var testable = new SatelliteAwsBusiness(source.Object, awsSource.Object, ows.Object, null!);

        // Act
        var result = await testable.ListKeys(dayValue, satellite, channel, dayPart, null!, ct);

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion
}
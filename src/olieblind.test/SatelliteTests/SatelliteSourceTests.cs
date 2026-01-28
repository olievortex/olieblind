using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Satellite;
using olieblind.lib.Services;
using SixLabors.ImageSharp;

namespace olieblind.test.SatelliteTests;

public class SatelliteSourceTests
{
    #region GetMarqueeSatelliteProduct

    [Test]
    public async Task GetMarqueeSatelliteProduct_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteSource(repo.Object, null!, null!);
        var ct = CancellationToken.None;
        const string effectiveDate = "2021-05-18";
        var eventTime = new DateTime(2021, 5, 18, 18, 0, 0);
        var expected = new SatelliteProductEntity();
        repo.Setup(s => s.SatelliteProductGetPoster(effectiveDate, eventTime, ct))
            .ReturnsAsync(expected);

        // Act
        var result = await testable.GetMarqueeSatelliteProduct(effectiveDate, eventTime, ct);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion

    #region MakeThumbnail

    [Test]
    public async Task MakeThumbnail_ShortCircuit_ExistingPoster()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var ois = new Mock<IOlieImageService>();
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteSource(repo.Object, ows.Object, ois.Object);
        var satellite = new SatelliteProductEntity { PathPoster = "a" };
        var finalSize = new Point(128, 128);

        // Act
        await testable.MakeThumbnail(satellite, finalSize, null!, ct);

        // Assert
        Assert.That(satellite.Timestamp, Is.EqualTo(DateTime.MinValue));
    }

    [Test]
    public void MakeThumbnail_ThrowsException_MissingSource()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var ois = new Mock<IOlieImageService>();
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteSource(repo.Object, ows.Object, ois.Object);
        var satellite = new SatelliteProductEntity();
        var finalSize = new Point(128, 128);

        // Act, Assert
        Assert.ThrowsAsync<NullReferenceException>(() =>
            testable.MakeThumbnail(satellite, finalSize, null!, ct));
    }

    [Test]
    public async Task MakeThumbnail_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var ois = new Mock<IOlieImageService>();
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteSource(repo.Object, ows.Object, ois.Object);
        var satellite = new SatelliteProductEntity { Path1080 = "a" };
        var finalSize = new Point(128, 128);

        // Act
        await testable.MakeThumbnail(satellite, finalSize, null!, ct);

        // Assert
        Assert.That(satellite.Timestamp, Is.Not.EqualTo(DateTime.MinValue));
    }

    #endregion

    #region MessagePurple

    [Test]
    public async Task MessagePurpleAsync_ShortCircuit_Missing1080Path()
    {
        // Arrange
        var ows = new Mock<IOlieWebService>();
        var testable = new SatelliteSource(null!, ows.Object, null!);
        var satellite = new SatelliteProductEntity();
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
        var testable = new SatelliteSource(null!, ows.Object, null!);
        var satellite = new SatelliteProductEntity()
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
        var testable = new SatelliteSource(null!, ows.Object, null!);
        var satellite = new SatelliteProductEntity
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
}

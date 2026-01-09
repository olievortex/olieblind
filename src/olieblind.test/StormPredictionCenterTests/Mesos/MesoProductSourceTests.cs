using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Services;
using olieblind.lib.StormPredictionCenter.Mesos;
using System.Net;

namespace olieblind.test.StormPredictionCenterTests.Mesos;

public class MesoProductSourceTests
{
    #region DownloadHtml

    [Test]
    public void DownloadHtml_ThrowsException_NotContent()
    {
        // Arrange
        const int year = 2021;
        const int index = 56;
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.ApiGet(It.IsAny<string>(), null, ct))
            .ReturnsAsync((HttpStatusCode.NoContent, null, string.Empty));
        var cosmos = new Mock<IMyRepository>();
        var ois = new Mock<IOlieImageService>();
        var testable = new MesoProductSource(ows.Object, cosmos.Object, ois.Object);

        // Act, Assert
        Assert.ThrowsAsync<ApplicationException>(() => testable.DownloadHtml(year, index, ct));
    }

    [Test]
    public async Task DownloadHtml_Null_NotFound()
    {
        // Arrange
        const int year = 2021;
        const int index = 56;
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.ApiGet(It.IsAny<string>(), null, ct))
            .ReturnsAsync((HttpStatusCode.NotFound, null, string.Empty));
        var cosmos = new Mock<IMyRepository>();
        var ois = new Mock<IOlieImageService>();
        var testable = new MesoProductSource(ows.Object, cosmos.Object, ois.Object);

        // Act
        var result = await testable.DownloadHtml(year, index, ct);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DownloadHtml_Completes_Ok()
    {
        // Arrange
        const int year = 2021;
        const int index = 56;
        var ct = CancellationToken.None;
        var html = Guid.NewGuid().ToString();
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.ApiGet(It.IsAny<string>(), null, ct))
            .ReturnsAsync((HttpStatusCode.OK, null, html));
        var cosmos = new Mock<IMyRepository>();
        var ois = new Mock<IOlieImageService>();
        var testable = new MesoProductSource(ows.Object, cosmos.Object, ois.Object);

        // Act
        var result = await testable.DownloadHtml(year, index, ct);

        // Assert
        Assert.That(result, Is.EqualTo(html));
    }

    #endregion

    #region StoreImage

    [Test]
    public async Task StoreImage_ShortCircuits_AlreadyDownloaded()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var cosmos = new Mock<IMyRepository>();
        var ois = new Mock<IOlieImageService>();
        var testable = new MesoProductSource(ows.Object, cosmos.Object, ois.Object);
        var product = new SpcMesoProductEntity
        {
            GraphicUrl = Guid.NewGuid().ToString()
        };

        // Act
        var result = await testable.StoreImage(string.Empty, product, string.Empty, ct);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task StoreImage_Completes_NotDownloaded()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var cosmos = new Mock<IMyRepository>();
        var ois = new Mock<IOlieImageService>();
        ois.Setup(s => s.SafeConvert(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), ct))
            .ReturnsAsync(expected);
        var testable = new MesoProductSource(ows.Object, cosmos.Object, ois.Object);
        var product = new SpcMesoProductEntity
        {
            EffectiveTime = new DateTime(2021, 7, 18)
        };

        // Act
        var result = await testable.StoreImage(string.Empty, product, "meow", ct);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion

    #region StoreHtml

    [Test]
    public async Task StoreImage_UploadsBlob_ValidParameters()
    {
        // Arrange
        const int index = 42;
        var html = Guid.NewGuid().ToString();
        var effectiveTime = new DateTime(2021, 7, 18);
        var expected = "bronze/spc/meso/2021/7/md0042.html";
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var cosmos = new Mock<IMyRepository>();
        var ois = new Mock<IOlieImageService>();
        var testable = new MesoProductSource(ows.Object, cosmos.Object, ois.Object);

        // Act
        var result = await testable.StoreHtml(index, html, effectiveTime, null!, ct);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion

    #region GetLatestIdForYear

    [Test]
    public async Task GetLatestIdForYear_CompletesSteps_ValidParameters()
    {
        // Arrange
        const int year = 2023;
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var cosmos = new Mock<IMyRepository>();
        var ois = new Mock<OlieImageService>();
        var testable = new MesoProductSource(ows.Object, cosmos.Object, ois.Object);
        var entity = new SpcMesoProductEntity { Id = 5 };
        cosmos.Setup(s => s.SpcMesoProductGetLatest(year, ct))
            .ReturnsAsync(entity);

        // Act
        var result = await testable.GetLatestIdForYear(year, ct);

        // Assert
        Assert.That(result, Is.EqualTo(5));
    }

    [Test]
    public async Task GetLatestIdForYear_Zero_NoRecord()
    {
        // Arrange
        const int year = 2023;
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var cosmos = new Mock<IMyRepository>();
        var ois = new Mock<IOlieImageService>();
        var testable = new MesoProductSource(ows.Object, cosmos.Object, ois.Object);

        // Act
        var result = await testable.GetLatestIdForYear(year, ct);

        // Assert
        Assert.That(result, Is.Zero);
    }

    #endregion
}
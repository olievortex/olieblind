using System.Net;
using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Services;
using olieblind.lib.StormPredictionCenter.Mesos;

namespace olieblind.test.StormPredictionCenterTests.Mesos;

public class MesoProductSourceTests
{
    #region DownloadHtml

    [Test]
    public void DownloadHtmlAsync_ThrowsException_NotContent()
    {
        // Arrange
        const int year = 2021;
        const int index = 56;
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.ApiGet(It.IsAny<string>(), null, ct))
            .ReturnsAsync((HttpStatusCode.NoContent, null, string.Empty));
        var cosmos = new Mock<IMyRepository>();
        var testable = new MesoProductSource(ows.Object, cosmos.Object);

        // Act, Assert
        Assert.ThrowsAsync<ApplicationException>(() => testable.DownloadHtml(year, index, ct));
    }

    [Test]
    public async Task DownloadHtmlAsync_Null_NotFound()
    {
        // Arrange
        const int year = 2021;
        const int index = 56;
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.ApiGet(It.IsAny<string>(), null, ct))
            .ReturnsAsync((HttpStatusCode.NotFound, null, string.Empty));
        var cosmos = new Mock<IMyRepository>();
        var testable = new MesoProductSource(ows.Object, cosmos.Object);

        // Act
        var result = await testable.DownloadHtml(year, index, ct);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DownloadHtmlAsync_Completes_Ok()
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
        var testable = new MesoProductSource(ows.Object, cosmos.Object);

        // Act
        var result = await testable.DownloadHtml(year, index, ct);

        // Assert
        Assert.That(result, Is.EqualTo(html));
    }

    #endregion

    #region DownloadImage

    [Test]
    public async Task DownloadImageAsync_ShortCircuits_AlreadyDownloaded()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var cosmos = new Mock<IMyRepository>();
        var testable = new MesoProductSource(ows.Object, cosmos.Object);
        var product = new SpcMesoProductEntity
        {
            GraphicUrl = Guid.NewGuid().ToString()
        };

        // Act
        await testable.DownloadImage(string.Empty, product, null!, ct);

        // Assert
        cosmos.Verify(v => v.SpcMesoProductUpdate(product, ct), Times.Never);
    }

    [Test]
    public async Task DownloadImageAsync_Completes_NotDownloaded()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var cosmos = new Mock<IMyRepository>();
        var testable = new MesoProductSource(ows.Object, cosmos.Object);
        var product = new SpcMesoProductEntity
        {
            EffectiveTime = new DateTime(2021, 7, 18)
        };

        // Act
        await testable.DownloadImage(string.Empty, product, null!, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(product.GraphicUrl, Is.Not.Null);
            Assert.That(product.Timestamp, Is.Not.EqualTo(DateTime.MinValue));
        }
    }

    #endregion

    #region GetLatestIdForYear

    [Test]
    public async Task GetLatestIdForYearAsync_CompletesSteps_ValidParameters()
    {
        // Arrange
        const int year = 2023;
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var cosmos = new Mock<IMyRepository>();
        var testable = new MesoProductSource(ows.Object, cosmos.Object);
        var entity = new SpcMesoProductEntity { Id = 5 };
        cosmos.Setup(s => s.SpcMesoProductGetLatest(year, ct))
            .ReturnsAsync(entity);

        // Act
        var result = await testable.GetLatestIdForYear(year, ct);

        // Assert
        Assert.That(result, Is.EqualTo(5));
    }

    [Test]
    public async Task GetLatestIdForYearAsync_Zero_NoRecord()
    {
        // Arrange
        const int year = 2023;
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var cosmos = new Mock<IMyRepository>();
        var testable = new MesoProductSource(ows.Object, cosmos.Object);

        // Act
        var result = await testable.GetLatestIdForYear(year, ct);

        // Assert
        Assert.That(result, Is.Zero);
    }

    #endregion
}
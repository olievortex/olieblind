using Moq;
using olieblind.lib.Services;
using olieblind.lib.StormPredictionCenter.Models;
using olieblind.lib.StormPredictionCenter.Outlooks;
using System.Drawing;

namespace olieblind.test.StormPredictionCenterTests;

public class OutlookProductTests
{
    [Test]
    public async Task GetCurrentIndexAsync_ReturnsTempFilename_ValidUrl()
    {
        // Arrange
        const string body = $"Too many tornadoes";
        const int dayNumber = 1;
        var ois = new Mock<IOlieImageService>();
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.ApiGetString(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(body);
        var sdo = new OutlookProduct(ows.Object, ois.Object);

        // Act
        var result = await sdo.GetCurrentIndexAsync(dayNumber, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(body));
    }

    [Test]
    public async Task GetCurrentImageAsync_ReturnsBytes_ValidUrl()
    {
        // Arrange
        var ois = new Mock<IOlieImageService>();
        var ows = new Mock<IOlieWebService>();
        var sdo = new OutlookProduct(ows.Object, ois.Object);

        // Act
        var result = await sdo.GetCurrentImageAsync(string.Empty, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task AddBrandingToImageAsync_ReturnsBytes_ValidInput()
    {
        // Arrange
        var finalSize = new Point(1280, 720);
        var ois = new Mock<IOlieImageService>();
        var ows = new Mock<IOlieWebService>();
        var sdo = new OutlookProduct(ows.Object, ois.Object);
        var branding = new OutlookBrandingModel("Dillon", new Point(), null!, 128, 1);
        var image = new byte[1024];

        // Act
        var result = await sdo.AddBrandingToImageAsync(branding, image, finalSize, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    #region GetBrandingByImageName

    [Test]
    public void GetBrandingByImageName_ReturnsCategories_CategoryImage()
    {
        // Arrange
        const string imageName = "day1otlk_1630.gif";
        var ois = new Mock<IOlieImageService>();
        var ows = new Mock<IOlieWebService>();
        var sdo = new OutlookProduct(ows.Object, ois.Object);

        // Act
        var result = sdo.GetBrandingByImageName(imageName, string.Empty, new Point());

        // Assert
        Assert.That(result.Category, Is.EqualTo("Categories"));
    }

    [Test]
    public void GetBrandingByImageName_ReturnsProbability_ProbabilityImage()
    {
        // Arrange
        const string imageName = "day1prob_1630.gif";
        var ois = new Mock<IOlieImageService>();
        var ows = new Mock<IOlieWebService>();
        var sdo = new OutlookProduct(ows.Object, ois.Object);

        // Act
        var result = sdo.GetBrandingByImageName(imageName, string.Empty, new Point());

        // Assert
        Assert.That(result.Category, Is.EqualTo("Probability"));
    }

    [Test]
    public void GetBrandingByImageName_ReturnsTornado_TornadoImage()
    {
        // Arrange
        const string imageName = "day1probotlk_1630_torn.gif";
        var ois = new Mock<IOlieImageService>();
        var ows = new Mock<IOlieWebService>();
        var sdo = new OutlookProduct(ows.Object, ois.Object);

        // Act
        var result = sdo.GetBrandingByImageName(imageName, string.Empty, new Point());

        // Assert
        Assert.That(result.Category, Is.EqualTo("Tornado"));
    }

    [Test]
    public void GetBrandingByImageName_ReturnsWind_WindImage()
    {
        // Arrange
        const string imageName = "day1probotlk_1630_wind.gif";
        var ois = new Mock<IOlieImageService>();
        var ows = new Mock<IOlieWebService>();
        var sdo = new OutlookProduct(ows.Object, ois.Object);

        // Act
        var result = sdo.GetBrandingByImageName(imageName, string.Empty, new Point());

        // Assert
        Assert.That(result.Category, Is.EqualTo("Wind"));
    }

    [Test]
    public void GetBrandingByImageName_ReturnsHail_HailImage()
    {
        // Arrange
        const string imageName = "day1probotlk_1630_hail.gif";
        var ois = new Mock<IOlieImageService>();
        var ows = new Mock<IOlieWebService>();
        var sdo = new OutlookProduct(ows.Object, ois.Object);

        // Act
        var result = sdo.GetBrandingByImageName(imageName, string.Empty, new Point());

        // Assert
        Assert.That(result.Category, Is.EqualTo("Hail"));
    }

    [Test]
    public void GetBrandingByImageName_ThrowsException_UnknownImage()
    {
        // Arrange
        const string imageName = "day1probotlk_1630_ufo.gif";
        var ois = new Mock<IOlieImageService>();
        var ows = new Mock<IOlieWebService>();
        var sdo = new OutlookProduct(ows.Object, ois.Object);

        // Act, Assert
        Assert.Throws<InvalidOperationException>(() =>
            sdo.GetBrandingByImageName(imageName, string.Empty, new Point()));
    }

    #endregion
}
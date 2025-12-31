using Moq;
using olieblind.lib.DroughtMonitor;
using olieblind.lib.Services;

namespace olieblind.test.DroughtMonitorTests;

public class DroughtMonitorTests
{
    private readonly string _xml = File.ReadAllText("./Resources/DroughtMonitor/usdm_summary_20250114.xml");

    [Test]
    public async Task GetCurrentDroughtMonitorXmlAsync_ReturnsXml_Always()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.ApiGetString(It.IsAny<string>(), ct))
            .ReturnsAsync(_xml);
        var testable = new DroughtMonitor(ows.Object);

        // Act
        var result = await testable.GetCurrentDroughtMonitorXmlAsync(ct);

        // Assert
        Assert.That(result, Is.EqualTo(_xml));
    }

    [Test]
    public void GetProductFromXml_ReturnsProduct_ValidXml()
    {
        // Arrange
        var testable = new DroughtMonitor(null!);

        // Act
        var result = testable.GetProductFromXml(_xml);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.EffectiveDate, Is.EqualTo(new DateTime(2025, 01, 16, 0, 0, 0, DateTimeKind.Utc)));
            Assert.That(result.Intro, Does.StartWith("On January 9 and 10, a low pressure system"));
            Assert.That(result.Forecast, Does.StartWith("Another Arctic air outbreak is forecast for"));
            Assert.That(result.Regions, Has.Count.EqualTo(8));
        }
    }

    [Test]
    public void GetImageNames_ReturnsList_Always()
    {
        // Arrange
        var testable = new DroughtMonitor(null!);

        // Act
        var result = testable.GetImageNames();

        // Assert
        Assert.That(result, Has.Count.EqualTo(7));
    }

    [Test]
    public void CorrectXmlFormatting_CorrectsXml_Always()
    {
        // Arrange
        var testable = new DroughtMonitor(null!);
        var badXml = "<root><value>5 & 10 <1 <2 inches</value></root>";
        var expectedXml = "<root><value>5 &amp; 10 &lt;1 &lt;2 inches</value></root>";

        // Act
        var result = testable.CorrectXmlFormatting(badXml);

        // Assert
        Assert.That(result, Is.EqualTo(expectedXml));
    }
}
using olieblind.lib.DroughtMonitor;
using System.Xml;

namespace olieblind.test.DroughtMonitorTests;

public class DroughtMonitorParsingTests
{
    private readonly string _xml = File.ReadAllText("./Resources/DroughtMonitor/usdm_summary_20250114.xml");

    #region GetWeekXmlElement tests

    [Test]
    public void GetWeekXmlElement_ThrowsException_MissingResults()
    {
        // Arrange
        var xmlDoc = DroughtMonitorParsing.ParseXml("<dillon><p>This is me</p></dillon>");

        // Act, Assert
        Assert.Throws<Exception>(() => DroughtMonitorParsing.GetWeekXmlElement(xmlDoc));
    }

    [Test]
    public void GetWeekXmlElement_ThrowsException_MissingWeek()
    {
        // Arrange
        var xmlDoc = DroughtMonitorParsing.ParseXml("<Results><p>This is me</p></Results>");

        // Act, Assert
        Assert.Throws<Exception>(() => DroughtMonitorParsing.GetWeekXmlElement(xmlDoc));
    }

    #endregion

    #region GetDate tests

    [Test]
    public void GetDate_ReturnsDate_ValidXml()
    {
        // Arrange
        var xmlDoc = DroughtMonitorParsing.ParseXml(_xml);
        var weekElement = DroughtMonitorParsing.GetWeekXmlElement(xmlDoc);

        // Act
        var result = DroughtMonitorParsing.GetDate(weekElement);

        // Assert
        Assert.That(result, Is.EqualTo(new DateTime(2025, 01, 16, 0, 0, 0, DateTimeKind.Utc)));
    }

    [Test]
    public void GetDate_ThrowsException_MissingDate()
    {
        // Arrange
        var xmlDoc = DroughtMonitorParsing.ParseXml(_xml.Replace("date>", "dillonDate>"));
        var weekElement = DroughtMonitorParsing.GetWeekXmlElement(xmlDoc);

        // Act, Assert
        Assert.Throws<Exception>(() => DroughtMonitorParsing.GetDate(weekElement));
    }

    #endregion

    [Test]
    public void GetPageName_ReturnsPageName_ValidDate()
    {
        // Arrange
        const string date = "20250116";

        // Act
        var result = DroughtMonitorParsing.GetPageName(date);

        // Assert
        Assert.That(result, Is.EqualTo("usdm_summary_20250116.xml"));
    }

    [Test]
    public void GetPageDate_ReturnsPageDate_ValidDate()
    {
        // Arrange
        var date = new DateTime(2025, 01, 16, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var result = DroughtMonitorParsing.GetPageDate(date);

        // Assert
        Assert.That(result, Is.EqualTo("20250116"));
    }

    #region GetInnerXml tests

    [Test]
    public void GetInnerXml_ReturnsText_ValidXml()
    {
        // Arrange
        const string xml = "<week><intro><p>I'm here</p></intro></week>";
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);
        var xmlElement = xmlDoc["week"]!;

        // Act
        var result = DroughtMonitorParsing.GetInnerXml("intro", xmlElement);

        // Assert
        Assert.That(result, Is.EqualTo("<p>I'm here</p>"));
    }

    [Test]
    public void GetInnerXml_ThrowsException_MissingTag()
    {
        // Arrange
        const string xml = "<week><intro><p>I'm here</p></intro></week>";
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);
        var xmlElement = xmlDoc["week"]!;

        // Act, Assert
        Assert.Throws<Exception>(() => DroughtMonitorParsing.GetInnerXml("viking", xmlElement));
    }

    #endregion

    [Test]
    public void ParagraphsToLineFeeds_ReturnsTwoLine_TwoParagraphs()
    {
        // Arrange
        const string xml = "  <p>Dillon</p>\n  <p>Horse cow</p> ";

        // Act
        var result = DroughtMonitorParsing.ParagraphsToLineFeeds(xml);

        // Assert
        Assert.That(result, Is.EqualTo("Dillon\nHorse cow"));
    }

    #region GetLastTuesdayDayOffset Tests

    [Test]
    public void GetLastTuesdayDayOffset_ReturnsTuesday_Monday()
    {
        // Arrange
        var date = new DateTime(2025, 01, 27, 17, 39, 32, DateTimeKind.Utc);

        // Act
        var result = DroughtMonitorParsing.GetLastTuesdayDayOffset(date);

        // Assert
        Assert.That(result, Is.EqualTo(-6));
    }

    [Test]
    public void GetLastTuesdayDayOffset_ReturnsTuesday_Tuesday()
    {
        // Arrange
        var date = new DateTime(2025, 01, 28, 17, 39, 32, DateTimeKind.Utc);

        // Act
        var result = DroughtMonitorParsing.GetLastTuesdayDayOffset(date);

        // Assert
        Assert.That(result, Is.EqualTo(-7));
    }

    [Test]
    public void GetLastTuesdayDayOffset_ReturnsTuesday_Wednesday()
    {
        // Arrange
        var date = new DateTime(2025, 01, 29, 17, 39, 32, DateTimeKind.Utc);

        // Act
        var result = DroughtMonitorParsing.GetLastTuesdayDayOffset(date);

        // Assert
        Assert.That(result, Is.EqualTo(-8));
    }

    [Test]
    public void GetLastTuesdayDayOffset_ReturnsTuesday_Thursday()
    {
        // Arrange
        var date = new DateTime(2025, 01, 23, 17, 39, 32, DateTimeKind.Utc);

        // Act
        var result = DroughtMonitorParsing.GetLastTuesdayDayOffset(date);

        // Assert
        Assert.That(result, Is.EqualTo(-2));
    }

    [Test]
    public void GetLastTuesdayDayOffset_ReturnsTuesday_Friday()
    {
        // Arrange
        var date = new DateTime(2025, 01, 24, 17, 39, 32, DateTimeKind.Utc);

        // Act
        var result = DroughtMonitorParsing.GetLastTuesdayDayOffset(date);

        // Assert
        Assert.That(result, Is.EqualTo(-3));
    }

    [Test]
    public void GetLastTuesdayDayOffset_ReturnsTuesday_Saturday()
    {
        // Arrange
        var date = new DateTime(2025, 01, 25, 17, 39, 32, DateTimeKind.Utc);

        // Act
        var result = DroughtMonitorParsing.GetLastTuesdayDayOffset(date);

        // Assert
        Assert.That(result, Is.EqualTo(-4));
    }

    [Test]
    public void GetLastTuesdayDayOffset_ReturnsTuesday_Sunday()
    {
        // Arrange
        var date = new DateTime(2025, 01, 26, 17, 39, 32, DateTimeKind.Utc);

        // Act
        var result = DroughtMonitorParsing.GetLastTuesdayDayOffset(date);

        // Assert
        Assert.That(result, Is.EqualTo(-5));
    }

    #endregion

    #region GetInnerXmlDict tests

    [Test]
    public void GetInnerXmlDict_ReturnsDictionary_ValidInput()
    {
        // Arrange
        var xmlDoc = DroughtMonitorParsing.ParseXml(_xml);
        var week = DroughtMonitorParsing.GetWeekXmlElement(xmlDoc);

        // Act
        var result = DroughtMonitorParsing.GetInnerXmlDict("region", "name", week, s => s);

        // Assert
        Assert.That(result, Has.Count.EqualTo(8));
    }

    [Test]
    public void GetInnerXmlDict_ThrowsException_InvalidAttributeName()
    {
        // Arrange
        var xmlDoc = DroughtMonitorParsing.ParseXml(_xml);
        var week = DroughtMonitorParsing.GetWeekXmlElement(xmlDoc);

        // Act, Assert
        Assert.Throws<Exception>(() =>
            DroughtMonitorParsing.GetInnerXmlDict("region", "dillon", week, null!));
    }

    #endregion
}
using olieblind.lib.StormPredictionCenter.Outlooks;

namespace olieblind.test.StormPredictionCenterTests.OutlookProductParsingTests.Examples;

public class Case24Nov171630Tests
{
    private readonly string _html = File.ReadAllText("./Resources/SpcDayOne/24Nov17_1630.htm");
    private const int DayNumber = 1;

    [Test]
    public void ParseNarrative_ReturnsModel_24Nov171630()
    {
        var parsing = new OutlookProductParsing();
        var narrative = parsing.ExtractNarrativeFromHtml(_html);
        var result = parsing.ParseNarrative(narrative, DayNumber);

        Assert.Multiple(() =>
        {
            Assert.That(result.EffectiveDate, Is.EqualTo(new DateTime(2024, 11, 17, 16, 30, 0)));
            Assert.That(result.ProductName, Is.EqualTo("Day 1 Convective Outlook"));
            Assert.That(result.Headline, Is.EqualTo(
                "THERE IS A SLIGHT RISK OF SEVERE THUNDERSTORMS ACROSS PARTS OF THE SOUTHERN PLAINS"));
            Assert.That(result.Headings, Has.Count.EqualTo(3));
            Assert.That(result.Headings[0], Does.Match("SUMMARY"));
            Assert.That(result.Headings[1], Does.Match("Synopsis"));
            Assert.That(result.Headings[2], Does.Match("Southern Great Plains"));
            Assert.That(result.Paragraphs, Has.Count.EqualTo(3));
            Assert.That(result.Paragraphs[0], Does.StartWith("Scattered severe thunderstorms are expected"));
            Assert.That(result.Paragraphs[0], Does.EndWith("severe wind gusts and a few tornadoes."));
            Assert.That(result.Paragraphs[1], Does.StartWith("Water-vapor imagery this morning shows a potent"));
            Assert.That(result.Paragraphs[1], Does.EndWith("will advance northward from north TX into OK late."));
            Assert.That(result.Paragraphs[2], Does.StartWith("The 12 UTC Fort Worth, TX observed sounding"));
            Assert.That(result.Paragraphs[2], Does.EndWith("TX northeastward into southwest OK late."));
        });
    }

    [Test]
    public void ExtractImageNamesFromHtml_ReturnsImageNames_24Nov171630()
    {
        var parsing = new OutlookProductParsing();
        var result = parsing.ExtractImageNamesFromHtml(_html, DayNumber);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(4));
            Assert.That(result[0], Is.EqualTo("day1otlk_1630.gif"));
            Assert.That(result[1], Is.EqualTo("day1probotlk_1630_torn.gif"));
            Assert.That(result[2], Is.EqualTo("day1probotlk_1630_wind.gif"));
            Assert.That(result[3], Is.EqualTo("day1probotlk_1630_hail.gif"));
        });
    }

    [Test]
    public void CreateHeadline_ReturnsHeadline_24Nov171630()
    {
        var parsing = new OutlookProductParsing();
        var scripting = new OutlookProductScript();
        var narrative = parsing.ExtractNarrativeFromHtml(_html);
        var product = parsing.ParseNarrative(narrative, DayNumber);
        var result = scripting.CreateHeadline(product, DayNumber);

        Assert.That(result, Does.StartWith("10AM Day 1 Convective Outlook for Sunday, November 17."));
    }

    [Test]
    public void CreateDefaultScript_CorrectsHeadings_24Nov171630()
    {
        // Arrange
        var parsing = new OutlookProductParsing();
        var narrative = parsing.ExtractNarrativeFromHtml(_html);
        var model = parsing.ParseNarrative(narrative, DayNumber);
        var testable = new OutlookProductScript();

        // Act
        var script = testable.CreateDefaultScript(model, DayNumber);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(script, Does.Contain("of 12.6 grams per kilogram. The"));
            Assert.That(script, Does.Contain("As an intense 100-knots 500-millibar speed max"));
            Assert.That(script, Does.Contain("indicates 250 to 1000 jewels per kilogram mixed layer cape across"));
            Assert.That(script, Does.Contain("intensifying low level jet develops tonight"));
            Assert.That(script, Does.Contain("near the eye-35 corridor from"));
            Assert.That(script, Does.Not.Contain("SUMMARY"));
        });
    }
}
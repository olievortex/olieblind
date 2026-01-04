using olieblind.lib.StormPredictionCenter.Outlooks;

namespace olieblind.test.StormPredictionCenterTests.Outlooks.OutlookProductParsingTests.Examples;

public class Case03Mar031200Tests
{
    private readonly string _html = File.ReadAllText("./Resources/SpcDayOne/03Mar03_1200.htm");
    private const int DayNumber = 1;

    [Test]
    public void ParseNarrative_ReturnsModel_03Mar031200()
    {
        // Arrange
        var parsing = new OutlookProductParsing();
        var narrative = parsing.ExtractNarrativeFromHtml(_html);

        // Act
        var result = parsing.ParseNarrative(narrative, DayNumber);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.EffectiveDate, Is.EqualTo(new DateTime(2025, 03, 03, 6, 0, 0)));
            Assert.That(result.ProductName, Is.EqualTo("Day 1 Convective Outlook"));
            Assert.That(result.Headline, Is.EqualTo(
                "THERE IS A SLIGHT RISK OF SEVERE THUNDERSTORMS ACROSS PORTIONS OF CENTRAL OKLAHOMA INTO NORTHERN TEXAS"));
            Assert.That(result.Headings, Has.Count.EqualTo(4));
            Assert.That(result.Headings[0], Does.Match("SUMMARY"));
            Assert.That(result.Headings[1], Does.Match("Synopsis"));
            Assert.That(result.Headings[2], Does.Match("Central and southern Plains this evening into tonight"));
            Assert.That(result.Headings[3], Is.Empty);
            Assert.That(result.Paragraphs, Has.Count.EqualTo(4));
            Assert.That(result.Paragraphs[0], Does.StartWith("Thunderstorms should increase this evening"));
            Assert.That(result.Paragraphs[0], Does.EndWith("tornadoes are possible with the stronger storms."));
            Assert.That(result.Paragraphs[1], Does.StartWith("A pronounced mid-level trough will traverse the"));
            Assert.That(result.Paragraphs[1], Does.EndWith("potential, with all severe hazards possible."));
            Assert.That(result.Paragraphs[2], Does.StartWith("A southerly 850 mb jet increases to well over"));
            Assert.That(result.Paragraphs[2], Does.EndWith("primary concern with squall line development."));
            Assert.That(result.Paragraphs[3], Does.StartWith("Overnight, low 60s F surface dewpoints will"));
            Assert.That(result.Paragraphs[3], Does.EndWith("boundary-layer buoyancy becomes apparent."));
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

        Assert.That(result, Does.StartWith("12AM Day 1 Convective Outlook for Monday, March 3."));
    }
}
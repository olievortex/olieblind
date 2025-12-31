using olieblind.lib.StormPredictionCenter.Outlooks;

namespace olieblind.test.StormPredictionCenterTests.OutlookProductParsingTests.Examples;

public class Case24Nov210100Tests
{
    private readonly string _html = File.ReadAllText("./Resources/SpcDayOne/24Nov21_0100.htm");
    private const int DayNumber = 1;

    [Test]
    public void ParseNarrative_ReturnsModel_24Nov210100()
    {
        var parsing = new OutlookProductParsing();
        var narrative = parsing.ExtractNarrativeFromHtml(_html);
        var result = parsing.ParseNarrative(narrative, DayNumber);

        Assert.Multiple(() =>
        {
            Assert.That(result.EffectiveDate, Is.EqualTo(new DateTime(2024, 11, 21, 5, 18, 0)));
            Assert.That(result.ProductName, Is.EqualTo("Day 1 Convective Outlook"));
            Assert.That(result.Headline,
                Does.Match("NO SEVERE THUNDERSTORM AREAS FORECAST"));
            Assert.That(result.Headings, Has.Count.EqualTo(2));
            Assert.That(result.Headings[0], Does.Match("SUMMARY"));
            Assert.That(result.Headings[1], Does.Match("Southern New England"));
            Assert.That(result.Paragraphs, Has.Count.EqualTo(2));
            Assert.That(result.Paragraphs[0], Does.StartWith("A few thunderstorms are possible"));
            Assert.That(result.Paragraphs[0], Does.EndWith("across southern New England Thursday morning."));
            Assert.That(result.Paragraphs[1], Does.StartWith("Strong upper low will shift southeast from the"));
            Assert.That(result.Paragraphs[1], Does.EndWith("Thereafter, deepest convection will focus offshore."));
        });
    }

    [Test]
    public void ExtractImageNamesFromHtml_ReturnsImageNames_24Nov210100()
    {
        var parsing = new OutlookProductParsing();
        var result = parsing.ExtractImageNamesFromHtml(_html, DayNumber);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(4));
            Assert.That(result[0], Is.EqualTo("day1otlk_1200.gif"));
            Assert.That(result[1], Is.EqualTo("day1probotlk_1200_torn.gif"));
            Assert.That(result[2], Is.EqualTo("day1probotlk_1200_wind.gif"));
            Assert.That(result[3], Is.EqualTo("day1probotlk_1200_hail.gif"));
        });
    }

    [Test]
    public void CreateHeadline_ReturnsHeadline_24Nov210100()
    {
        var parsing = new OutlookProductParsing();
        var scripting = new OutlookProductScript();
        var narrative = parsing.ExtractNarrativeFromHtml(_html);
        var product = parsing.ParseNarrative(narrative, DayNumber);
        var result = scripting.CreateHeadline(product, DayNumber);

        Assert.That(result, Does.StartWith("11PM Day 1 Convective Outlook for Thursday, November 21."));
    }
}
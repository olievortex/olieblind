using olieblind.lib.StormPredictionCenter.Outlooks;

namespace olieblind.test.StormPredictionCenterTests.Outlooks.OutlookProductParsingTests.Examples;

public class Case24Nov241300Tests
{
    private readonly string _html = File.ReadAllText("./Resources/SpcDayOne/24Nov24_1300.htm");
    private const int DayNumber = 1;

    [Test]
    public void ParseNarrative_ReturnsModel_24Nov241300()
    {
        var parsing = new OutlookProductParsing();
        var narrative = parsing.ExtractNarrativeFromHtml(_html);
        var result = parsing.ParseNarrative(narrative, DayNumber);

        Assert.Multiple(() =>
        {
            Assert.That(result.EffectiveDate, Is.EqualTo(new DateTime(2024, 11, 24, 12, 31, 0)));
            Assert.That(result.ProductName, Is.EqualTo("Day 1 Convective Outlook"));
            Assert.That(result.Headline,
                Does.Match("NO SEVERE THUNDERSTORM AREAS FORECAST"));
            Assert.That(result.Headings, Has.Count.EqualTo(3));
            Assert.That(result.Headings[0], Does.Match("SUMMARY"));
            Assert.That(result.Headings[1], Does.Match("Synopsis/Discussion"));
            Assert.That(result.Headings[2], Does.Match(string.Empty));
            Assert.That(result.Paragraphs, Has.Count.EqualTo(3));
            Assert.That(result.Paragraphs[0], Is.EqualTo("Severe thunderstorms are unlikely through tonight."));
            Assert.That(result.Paragraphs[1], Does.StartWith("In mid/upper levels, a belt of relatively strong"));
            Assert.That(result.Paragraphs[1], Does.EndWith("dissipating in more-stable low-level conditions."));
            Assert.That(result.Paragraphs[2], Does.StartWith("A shortwave trough -- evident in moisture-channel"));
            Assert.That(result.Paragraphs[2], Does.EndWith("strongly capped for an areal thunderstorm threat."));
        });
    }

    [Test]
    public void ExtractImageNamesFromHtml_ReturnsImageNames_24Nov241300()
    {
        var parsing = new OutlookProductParsing();
        var result = parsing.ExtractImageNamesFromHtml(_html, DayNumber);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(4));
            Assert.That(result[0], Is.EqualTo("day1otlk_1300.gif"));
            Assert.That(result[1], Is.EqualTo("day1probotlk_1300_torn.gif"));
            Assert.That(result[2], Is.EqualTo("day1probotlk_1300_wind.gif"));
            Assert.That(result[3], Is.EqualTo("day1probotlk_1300_hail.gif"));
        });
    }

    [Test]
    public void CreateHeadline_ReturnsHeadline_24Nov241300()
    {
        var parsing = new OutlookProductParsing();
        var scripting = new OutlookProductScript();
        var narrative = parsing.ExtractNarrativeFromHtml(_html);
        var product = parsing.ParseNarrative(narrative, DayNumber);
        var result = scripting.CreateHeadline(product, DayNumber);

        Assert.That(result, Does.StartWith("6AM Day 1 Convective Outlook for Sunday, November 24."));
    }
}
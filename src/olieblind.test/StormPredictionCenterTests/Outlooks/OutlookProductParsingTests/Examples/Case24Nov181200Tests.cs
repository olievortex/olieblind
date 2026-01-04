using olieblind.lib.StormPredictionCenter.Outlooks;

namespace olieblind.test.StormPredictionCenterTests.Outlooks.OutlookProductParsingTests.Examples;

public class Case24Nov181200Tests
{
    private readonly string _html = File.ReadAllText("./Resources/SpcDayOne/24Nov18_1200.htm");
    private const int DayNumber = 1;

    [Test]
    public void ParseNarrative_ReturnsModel_24Nov181200()
    {
        var parsing = new OutlookProductParsing();
        var narrative = parsing.ExtractNarrativeFromHtml(_html);
        var result = parsing.ParseNarrative(narrative, DayNumber);

        Assert.Multiple(() =>
        {
            Assert.That(result.EffectiveDate, Is.EqualTo(new DateTime(2024, 11, 18, 12, 59, 0)));
            Assert.That(result.ProductName, Is.EqualTo("Day 1 Convective Outlook"));
            Assert.That(result.Headline,
                Does.Match(
                    "THERE IS A SLIGHT RISK OF SEVERE THUNDERSTORMS ACROSS PARTS OF THE SOUTHERN PLAINS AND FROM PORTIONS OF EAST TEXAS INTO LOUISIANA AND FAR SOUTHERN ARKANSAS"));
            Assert.That(result.Headings, Has.Count.EqualTo(5));
            Assert.That(result.Headings[0], Does.Match("SUMMARY"));
            Assert.That(result.Headings[1], Does.Match("Synopsis"));
            Assert.That(result.Headings[2], Does.Match("Southern Plains"));
            Assert.That(result.Headings[3], Does.Match("East Texas into the Lower Mississippi Valley"));
            Assert.That(result.Headings[4], Does.Match("Central Plains/Ozarks into the Mid Missouri Valley"));
            Assert.That(result.Paragraphs, Has.Count.EqualTo(5));
            Assert.That(result.Paragraphs[0], Does.StartWith("Thunderstorms may continue to produce"));
            Assert.That(result.Paragraphs[0], Does.EndWith("east Texas into Louisiana and vicinity."));
            Assert.That(result.Paragraphs[1], Does.StartWith("A negatively tilted shortwave trough over the"));
            Assert.That(result.Paragraphs[1], Does.EndWith("Valley late tonight into early Tuesday morning."));
            Assert.That(result.Paragraphs[2], Does.StartWith("With low-level winds remaining very strong per area"));
            Assert.That(result.Paragraphs[2], Does.EndWith("becomes increasingly marginal with eastward extent."));
            Assert.That(result.Paragraphs[3], Does.StartWith("The southern portion of the squall line along/ahead"));
            Assert.That(result.Paragraphs[3], Does.EndWith("into LA and vicinity to account for this potential."));
            Assert.That(result.Paragraphs[4], Does.StartWith("Later today, a secondary area of"));
            Assert.That(result.Paragraphs[4], Does.EndWith("include parts of the mid MO Valley."));
        });
    }

    [Test]
    public void ExtractImageNamesFromHtml_ReturnsImageNames_24Nov181200()
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
    public void CreateHeadline_ReturnsHeadline_24Nov181200()
    {
        var parsing = new OutlookProductParsing();
        var scripting = new OutlookProductScript();
        var narrative = parsing.ExtractNarrativeFromHtml(_html);
        var product = parsing.ParseNarrative(narrative, DayNumber);
        var result = scripting.CreateHeadline(product, DayNumber);

        Assert.That(result, Does.StartWith("6AM Day 1 Convective Outlook for Monday, November 18."));
    }
}
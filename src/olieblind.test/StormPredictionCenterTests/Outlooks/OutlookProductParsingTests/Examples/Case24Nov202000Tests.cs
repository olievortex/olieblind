using olieblind.lib.StormPredictionCenter.Outlooks;

namespace olieblind.test.StormPredictionCenterTests.Outlooks.OutlookProductParsingTests.Examples;

public class Case24Nov202000Tests
{
    private readonly string _html = File.ReadAllText("./Resources/SpcDayOne/24Nov20_2000.htm");
    private const int DayNumber = 1;

    [Test]
    public void ParseNarrative_ReturnsModel_24Nov202000()
    {
        var parsing = new OutlookProductParsing();
        var narrative = parsing.ExtractNarrativeFromHtml(_html);
        var result = parsing.ParseNarrative(narrative, DayNumber);

        Assert.Multiple(() =>
        {
            Assert.That(result.EffectiveDate, Is.EqualTo(new DateTime(2024, 11, 20, 19, 50, 0)));
            Assert.That(result.ProductName, Is.EqualTo("Day 1 Convective Outlook"));
            Assert.That(result.Headline,
                Does.Match("NO SEVERE THUNDERSTORM AREAS FORECAST"));
            Assert.That(result.Headings, Has.Count.EqualTo(4));
            Assert.That(result.Headings[0], Does.Match("SUMMARY"));
            Assert.That(result.Headings[1], Does.Match("20z Update"));
            Assert.That(result.Headings[2], Does.Match("Mid-Atlantic into North Carolina"));
            Assert.That(result.Headings[3], Does.Match("Coastal Pacific Northwest"));
            Assert.That(result.Paragraphs, Has.Count.EqualTo(4));
            Assert.That(result.Paragraphs[0], Does.StartWith("Severe thunderstorms appear unlikely through"));
            Assert.That(result.Paragraphs[0], Does.EndWith("through tonight across the contiguous United States."));
            Assert.That(result.Paragraphs[1], Does.StartWith("No changes are needed to the current D1 Convective"));
            Assert.That(result.Paragraphs[1], Does.EndWith("See previous discussion below for more information."));
            Assert.That(result.Paragraphs[2], Does.StartWith("Midday water-vapor imagery shows a cyclone over the"));
            Assert.That(result.Paragraphs[2], Does.EndWith("extent, which should limit the storm intensity."));
            Assert.That(result.Paragraphs[3], Does.StartWith("A powerful upper cyclone west of British Columbia"));
            Assert.That(result.Paragraphs[3], Does.EndWith("will remain scant---precluding a severe risk."));
        });
    }

    [Test]
    public void ExtractImageNamesFromHtml_ReturnsImageNames_24Nov202000()
    {
        var parsing = new OutlookProductParsing();
        var result = parsing.ExtractImageNamesFromHtml(_html, DayNumber);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(4));
            Assert.That(result[0], Is.EqualTo("day1otlk_2000.gif"));
            Assert.That(result[1], Is.EqualTo("day1probotlk_2000_torn.gif"));
            Assert.That(result[2], Is.EqualTo("day1probotlk_2000_wind.gif"));
            Assert.That(result[3], Is.EqualTo("day1probotlk_2000_hail.gif"));
        });
    }

    [Test]
    public void CreateHeadline_ReturnsHeadline_24Nov202000()
    {
        var parsing = new OutlookProductParsing();
        var scripting = new OutlookProductScript();
        var narrative = parsing.ExtractNarrativeFromHtml(_html);
        var product = parsing.ParseNarrative(narrative, DayNumber);
        var result = scripting.CreateHeadline(product, DayNumber);

        Assert.That(result, Does.StartWith("1PM Day 1 Convective Outlook for Wednesday, November 20."));
    }
}
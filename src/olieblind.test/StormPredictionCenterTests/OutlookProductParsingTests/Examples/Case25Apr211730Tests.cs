using olieblind.lib.StormPredictionCenter.Outlooks;

namespace olieblind.test.StormPredictionCenterTests.OutlookProductParsingTests.Examples;

public class Case25Apr211730Tests
{
    private readonly string _html = File.ReadAllText("./Resources/SpcDayTwo/25Apr21_1730.htm");
    private const int DayNumber = 2;

    [Test]
    public void ParseNarrative_ReturnsModel_24Nov181200()
    {
        var parsing = new OutlookProductParsing();
        var narrative = parsing.ExtractNarrativeFromHtml(_html);
        var result = parsing.ParseNarrative(narrative, DayNumber);

        Assert.Multiple(() =>
        {
            Assert.That(result.EffectiveDate, Is.EqualTo(new DateTime(2025, 4, 21, 17, 20, 0)));
            Assert.That(result.ProductName, Is.EqualTo("Day 2 Convective Outlook"));
            Assert.That(result.Headline,
                Does.Match(
                    "THERE IS A SLIGHT RISK OF SEVERE THUNDERSTORMS ACROSS THE SOUTHERN HIGH PLAINS AND INTO PARTS OF SOUTHERN KANSAS"));
            Assert.That(result.Headings, Has.Count.EqualTo(7));
            Assert.That(result.Headings[0], Does.Match("SUMMARY"));
            Assert.That(result.Headings[1], Does.Match("Synopsis"));
            Assert.That(result.Headings[2], Does.Match("Southern High Plains"));
            Assert.That(result.Headings[3], Does.Match(string.Empty));
            Assert.That(result.Headings[4], Does.Match("Northern Panhandle into Kansas"));
            Assert.That(result.Headings[5], Does.Match("IA/WI/IL"));
            Assert.That(result.Headings[6], Does.Match("Carolinas"));
            Assert.That(result.Paragraphs, Has.Count.EqualTo(7));
            Assert.That(result.Paragraphs[0], Does.StartWith("Isolated to scattered severe thunderstorms are"));
            Assert.That(result.Paragraphs[0], Does.EndWith("on Tuesday across portions of the southern Plains."));
            Assert.That(result.Paragraphs[1], Does.StartWith("A mostly zonal pattern will be in place across"));
            Assert.That(result.Paragraphs[1], Does.EndWith("severe storms along the dryline Tuesday afternoon."));
            Assert.That(result.Paragraphs[2], Does.StartWith("Low-level moisture will advect northward across West"));
            Assert.That(result.Paragraphs[2], Does.EndWith("boundaries could enhance low-level vorticity ingestion."));
            Assert.That(result.Paragraphs[3], Does.StartWith("Upscale growth may eventually occur with one or"));
            Assert.That(result.Paragraphs[3], Does.EndWith("supported by a modestly enhanced low-level jet."));
            Assert.That(result.Paragraphs[4], Does.StartWith("A surface front is forecast to extend across Kansas"));
            Assert.That(result.Paragraphs[4], Does.EndWith("for isolated large hail and severe wind gusts."));
            Assert.That(result.Paragraphs[5], Does.StartWith("A stalled frontal boundary will be in place across"));
            Assert.That(result.Paragraphs[5], Does.EndWith("could produce sporadic hail or damaging wind gusts."));
            Assert.That(result.Paragraphs[6], Does.StartWith("A cold front associated with a surface low over"));
            Assert.That(result.Paragraphs[6], Does.EndWith("periodically greater large hail/damaging wind threat."));
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
            Assert.That(result[0], Is.EqualTo("day2otlk_1730.gif"));
            Assert.That(result[1], Is.EqualTo("day2probotlk_1730_torn.gif"));
            Assert.That(result[2], Is.EqualTo("day2probotlk_1730_wind.gif"));
            Assert.That(result[3], Is.EqualTo("day2probotlk_1730_hail.gif"));
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

        Assert.That(result, Does.StartWith("12PM Day 2 Convective Outlook for Tuesday, April 22."));
    }
}
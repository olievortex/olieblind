using olieblind.lib.StormPredictionCenter.Outlooks;

namespace olieblind.test.StormPredictionCenterTests.OutlookProductParsingTests;

public class ExtractFromHtmlTests
{
    [Test]
    public void ExtractFromHtml_ReturnsNarrative_ValidSpcProduct()
    {
        const string html = "Sushi\n<pre>\nNoSushiForYou\n   </pre>";
        var testable = new OutlookProductParsing();
        var result = testable.ExtractNarrativeFromHtml(html);
        Assert.That(result, Is.EqualTo("\nNoSushiForYou\n   "));
    }

    [Test]
    public void ExtractFromHtml_ThrowsException_NotASpcProduct()
    {
        const string html = "Sushi\n<h>\nNoSushiForYou\n   </h>";
        var testable = new OutlookProductParsing();
        Assert.Throws<ApplicationException>(() => testable.ExtractNarrativeFromHtml(html));
    }
}
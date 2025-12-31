using olieblind.lib.StormPredictionCenter.Models;
using olieblind.lib.StormPredictionCenter.Outlooks;

namespace olieblind.test.StormPredictionCenterTests.OutlookProductParsingTests;

public class CategorizeLineTests
{
    private const int DayNumber = 1;

    [Test]
    public void CategorizeLine_BlankLine_BlankLine()
    {
        const string text = "   ";
        var result = OutlookProductParsing.CategorizeLine(text, DayNumber);
        Assert.That(result, Is.EqualTo(OutlookLineTypeEnum.BlankLine));
    }

    [Test]
    public void CategorizeLine_MessageStructure_ProductId()
    {
        const string text = "   SPC AC 201950 ";
        var result = OutlookProductParsing.CategorizeLine(text, DayNumber);
        Assert.That(result, Is.EqualTo(OutlookLineTypeEnum.MessageStructure));
    }

    [Test]
    public void CategorizeLine_MessageStructure_ProductName()
    {
        const string text = "   Day 1 Convective Outlook ";
        var result = OutlookProductParsing.CategorizeLine(text, DayNumber);
        Assert.That(result, Is.EqualTo(OutlookLineTypeEnum.ProductName));
    }

    [Test]
    public void CategorizeLine_MessageStructure_WeatherOffice()
    {
        const string text = "   NWS Storm Prediction Center Norman OK ";
        var result = OutlookProductParsing.CategorizeLine(text, DayNumber);
        Assert.That(result, Is.EqualTo(OutlookLineTypeEnum.MessageStructure));
    }

    [Test]
    public void CategorizeLine_EffectiveDate_EffectiveDate()
    {
        const string text = "   0150 PM CST Wed Nov 20 2024 ";
        var result = OutlookProductParsing.CategorizeLine(text, DayNumber);
        Assert.That(result, Is.EqualTo(OutlookLineTypeEnum.EffectiveDate));
    }

    [Test]
    public void CategorizeLine_MessageStructure_ValidPeriod()
    {
        const string text = "   Valid 202000Z - 211200Z ";
        var result = OutlookProductParsing.CategorizeLine(text, DayNumber);
        Assert.That(result, Is.EqualTo(OutlookLineTypeEnum.MessageStructure));
    }

    [Test]
    public void CategorizeLine_Header_CompleteHeader()
    {
        const string text = "   ...NO SEVERE THUNDERSTORM AREAS FORECAST... ";
        var result = OutlookProductParsing.CategorizeLine(text, DayNumber);
        Assert.That(result, Is.EqualTo(OutlookLineTypeEnum.Header));
    }

    [Test]
    public void CategorizeLine_HeaderStart_HeaderStart()
    {
        const string text = "   ...THERE IS A SLIGHT RISK OF SEVERE THUNDERSTORMS ACROSS PARTS OF ";
        var result = OutlookProductParsing.CategorizeLine(text, DayNumber);
        Assert.That(result, Is.EqualTo(OutlookLineTypeEnum.HeaderStart));
    }

    [Test]
    public void CategorizeLine_HeaderStart_HeaderEnd()
    {
        const string text = "   AND FAR SOUTHERN ARKANSAS... ";
        var result = OutlookProductParsing.CategorizeLine(text, DayNumber);
        Assert.That(result, Is.EqualTo(OutlookLineTypeEnum.HeaderEnd));
    }

    [Test]
    public void CategorizeLine_MessageStructure_Forecasters()
    {
        const string text = "   ..Thornton/Smith.. 11/20/2024 ";
        var result = OutlookProductParsing.CategorizeLine(text, DayNumber);
        Assert.That(result, Is.EqualTo(OutlookLineTypeEnum.MessageStructure));
    }

    [Test]
    public void CategorizeLine_MessageStructure_PreviousDiscussion()
    {
        const string text = "   .PREV DISCUSSION... /ISSUED 1030 AM CST Wed Nov 20 2024/ ";
        var result = OutlookProductParsing.CategorizeLine(text, DayNumber);
        Assert.That(result, Is.EqualTo(OutlookLineTypeEnum.MessageStructure));
    }

    [Test]
    public void CategorizeLine_MessageStructure_Hyperlink()
    {
        const string text =
            "   CLICK TO GET <a href=\"/products/outlook/archive/2024/KWNSPTSDY1_202411202000.txt\">WUUS01 PTSDY1</a> PRODUCT ";
        var result = OutlookProductParsing.CategorizeLine(text, DayNumber);
        Assert.That(result, Is.EqualTo(OutlookLineTypeEnum.MessageStructure));
    }

    [Test]
    public void CategorizeLine_MessageStructure_JavaScript()
    {
        const string text = "   <script type=\"text/javascript\" src=\"/misc/utctime.js\"></script> ";
        var result = OutlookProductParsing.CategorizeLine(text, DayNumber);
        Assert.That(result, Is.EqualTo(OutlookLineTypeEnum.MessageStructure));
    }

    [Test]
    public void CategorizeLine_MessageStructure_Note()
    {
        const string text = "   NOTE: THE NEXT DAY 1 OUTLOOK IS SCHEDULED BY 0100Z ";
        var result = OutlookProductParsing.CategorizeLine(text, DayNumber);
        Assert.That(result, Is.EqualTo(OutlookLineTypeEnum.MessageStructure));
    }

    [Test]
    public void CategorizeLine_MessageStructure_Sentence()
    {
        const string text = "   low-level warm advection across southern New England as a surface ";
        var result = OutlookProductParsing.CategorizeLine(text, DayNumber);
        Assert.That(result, Is.EqualTo(OutlookLineTypeEnum.Sentence));
    }
}
using olieblind.lib.StormPredictionCenter.Outlooks;

namespace olieblind.test.StormPredictionCenterTests.OutlookProductParsingTests;

public class SpcProductParsingTests
{
    private const int DayNumber = 1;

    [Test]
    public void ParseHeader_RemovesTripleDots_ExtremeExample()
    {
        const string example = "   ...THIS IS A...REALLY WEIRD \n   HEADER... ";
        var result = OutlookProductParsing.ParseHeader(example);

        Assert.That(result, Is.EqualTo("THIS IS A REALLY WEIRD HEADER"));
    }

    [Test]
    public void ParseNarrative_BuildsParagraph_MultiLineHeader()
    {
        const string example =
            "...HEADLINE...\n...Heading 1...\nSome random text\n\n...Heading 2\nis multi-line...\nMeow\n\n";

        var testable = new OutlookProductParsing();
        var result = testable.ParseNarrative(example, DayNumber);

        Assert.Multiple(() =>
        {
            Assert.That(result.Headline, Is.EqualTo("HEADLINE"));
            Assert.That(result.Headings, Has.Count.EqualTo(2));
            Assert.That(result.Headings[0], Is.EqualTo("Heading 1"));
            Assert.That(result.Headings[1], Is.EqualTo("Heading 2 is multi-line"));
            Assert.That(result.Paragraphs, Has.Count.EqualTo(2));
            Assert.That(result.Paragraphs[0], Is.EqualTo("Some random text"));
            Assert.That(result.Paragraphs[1], Is.EqualTo("Meow"));
        });
    }

    #region ExtractImageNamesFromHtml

    [Test]
    public void ExtractImageNamesFromHtml_Parse_Day3()
    {
        // Arrange
        const string html = """
                            <table id="elements">
                            <tr>
                            <td><a OnClick="show_tab('otlk_1930')" OnMouseOver="show_tab('otlk_1930')" title="Categorical Outlook">Categorical</a></td>
                            <td><a OnClick="show_tab('prob_1930')" OnMouseOver="show_tab('prob_1930')" title="Probability of severe weather within 25 miles of a point. Hatched Area: 10% or greater probability of significant severe weather within 25 miles of a point.">Probabilistic</a></td>
                            </tr>
                            </table>
                            """;
        const int dayNumber = 3;
        var testable = new OutlookProductParsing();

        // Act
        var result = testable.ExtractImageNamesFromHtml(html, dayNumber);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public void ExtractImageNamesFromHtml_ThrowsException_EmptyText()
    {
        var testable = new OutlookProductParsing();
        Assert.Throws<ApplicationException>(() =>
            testable.ExtractImageNamesFromHtml(string.Empty, DayNumber));
    }

    #endregion

    #region ReplaceAcronym

    [Test]
    public void ReplaceAcronym_Replaces_Beginning()
    {
        // Arrange
        const string source = "MN is a state in the midwest.";
        const string expected = "Minnesota is a state in the midwest.";
        var rule = new KeyValuePair<string, string>("MN", "Minnesota");

        // Act
        var result = OutlookProductScript.ReplaceAcronym(source, rule, StringComparison.Ordinal);

        //
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ReplaceAcronym_Replaces_Ending()
    {
        // Arrange
        const string source = "There are 10000 lakes in the state of MN";
        const string expected = "There are 10000 lakes in the state of Minnesota";
        var rule = new KeyValuePair<string, string>("MN", "Minnesota");

        // Act
        var result = OutlookProductScript.ReplaceAcronym(source, rule, StringComparison.Ordinal);

        //
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ReplaceAcronym_Replaces_Middle()
    {
        // Arrange
        const string source = "There are 10000 lakes in the state of MN.";
        const string expected = "There are 10000 lakes in the state of Minnesota.";
        var rule = new KeyValuePair<string, string>("MN", "Minnesota");

        // Act
        var result = OutlookProductScript.ReplaceAcronym(source, rule, StringComparison.Ordinal);

        //
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ReplaceAcronym_NoReplace_NextToLetter()
    {
        // Arrange
        const string source = "There are 10000 lakes in the state of EMN.";
        const string expected = "There are 10000 lakes in the state of EMN.";
        var rule = new KeyValuePair<string, string>("MN", "Minnesota");

        // Act
        var result = OutlookProductScript.ReplaceAcronym(source, rule, StringComparison.Ordinal);

        //
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion
}
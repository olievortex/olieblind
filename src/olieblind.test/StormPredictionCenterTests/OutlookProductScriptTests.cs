using olieblind.lib.StormPredictionCenter.Models;
using olieblind.lib.StormPredictionCenter.Outlooks;

namespace olieblind.test.StormPredictionCenterTests;

public class OutlookProductScriptTests
{
    private const int DayNumber = 1;

    #region CreateHeadline

    [Test]
    public void CreateHeadline_CorrectTimeDay1_WinterPMEffectiveDate()
    {
        // Arrange
        var effectiveDate = new DateTime(2024, 11, 24, 20, 4, 0, DateTimeKind.Utc);
        var model = new OutlookProductModel
        {
            EffectiveDate = effectiveDate,
            ProductName = "severe weather outlook"
        };
        var testable = new OutlookProductScript();

        // Act
        var script = testable.CreateHeadline(model, DayNumber);

        // Assert
        Assert.That(script, Is.EqualTo("2PM severe weather outlook for Sunday, November 24."));
    }

    [Test]
    public void CreateHeadline_CorrectTimeDay1_SummerPMEffectiveDate()
    {
        // Arrange
        var effectiveDate = new DateTime(2024, 7, 1, 18, 4, 0, DateTimeKind.Utc);
        var model = new OutlookProductModel
        {
            EffectiveDate = effectiveDate,
            ProductName = "severe weather outlook"
        };
        var testable = new OutlookProductScript();

        // Act
        var script = testable.CreateHeadline(model, DayNumber);

        // Assert
        Assert.That(script, Is.EqualTo("1PM severe weather outlook for Monday, July 1."));
    }

    [Test]
    public void CreateHeadline_CorrectTimeDay1_WinterNoonEffectiveDate()
    {
        // Arrange
        var effectiveDate = new DateTime(2024, 11, 24, 18, 4, 0, DateTimeKind.Utc);
        var model = new OutlookProductModel
        {
            EffectiveDate = effectiveDate,
            ProductName = "severe weather outlook"
        };
        var testable = new OutlookProductScript();

        // Act
        var script = testable.CreateHeadline(model, DayNumber);

        // Assert
        Assert.That(script, Is.EqualTo("12PM severe weather outlook for Sunday, November 24."));
    }

    [Test]
    public void CreateHeadline_CorrectTimeDay1_WinterAMEffectiveDate()
    {
        // Arrange
        var effectiveDate = new DateTime(2024, 11, 24, 16, 4, 0, DateTimeKind.Utc);
        var model = new OutlookProductModel
        {
            EffectiveDate = effectiveDate,
            ProductName = "severe weather outlook"
        };
        var testable = new OutlookProductScript();

        // Act
        var script = testable.CreateHeadline(model, DayNumber);

        // Assert
        Assert.That(script, Is.EqualTo("10AM severe weather outlook for Sunday, November 24."));
    }

    [Test]
    public void CreateHeadline_CorrectTimeDay2_WinterAMEffectiveDate()
    {
        // Arrange
        const int dayNumber = 2;
        var effectiveDate = new DateTime(2024, 11, 24, 16, 4, 0, DateTimeKind.Utc);
        var model = new OutlookProductModel
        {
            EffectiveDate = effectiveDate,
            ProductName = "severe weather outlook"
        };
        var testable = new OutlookProductScript();

        // Act
        var script = testable.CreateHeadline(model, dayNumber);

        // Assert
        Assert.That(script, Is.EqualTo("10AM severe weather outlook for Monday, November 25."));
    }

    #endregion

    #region RephraseHeading

    [Test]
    public void RephraseHeading_Echos_NoFluff()
    {
        Assert.That(OutlookProductScript.RephraseHeading("sushi"), Is.EqualTo("sushi"));
    }

    [Test]
    public void RephraseHeading_RemovesFluff_VariousFluff()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(OutlookProductScript.RephraseHeading("summary"), Is.Empty);
            Assert.That(OutlookProductScript.RephraseHeading("discussion"), Is.Empty);
            Assert.That(OutlookProductScript.RephraseHeading("synopsis"), Is.Empty);
            Assert.That(OutlookProductScript.RephraseHeading("synopsis/discussion"), Is.Empty);
            Assert.That(OutlookProductScript.RephraseHeading("Synopsis and Discussion"), Is.Empty);
        }
    }

    #endregion

    #region CreateTranscript Tests

    [Test]
    public void CreateTranscript_ThrowsException_MismatchedLengths()
    {
        // Arrange
        List<string> a = ["1", "2", "3"];
        List<string> b = ["1", "2", "3", "4"];
        var efDate = DateTime.UtcNow;
        var model = new OutlookProductModel
        {
            EffectiveDate = efDate
        };
        model.Headings.AddRange(a);
        model.Headings.AddRange(b);
        var testable = new OutlookProductScript();

        // Act, Assert
        Assert.Throws<ArgumentException>(() =>
            testable.CreateDefaultTranscript(model, DayNumber));
    }

    [Test]
    public void CreateTranscript_ReturnsScript_ValidParameters()
    {
        // Arrange
        const string headline = "headline";
        const string product = "Day 1 Outlook";
        List<string> a = ["1", "2"];
        List<string> b = ["One is the loneliest number.", "Two is a couple."];
        var efDate = new DateTime(2020, 01, 01);
        var model = new OutlookProductModel
        {
            EffectiveDate = efDate,
            ProductName = product,
            Headline = headline
        };
        model.Headings.AddRange(a);
        model.Paragraphs.AddRange(b);
        var testable = new OutlookProductScript();

        // Act
        var result = testable.CreateDefaultTranscript(model, DayNumber);
        result = result.Replace("\r", string.Empty);

        // Assert
        Assert.That(result, Is.EqualTo("6PM Day 1 Outlook for Wednesday, January 1." +
                                       " headline\n1\nOne is the loneliest number.\n2\nTwo is a couple.\n"));
    }

    #endregion

    #region CreateDefaultScript Tests

    [Test]
    public void CreateDefaultScript_ThrowsException_MismatchedLengths()
    {
        // Arrange
        List<string> a = ["1", "2", "3"];
        List<string> b = ["1", "2", "3", "4"];
        var efDate = DateTime.UtcNow;
        var model = new OutlookProductModel
        {
            EffectiveDate = efDate
        };
        model.Headings.AddRange(a);
        model.Headings.AddRange(b);
        var testable = new OutlookProductScript();

        // Act, Assert
        Assert.Throws<ArgumentException>(() =>
            testable.CreateDefaultScript(model, DayNumber));
    }

    [Test]
    public void GenerateDefaultScript_ReturnsScript_ValidParameters()
    {
        // Arrange
        const string headline = "headline";
        const string product = "Day 1 Outlook";
        List<string> a = ["1", "2"];
        List<string> b = ["One is the loneliest number.", "Two is a couple."];
        var efDate = new DateTime(2020, 01, 01);
        var model = new OutlookProductModel
        {
            EffectiveDate = efDate,
            ProductName = product,
            Headline = headline
        };
        model.Headings.AddRange(a);
        model.Paragraphs.AddRange(b);
        var testable = new OutlookProductScript();

        // Act
        var result = testable.CreateDefaultScript(model, DayNumber);
        result = result.Replace("\r", "");

        // Assert
        Assert.That(result, Is.EqualTo("6PM Day 1 Outlook for Wednesday, January 1." +
                                       " headline\n1\nOne is the loneliest number.\n2\nTwo is a couple.\n"));
    }

    #endregion

    [Test]
    public void ReplaceAcronyms_NoException_ValidParameters()
    {
        // Arrange, Act, Assert
        OutlookProductScript.ReplaceAcronyms("WA is a state on the west coast.");
    }

    [Test]
    public void ReplaceAcronyms_ReturnsCAPE_CaliforniaSubstitution()
    {
        // Arrange
        const string value = "Too much fires in CA, but the CAPE isn't high enough for thunderstorms.";

        // Act
        var result = OutlookProductScript.ReplaceAcronyms(value);

        // Assert
        Assert.That(result,
            Is.EqualTo("Too much fires in California, but the CAPE isn't high enough for thunderstorms."));
    }

    [Test]
    public void ReplaceAcronyms_ReturnsGulfOfMexico_GOMSubstitution()
    {
        // Arrange
        const string value =
            "into the western Gulf of Mexico (GOM), with a weak low ahead of this influx from the west-central into the northern GOM.";

        // Act
        var result = OutlookProductScript.ReplaceAcronyms(value);

        // Assert
        Assert.That(result,
            Is.EqualTo(
                "into the western Gulf of Mexico, with a weak low ahead of this influx from the west-central into the northern Gulf of Mexico."));
    }
}
using olieblind.lib.DroughtMonitor;
using olieblind.lib.DroughtMonitor.Models;

namespace olieblind.test.DroughtMonitorTests;

public class DroughtMonitorScriptingTests
{
    [Test]
    public void CreateHeadline_CorrectDate_UtcEffectiveDate()
    {
        // Arrange
        var effectiveDate = new DateTime(2024, 11, 24, 20, 4, 0, DateTimeKind.Utc);
        var model = new DroughtMonitorProductModel
        {
            EffectiveDate = effectiveDate
        };

        // Act
        var script = DroughtMonitorScripting.CreateHeadline(model);

        // Assert
        Assert.That(script, Is.EqualTo("U.S. Drought Monitor discussion for Sunday, November 24."));
    }

    [Test]
    public void CreateDefaultTitle_CorrectTitle_UtcEffectiveDate()
    {
        // Arrange
        var effectiveDate = new DateTime(2024, 11, 24, 20, 4, 0, DateTimeKind.Utc);
        var model = new DroughtMonitorProductModel
        {
            EffectiveDate = effectiveDate
        };
        var testable = new DroughtMonitorScripting();

        // Act
        var script = testable.CreateDefaultTitle(model);

        // Assert
        Assert.That(script, Is.EqualTo("U.S. Drought Monitor 11/24."));
    }

    [Test]
    public void CreateHeading_CorrectHeading_ValidRegion()
    {
        // Arrange
        const string region = "Dillon";

        // Act
        var script = DroughtMonitorScripting.CreateHeading(region);

        // Assert
        Assert.That(script, Is.EqualTo("Dillon region."));
    }

    [Test]
    public void CreateDefaultTranscript_ReturnsTranscript_ValidInput()
    {
        // Arrange
        var efDate = new DateTime(2020, 01, 01);
        var model = new DroughtMonitorProductModel
        {
            EffectiveDate = efDate,
            Intro = "Intro.",
            Forecast = "Forecast."
        };
        model.Regions.Add("A", "A stuff.");
        model.Regions.Add("B", "B NWS stuff.");
        model.Regions.Add("Pacific", "Pacific stuff.");
        model.Regions.Add("Caribbean", "Caribbean stuff.");
        var testable = new DroughtMonitorScripting();

        // Act
        var result = testable.CreateDefaultTranscript(model);
        result = result.Replace("\r", string.Empty);

        // Assert
        Assert.That(result, Is.EqualTo("U.S. Drought Monitor discussion for Wednesday, January 1.\n" +
                                       "Intro.\nForecast.\nA region.\nA stuff.\nB region.\nB NWS stuff.\n"));
    }

    [Test]
    public void CreateDefaultScript_ReturnsScript_ValidInput()
    {
        // Arrange
        var ct = CancellationToken.None;
        var efDate = new DateTime(2020, 01, 01);
        var model = new DroughtMonitorProductModel
        {
            EffectiveDate = efDate,
            Intro = "Intro.",
            Forecast = "Forecast."
        };
        model.Regions.Add("A", "A stuff.");
        model.Regions.Add("B", "B NWS stuff.");
        model.Regions.Add("Pacific", "Pacific stuff.");
        model.Regions.Add("Caribbean", "Caribbean stuff.");
        var testable = new DroughtMonitorScripting();

        // Act
        var result = testable.CreateDefaultScript(model, ct);
        result = result.Replace("\r", string.Empty);

        // Assert
        Assert.That(result, Is.EqualTo("U.S. Drought Monitor discussion for Wednesday, January 1.\n" +
                                       "Intro.\nForecast.\nA region.\nA stuff.\nB region.\n" +
                                       "B National Weather Service stuff.\n"));
    }

    [Test]
    public void ReplaceAcronyms_NoException_ValidParameters()
    {
        // Arrange, Act
        var result = DroughtMonitorScripting.ReplaceAcronyms("NWS forecasts for the west coast");

        // Assert
        Assert.That(result, Is.EqualTo("National Weather Service forecasts for the west coast"));
    }
}
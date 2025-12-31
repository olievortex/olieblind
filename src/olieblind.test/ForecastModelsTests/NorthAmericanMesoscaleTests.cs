using olieblind.lib.ForecastModels;

namespace olieblind.test.ForecastModelsTests;

public class NorthAmericanMesoscaleTests
{
    #region GetImageFolder

    [Test]
    public void GetFolder_Folder_Valid()
    {
        // Arrange
        const string baseFolder = "/var/www/videos/";
        var effectiveDate = new DateOnly(2025, 7, 18);
        var testable = new NorthAmericanMesoscale();

        // Act
        var result = testable.GetFolder(effectiveDate, baseFolder);

        // Assert
        Assert.That(result, Is.EqualTo("/var/www/videos/2025/07/nam.20250718"));
    }

    #endregion

    #region GetNcepUrl

    [Test]
    public void GetNcepUrl_Url_Valid()
    {
        // Arrange
        var effectiveDay = new DateOnly(2024, 7, 15);
        const int effectiveHour = 6;
        const int forecastHour = 18;
        var testable = new NorthAmericanMesoscale();

        // Act
        var result = testable.GetNcepUrl(effectiveDay, effectiveHour, forecastHour);

        // Assert
        Assert.That(result, Is.EqualTo("https://nomads.ncep.noaa.gov/pub/data/nccf/com/nam/prod/nam.20240715/nam.t06z.awphys18.tm00.grib2"));
    }

    #endregion
}

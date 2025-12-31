namespace olieblind.lib.ForecastModels;

public class NorthAmericanMesoscale : INorthAmericanMesoscale
{
    public string GetNcepUrl(DateOnly effectiveDate, int effectiveHour, int forecastHour)
    {
        var subfolder = $"nam.{effectiveDate:yyyyMMdd}";
        var filename = $"nam.t{effectiveHour:00}z.awphys{forecastHour:00}.tm00.grib2";
        var url = $"https://nomads.ncep.noaa.gov/pub/data/nccf/com/nam/prod/{subfolder}/{filename}";

        return url;
    }

    public string GetFolder(DateOnly effectiveDate, string baseFolder)
    {
        baseFolder = baseFolder.TrimEnd('/');
        var subFolder = $"{effectiveDate.Year}/{effectiveDate.Month:00}";
        var timestamp = $"{effectiveDate.Year}{effectiveDate.Month:00}{effectiveDate.Day:00}";
        var result = $"{baseFolder}/{subFolder}/nam.{timestamp}";

        return result;
    }
}

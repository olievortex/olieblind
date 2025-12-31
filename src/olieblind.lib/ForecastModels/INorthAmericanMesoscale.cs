namespace olieblind.lib.ForecastModels;

public interface INorthAmericanMesoscale
{
    string GetNcepUrl(DateOnly effectiveDay, int effectiveHour, int forecastHour);
    string GetFolder(DateOnly effectiveDate, string baseFolder);
}

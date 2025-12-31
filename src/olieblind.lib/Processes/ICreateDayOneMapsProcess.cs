namespace olieblind.lib.Processes;

public interface ICreateDayOneMapsProcess
{
    Task RunAsync(DateOnly effectiveDateOnly, int effectiveHour, int forecastHour, CancellationToken ct);
}

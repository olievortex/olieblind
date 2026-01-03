namespace olieblind.lib.Processes.Interfaces;

public interface ICreateDayOneMapsProcess
{
    Task Run(DateOnly effectiveDateOnly, int effectiveHour, int forecastHour, CancellationToken ct);
}

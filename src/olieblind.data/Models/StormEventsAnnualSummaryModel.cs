namespace olieblind.data.Models;

public class StormEventsAnnualSummaryModel
{
    public int Year { get; init; }
    public int SevereDays { get; init; }
    public int HailReports { get; init; }
    public int WindReports { get; init; }
    public int ExtremeTornadoes { get; init; }
    public int StrongTornadoes { get; init; }
    public int OtherTornadoes { get; init; }
}
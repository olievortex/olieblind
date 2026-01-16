using olieblind.data.Entities;

namespace olieblind.lib.StormEvents.Models;

public class AnnualOverviewModel
{
    public class AnnualOverviewItem
    {
        public string Id { get; init; } = string.Empty;
        public string SourceFk { get; init; } = string.Empty;
        public int HailCount { get; init; }
        public int TornadoCount { get; init; }
        public int WindCount { get; init; }

        public AnnualOverviewItem(StormEventsDailySummaryEntity entity)
        {
            Id = entity.Id;
            SourceFk = entity.SourceFk;
            HailCount = entity.Hail;
            TornadoCount = entity.F1 + entity.F2 + entity.F3 + entity.F4 + entity.F5;
            WindCount = entity.Wind;
        }

        public AnnualOverviewItem()
        {

        }
    }

    public List<AnnualOverviewItem> HailTop10 { get; init; } = [];
    public List<AnnualOverviewItem> TornadoTop10 { get; init; } = [];
    public List<AnnualOverviewItem> WindTop10 { get; init; } = [];
    public List<AnnualOverviewItem> Recent10 { get; init; } = [];
    public List<AnnualOverviewItem> ExtremeTornadoes { get; init; } = [];
    public List<AnnualOverviewItem> StrongTornadoes { get; init; } = [];
}
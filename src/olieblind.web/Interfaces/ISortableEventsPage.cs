using olieblind.data.Entities;

namespace olieblind.web.Interfaces;

public interface ISortableEventsPage
{
    string Solid { get; }
    string Outline { get; }

    string CountyClass { get; set; }
    string MagnitudeClass { get; set; }
    string StateClass { get; set; }
    string TimeClass { get; set; }

    List<StormEventsDailyDetailEntity> EventList { get; set; }
}
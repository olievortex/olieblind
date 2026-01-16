using olieblind.data.Entities;
using olieblind.data.Models;

namespace olieblind.lib.StormEvents.Interfaces;

public interface IStormEventsSource
{
    DateTime? FromEffectiveDate(string value);

    Task<List<StormEventsAnnualSummaryModel>> GetAnnualSummaryList(CancellationToken ct);

    Task<StormEventsDailySummaryEntity?> GetDailySummaryByDate(string effectiveDate, int year, CancellationToken ct);

    Task<List<StormEventsDailySummaryEntity>> GetDailySummaryList(int year, CancellationToken ct);

    List<SatelliteAwsProductEntity> GetIemSatelliteList();

    Task<SpcMesoProductEntity?> GetMeso(int year, int id, CancellationToken ct);

    Task<List<SpcMesoProductEntity>> GetMesoList(string effectiveDate, CancellationToken ct);

    Task<RadarInventoryEntity?> GetRadarInventory(string radarId, string effectiveDate, string bucketName, CancellationToken ct);
}

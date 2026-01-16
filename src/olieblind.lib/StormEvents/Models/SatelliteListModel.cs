using olieblind.data.Entities;

namespace olieblind.lib.StormEvents.Models;

public class SatelliteListModel
{
    public List<SatelliteAwsProductEntity> IemList { get; init; } = [];
    public List<SatelliteAwsProductEntity> AwsList { get; init; } = [];
}
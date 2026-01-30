using olieblind.data.Entities;

namespace olieblind.lib.StormEvents.Models;

public class SatelliteListModel
{
    public List<SatelliteProductEntity> IemList { get; init; } = [];
    public List<SatelliteProductEntity> AwsList { get; init; } = [];
}
namespace olieblind.lib.Satellite.Models;

public class SatelliteRequestResultModel(bool IsSuccessful, string Message)
{
    public bool IsSuccessful { get; } = IsSuccessful;
    public string Message { get; } = Message;
}

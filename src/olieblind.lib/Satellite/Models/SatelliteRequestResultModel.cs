namespace olieblind.lib.Satellite.Models;

public class SatelliteRequestResultModel
{
    public bool IsSuccessful { get; init; }
    public string Message { get; init; }

    public SatelliteRequestResultModel(bool IsSuccessful, string Message)
    {
        this.IsSuccessful = IsSuccessful;
        this.Message = Message;
    }
}

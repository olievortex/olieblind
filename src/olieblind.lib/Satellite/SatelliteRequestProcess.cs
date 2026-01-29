using Azure.Messaging.ServiceBus;
using olieblind.lib.Models;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Satellite.Models;

namespace olieblind.lib.Satellite;

public class SatelliteRequestProcess(ISatelliteRequestSource business) : ISatelliteRequestProcess
{
    public const string NothingToDoMessage = "There is nothing to process for the selected date.";
    public const string QuotaExceededMessage = "Quota exceeded. Please try again later.";
    public const string SuccessMessage = "Satellite request submitted successfully.";

    public async Task<SatelliteRequestResultModel> RequestHourlySatellite(SatelliteRequestModel model, ServiceBusSender sender, CancellationToken ct)
    {
        var productList = await business.GetHourlyProductList(model.EffectiveDate, ct);
        if (productList.Count == 0) return new SatelliteRequestResultModel(false, NothingToDoMessage);

        var isFreeRequest = await business.IsFreeDay(model.EffectiveDate, model.SourceFk, ct);

        if (!isFreeRequest && await business.IsQuotaAvailable(model.UserId, ct) is false)
            return new SatelliteRequestResultModel(false, QuotaExceededMessage);

        await business.SendMessage(productList, sender, ct);
        await business.CreateUserLog(model.UserId, model.EffectiveDate, isFreeRequest, ct);

        return new SatelliteRequestResultModel(true, SuccessMessage);
    }
}

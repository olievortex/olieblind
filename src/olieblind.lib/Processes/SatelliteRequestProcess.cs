using Amazon.S3;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using olieblind.lib.Models;
using olieblind.lib.Services;

namespace olieblind.lib.Processes;

public class SatelliteRequestProcess(IOlieWebService ows)
//: ISatelliteMarqueeProcess
{
    //private readonly Point _finalSize = new(1246, 540);

    public async Task Run(ServiceBusReceiver receiver, string goldPath, IAmazonS3 awsClient,
        BlobContainerClient bronzeClient, Func<int, Task> delayFunc, CancellationToken ct)
    {
        do
        {
            var (message, model) = await ows.ServiceBusReceiveJson<SatelliteRequestQueueModel>(receiver, ct);
            if (message is null) break;

            //var product = await repo.SatelliteAwsProductGet(model.Id, ct);

            //await process.DownloadSatelliteFile(year, satellite, delayFunc, sender, bronzeClient, awsClient, ct);

            await ows.ServiceBusCompleteMessage(receiver, message, ct);
        } while (!ct.IsCancellationRequested);
    }

    //private async Task AdhocProcess(string goldPath, CancellationToken ct)
    //{
    //    var missingPosters = await repo.SatelliteAwsProductListNoPoster(ct);

    //    foreach (var missingPoster in missingPosters)
    //        await satelliteSource.MakeThumbnail(missingPoster, _finalSize, goldPath, ct);
    //}

    //public async Task AnnualProcess(int year, Func<int, Task> delayFunc, ServiceBusSender sender,
    //    BlobContainerClient bronzeClient, string goldPath, IAmazonS3 awsClient, CancellationToken ct)
    //{
    //    var missingPosters = await repo.StormEventsDailySummaryListMissingPostersForYear(year, ct);

    //    foreach (var missingPoster in missingPosters)
    //    {
    //        var satellite = await satelliteProcess.GetMarqueeSatelliteProduct(missingPoster, ct);
    //        if (satellite is null) continue;

    //        await satelliteProcess.DownloadSatelliteFile(year, satellite, delayFunc, sender, bronzeClient, awsClient, ct);
    //        await satelliteProcess.UpdateDailySummary(satellite, missingPoster, ct);
    //        await satelliteProcess.CreateThumbnailAndUpdateDailySummary(satellite, missingPoster, _finalSize, goldPath, ct);
    //    }
    //}
}
using Azure.Messaging.ServiceBus;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Services;
using SixLabors.ImageSharp;
using System.Diagnostics;

namespace olieblind.lib.Satellite;

public class SatelliteSource(IMyRepository repo, IOlieWebService ows, IOlieImageService ois) : ISatelliteSource
{
    public async Task<SatelliteProductEntity?> GetMarqueeSatelliteProduct(string effectiveDate, DateTime eventTime, CancellationToken ct)
    {
        var result =
            await repo.SatelliteProductGetPoster(effectiveDate, eventTime, ct) ??
            await repo.SatelliteProductGetLastPoster(effectiveDate, ct);

        return result;
    }

    public async Task MakeThumbnail(SatelliteProductEntity satellite, Point finalSize, string goldPath, CancellationToken ct)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        // Sanity check
        if (satellite.PathPoster is not null) return;
        if (satellite.Path1080 is null) throw new NullReferenceException($"Missing Path1080 for {satellite.Id}");

        // Download full sized image
        var filename1080 = $"{goldPath}/{satellite.Path1080}";
        var bytes = await ows.FileReadAllBytes(filename1080, ct);

        // Convert to poster image
        var filenamePoster = filename1080.Replace(".png", "_poster.png");
        var finalSizePoint = new System.Drawing.Point(finalSize.X, finalSize.Y);
        var resizedBytes = await ois.Resize(bytes, finalSizePoint, ct);
        await ows.FileWriteAllBytes(filenamePoster, resizedBytes, ct);

        // Update CosmosDb
        satellite.PathPoster = satellite.Path1080.Replace(".png", "_poster.png");
        satellite.Timestamp = DateTime.UtcNow;
        satellite.TimeTakenPoster = (int)stopwatch.Elapsed.TotalSeconds;
        await repo.SatelliteProductUpdate(satellite, ct);
    }

    public async Task MessagePurple(SatelliteProductEntity satellite, ServiceBusSender sender, CancellationToken ct)
    {
        if (satellite.Path1080 is not null || satellite.PathSource is null) return;

        await ows.ServiceBusSendJson(sender, satellite, ct);
    }
}

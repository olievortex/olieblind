using Azure.Storage.Blobs;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Services;
using olieblind.lib.StormPredictionCenter.Interfaces;
using System.Net;

namespace olieblind.lib.StormPredictionCenter.Mesos;

public class MesoProductSource(IOlieWebService ows, IMyRepository repo, IOlieImageService ois) : IMesoProductSource
{
    private const string BaseUrl = "https://www.spc.noaa.gov/products/md/";

    public async Task<string?> DownloadHtml(int year, int index, CancellationToken ct)
    {
        var url = $"{BaseUrl}{year}/md{index:0000}.html";

        var (response, _, html) = await ows.ApiGet(url, null, ct);
        if (response == HttpStatusCode.NotFound) return null;
        if (response != HttpStatusCode.OK || html is null)
            throw new ApplicationException($"Could not download for {year}, {index}");

        return html;
    }

    public async Task<string?> StoreImage(string? imageName, SpcMesoProductEntity product, string goldPath, CancellationToken ct)
    {
        if (product.GraphicUrl is not null || imageName is null) return null;

        var dt = product.EffectiveTime;
        var url = $"{BaseUrl}{dt.Year}/{imageName}";
        var image = await ows.ApiGetBytes(url, ct);

        var imageFileName = await ois.SafeConvert(image, $"{goldPath}/gold/spc/meso/{dt.Year}/{dt.Month}/{imageName}", ".gif", ct);

        var imageUrl = imageFileName.Replace(goldPath, string.Empty);
        imageUrl = imageUrl.TrimStart('/');

        return imageUrl;
    }

    public async Task<string> StoreHtml(int index, string html, DateTime effectiveDate, BlobContainerClient bronze, CancellationToken ct)
    {
        var blobPath = $"bronze/spc/meso/{effectiveDate.Year}/{effectiveDate.Month}/md{index:D4}.html";
        await ows.BlobUploadText(bronze, blobPath, html, ct);

        return blobPath;
    }

    public async Task<int> GetLatestIdForYear(int year, CancellationToken ct)
    {
        var latest = await repo.SpcMesoProductGetLatest(year, ct);

        return latest?.Id ?? 0;
    }
}
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Services;
using olieblind.lib.StormPredictionCenter.Interfaces;
using System.Net;

namespace olieblind.lib.StormPredictionCenter.Mesos;

public class MesoProductSource(IOlieWebService ows, IMyRepository repo) : IMesoProductSource
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

    public async Task DownloadImage(string imageName, SpcMesoProductEntity product, string goldPath, CancellationToken ct)
    {
        if (product.GraphicUrl is not null) return;

        var dt = product.EffectiveTime;
        var url = $"{BaseUrl}{dt.Year}/{imageName}";
        var blobFileName = $"{goldPath}/gold/spc/meso/{dt.Year}/{dt.Month}/{imageName}";

        var image = await ows.ApiGetBytes(url, ct);
        await ows.FileWriteAllBytes(blobFileName, image, ct);

        product.GraphicUrl = blobFileName;
        product.Timestamp = DateTime.UtcNow;
        await repo.SpcMesoProductUpdate(product, ct);
    }

    public async Task UpdateCosmos(SpcMesoProductEntity existing, string areasAffected, string concerning, CancellationToken ct)
    {
        if (existing.AreasAffected == areasAffected && existing.Concerning == concerning) return;

        existing.AreasAffected = areasAffected;
        existing.Concerning = concerning;
        existing.Timestamp = DateTime.UtcNow;

        await repo.SpcMesoProductUpdate(existing, ct);
    }

    public async Task<int> GetLatestIdForYear(int year, CancellationToken ct)
    {
        var latest = await repo.SpcMesoProductGetLatest(year, ct);

        return latest?.Id ?? 0;
    }
}
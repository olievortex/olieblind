using Azure.Storage.Blobs;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.StormPredictionCenter.Interfaces;

namespace olieblind.lib.StormPredictionCenter.Mesos;

public class MesoProductProcess(IMesoProductSource source, IMesoProductParsing parse, IMyRepository repo) : IMesoProductProcess
{
    public async Task<bool> Download(int year, int index, string goldPath, BlobContainerClient bronze, CancellationToken ct)
    {
        var html = await source.DownloadHtml(year, index, ct);
        if (html is null) return false;

        var body = parse.GetBody(html);
        var effectiveTime = parse.GetEffectiveTime(body);

        var entity = new SpcMesoProductEntity
        {
            Id = index,
            EffectiveDate = effectiveTime.AddHours(-12).ToString("yyyy-MM-dd"),

            AreasAffected = parse.GetAreasAffected(body),
            Concerning = parse.GetConcerning(body),
            EffectiveTime = effectiveTime,
            Narrative = parse.GetNarrative(body),
            Timestamp = DateTime.UtcNow
        };

        var imageName = parse.GetImageName(html);

        entity.GraphicUrl = await source.StoreImage(imageName, entity, goldPath, ct);
        entity.Html = await source.StoreHtml(index, html, entity.EffectiveTime, bronze, ct);

        await repo.SpcMesoProductCreate(entity, ct);

        return true;
    }
}
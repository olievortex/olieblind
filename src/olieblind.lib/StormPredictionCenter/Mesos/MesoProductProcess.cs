using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.StormPredictionCenter.Interfaces;

namespace olieblind.lib.StormPredictionCenter.Mesos;

public class MesoProductProcess(IMesoProductSource source, IMesoProductParsing parse, IMyRepository repo) : IMesoProductProcess
{
    public async Task<bool> Download(int year, int index, string goldPath, CancellationToken ct)
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
            Html = html,
            Timestamp = DateTime.UtcNow
        };

        await repo.SpcMesoProductCreate(entity, ct);
        await source.DownloadImage(parse.GetImageName(html), entity, goldPath, ct);

        return true;
    }

    public async Task<bool> Update(int year, int index, CancellationToken ct)
    {
        var entity = await repo.SpcMesoProductGet(year, index, ct);
        if (entity is null) return false;

        var body = parse.GetBody(entity.Html);

        var updatedAreasAffected = parse.GetAreasAffected(body);
        var updatedConcerning = parse.GetConcerning(body);

        await source.UpdateCosmos(entity, updatedAreasAffected, updatedConcerning, ct);

        return true;
    }
}
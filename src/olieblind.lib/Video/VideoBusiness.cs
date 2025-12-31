using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Models;

namespace olieblind.lib.Video;

public class VideoBusiness(IMyRepository repo) : IVideoBusiness
{
    public async Task<ProductVideoModel?> GetVideoAsync(int id, CancellationToken ct)
    {
        var result = await repo.ProductVideoGet(id, ct);

        return result is null ? null : ProductVideoModel.Map(result);
    }

    public async Task<List<ProductVideoModel>> GetVideoListAsync(string category, CancellationToken ct)
    {
        var result = await repo.ProductVideoGetList(ct)
            .ContinueWith(c => c.Result
                .Where(w => w.Category == category)
                .Select(ProductVideoModel.Map)
                .ToList());

        return result;
    }

    public async Task<ProductVideoPageModel> GetIndexPageAsync(CancellationToken ct)
    {
        var recent = await repo.ProductVideoGetListMostRecent(ct);

        return new ProductVideoPageModel
        {
            SpcDayOne = Pivot(recent, "Day 1 Convective Outlook"),
            SpcDayTwo = Pivot(recent, "Day 2 Convective Outlook"),
            SpcDayThree = Pivot(recent, "Day 3 Convective Outlook"),
            DroughtMonitor = Pivot(recent, "Drought Monitor")
        };
    }

    public static ProductVideoModel? Pivot(List<ProductVideoEntity> items, string category)
    {
        var item = items.SingleOrDefault(s => s.Category == category);
        if (item == null) return null;

        return ProductVideoModel.Map(item);
    }
}